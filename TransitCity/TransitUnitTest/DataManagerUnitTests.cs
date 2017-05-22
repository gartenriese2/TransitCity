using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Time;
using Transit;
using Transit.Data;
using Transit.Timetable;
using Transit.Timetable.Algorithm;
using Utility.Timing;
using Utility.Units;
using Utility.Extensions;

namespace TransitUnitTest
{
    [TestClass]
    public class DataManagerUnitTests
    {
        [TestMethod]
        public void TestTransitData()
        {
            var (results, timespan) = Timing.Profile(() => new TestTransitData(), 10);
            Console.WriteLine($"Creating Test Transit Data: {timespan}");
        }

        [TestMethod]
        public void TestRandomPaths()
        {
            var dataManager = new TestTransitData().DataManager;
            var raptor = new Raptor(TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(15), dataManager);

            const int count = 50000;

            var sourceList = new List<Position2d>();
            var targetList = new List<Position2d>();
            for (var i = 0; i < count; ++i)
            {
                sourceList.Add(CreateRandomPosition());
                targetList.Add(CreateRandomPosition());
            }
            
            var time = new WeekTimePoint(DayOfWeek.Wednesday, 7, 30);

            var taskList = new List<Task>();
            for (var i = 0; i < count; ++i)
            {
                var source = sourceList[i];
                var target = targetList[i];
                taskList.Add(Task.Factory.StartNew(() => raptor.Compute(source, time, target, Speed.FromKilometersPerHour(8))));
            }

            var sw = Stopwatch.StartNew();
            Task.WaitAll(taskList.ToArray());
            sw.Stop();

            Console.WriteLine($"Total: {sw.ElapsedMilliseconds}ms.");
        }

        [TestMethod]
        public void DataAnalyzing()
        {
            var dataManager = new TestTransitData().DataManager;
            var raptor = new Raptor(TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(15), dataManager);

            const int count = 50000;

            var sourceList = new List<Position2d>();
            var targetList = new List<Position2d>();
            for (var i = 0; i < count; ++i)
            {
                sourceList.Add(CreateRandomPosition());
                targetList.Add(CreateRandomPosition());
            }

            var time = new WeekTimePoint(DayOfWeek.Wednesday, 7, 30);

            var taskList = new List<Task<List<Connection>>>();
            for (var i = 0; i < count; ++i)
            {
                var source = sourceList[i];
                var target = targetList[i];
                taskList.Add(Task.Factory.StartNew(() => raptor.Compute(source, time, target, Speed.FromKilometersPerHour(8))));
            }

            Task.WaitAll(taskList.ToArray());

            var results = taskList.Select(t => t.Result).ToList();

            var avgTravelTime = GetAverageTravelTime(results);
            Console.WriteLine($"Average travel time: {avgTravelTime}.");

            var usedLines = GetUsedLines(results).OrderByDescending(x => x.Value);
            foreach (var usedLine in usedLines)
            {
                Console.WriteLine($"Line {usedLine.Key.Name}: {usedLine.Value} people.");
            }

            Console.WriteLine();

            var busiestStations = GetBusiestStations(results).OrderByDescending(x => x.Value);
            var transferStationDic = new Dictionary<TransferStation, uint>();
            foreach (var station in busiestStations)
            {
                var ts = dataManager.GetTransferStation(station.Key);
                if (!transferStationDic.ContainsKey(ts))
                {
                    transferStationDic[ts] = station.Value;
                }
                else
                {
                    transferStationDic[ts] += station.Value;
                }
            }
            foreach (var ts in transferStationDic.OrderByDescending(x => x.Value))
            {
                Console.WriteLine($"{ts.Key.Name}: {ts.Value} people.");
            }

            Console.WriteLine();

            var tripSegments = GetBusiestTripSegments(results, dataManager);
            var passengersPerTrip = tripSegments.Select(kvp => new KeyValuePair<Trip, uint>(kvp.Key, (uint)kvp.Value.Sum(x => x.Value))).OrderByDescending(kvp => kvp.Value);
            for (var i = 0; i < 10; ++i)
            {
                var (trip, passengers) = passengersPerTrip.ElementAt(i);
                var tripFrom = trip.Stations.First();
                var timeFrom = trip.DepartureAtStation(tripFrom);
                var tripTo = trip.Stations.Last();
                var timeTo = trip.ArrivalAtStation(tripTo);
                Console.WriteLine($"Trip from {dataManager.GetTransferStation(tripFrom)} at {timeFrom} to {dataManager.GetTransferStation(tripTo)} at {timeTo} has {passengers} passengers.");
            }

            Console.WriteLine();

            var passengersPerTripSegment = tripSegments.SelectMany(tripSegment => tripSegment.Value, (tripSegment, u) => (tripSegment.Key, u.Key, u.Value)).OrderByDescending(t => t.Item3);
            for (var i = 0; i < 10; ++i)
            {
                var (trip, source, passengers) = passengersPerTripSegment.ElementAt(i);
                var target = trip.GetNextStation(source);
                var departure = trip.DepartureAtStation(source);
                var arrival = trip.ArrivalAtStation(target);
                Console.WriteLine($"{dataManager.GetTransferStation(source)} ({departure}) -> {dataManager.GetTransferStation(target)} ({arrival}): {passengers} passengers.");
            }
        }

        [TestMethod]
        public void ActiveTripsTest()
        {
            var dataManager = new TestTransitData().DataManager;
            var activeTrips0 = dataManager.GetActiveTrips(new WeekTimePoint(DayOfWeek.Monday, 3));
            var activeTrips1 = dataManager.GetActiveTrips(new WeekTimePoint(DayOfWeek.Monday, 11));
            var activeTrips2 = dataManager.GetActiveTrips(new WeekTimePoint(DayOfWeek.Monday, 8));

            var activeTrips0Count = activeTrips0.Count();
            var activeTrips1Count = activeTrips1.Count();
            var activeTrips2Count = activeTrips2.Count();
            Assert.IsTrue(activeTrips0Count < activeTrips1Count, $"{activeTrips0Count} >= {activeTrips1Count}");
            Assert.IsTrue(activeTrips1Count < activeTrips2Count, $"{activeTrips1Count} >= {activeTrips2Count}");
        }

        [TestMethod]
        public void ActivePositionsTest()
        {
            var dataManager = new TestTransitData().DataManager;
            var activePositions0 = dataManager.GetActiveVehiclePositions(new WeekTimePoint(DayOfWeek.Monday, 3));
            var activePositions1 = dataManager.GetActiveVehiclePositions(new WeekTimePoint(DayOfWeek.Monday, 11));
            var activePositions2 = dataManager.GetActiveVehiclePositions(new WeekTimePoint(DayOfWeek.Monday, 8));

            var activePositions0Count = activePositions0.Count();
            var activePositions1Count = activePositions1.Count();
            var activePositions2Count = activePositions2.Count();
            Assert.IsTrue(activePositions0Count < activePositions1Count, $"{activePositions0Count} >= {activePositions1Count}");
            Assert.IsTrue(activePositions1Count < activePositions2Count, $"{activePositions1Count} >= {activePositions2Count}");
        }

        private static readonly Random Random = new Random();

        private static Position2d CreateRandomPosition()
        {
            var x = Random.NextDouble() * 10000;
            var y = Random.NextDouble() * 10000;
            return new Position2d(x, y);
        }

        private static TimeSpan GetAverageTravelTime(IReadOnlyCollection<List<Connection>> connectionsLists)
        {
            var travelTimes = connectionsLists.Aggregate(TimeSpan.Zero, (current, connectionList) => current + (connectionList.Last().TargetTime - connectionList.First().SourceTime));

            return new TimeSpan(travelTimes.Ticks / connectionsLists.Count);
        }

        private static Dictionary<Line, uint> GetUsedLines(IEnumerable<List<Connection>> connectionsLists)
        {
            var dic = new Dictionary<Line, uint>();
            foreach (var connectionsList in connectionsLists)
            {
                foreach (var connection in connectionsList)
                {
                    if (connection.Type != Connection.TypeEnum.Ride)
                    {
                        continue;
                    }

                    if (!dic.ContainsKey(connection.Line))
                    {
                        dic[connection.Line] = 1;
                    }
                    else
                    {
                        dic[connection.Line]++;
                    }
                    
                }
            }

            return dic;
        }

        private static Dictionary<Station, uint> GetBusiestStations(IEnumerable<List<Connection>> connectionsLists)
        {
            var dic = new Dictionary<Station, uint>();
            foreach (var connectionsList in connectionsLists)
            {
                foreach (var connection in connectionsList)
                {
                    if (connection.Type == Connection.TypeEnum.Ride || connection.Type == Connection.TypeEnum.Undefined || connection.Type == Connection.TypeEnum.Walk)
                    {
                        continue;
                    }

                    if (connection.Type == Connection.TypeEnum.WalkToStation)
                    {
                        var station = connection.TargetStation;
                        if (!dic.ContainsKey(station))
                        {
                            dic[station] = 1;
                        }
                        else
                        {
                            dic[station]++;
                        }
                    }
                    else if (connection.Type == Connection.TypeEnum.WalkFromStation)
                    {
                        var station = connection.SourceStation;
                        if (!dic.ContainsKey(station))
                        {
                            dic[station] = 1;
                        }
                        else
                        {
                            dic[station]++;
                        }
                    }
                    else
                    {
                        var station1 = connection.SourceStation;
                        if (!dic.ContainsKey(station1))
                        {
                            dic[station1] = 1;
                        }
                        else
                        {
                            dic[station1]++;
                        }

                        var station2 = connection.TargetStation;
                        if (!dic.ContainsKey(station2))
                        {
                            dic[station2] = 1;
                        }
                        else
                        {
                            dic[station2]++;
                        }
                    }
                }
            }

            return dic;
        }

        private static Dictionary<Trip, Dictionary<Station, uint>> GetBusiestTripSegments(IEnumerable<List<Connection>> connectionsLists, DataManager dataManager)
        {
            var tripSegmentDic = new Dictionary<Trip, Dictionary<Station, uint>>();
            foreach (var connection in connectionsLists.SelectMany(c => c))
            {
                if (connection.Type != Connection.TypeEnum.Ride)
                {
                    continue;
                }

                var source = connection.SourceStation;
                var infos = dataManager.GetInfos(source);
                var (_, trip) = infos.stationInfo.GetNextDepartureAndTripArrayBinarySearch(connection.SourceTime);
                if (!tripSegmentDic.ContainsKey(trip))
                {
                    tripSegmentDic.Add(trip, new Dictionary<Station, uint>
                    {
                        [source] = 1
                    });
                }
                else
                {
                    var dic = tripSegmentDic[trip];
                    if (!dic.ContainsKey(source))
                    {
                        dic.Add(source, 1);
                    }
                    else
                    {
                        dic[source]++;
                    }
                }
            }

            return tripSegmentDic;
        }
    }
}
