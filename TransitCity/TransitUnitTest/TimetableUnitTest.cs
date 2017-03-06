using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PathFinding.Network;
using Time;
using Transit;
using Transit.Timetable;

namespace TransitUnitTest
{
    [TestClass]
    public class TimetableUnitTest
    {
        [TestMethod]
        public void AddEntryTest()
        {
            var table = new Timetable<Position2f>();

            var station1A = new Station<Position2f>(new Position2f(1500, 1000));
            var station2A = new Station<Position2f>(new Position2f(2500, 1000));
            var station3A = new Station<Position2f>(new Position2f(3500, 1500));
            var station4A = new Station<Position2f>(new Position2f(4000, 2500));
            var station5A = new Station<Position2f>(new Position2f(4500, 3500));
            var station6A = new Station<Position2f>(new Position2f(5500, 4000));
            var station7A = new Station<Position2f>(new Position2f(6500, 4000));
            var station8A = new Station<Position2f>(new Position2f(7500, 4000));
            var station9A = new Station<Position2f>(new Position2f(8000, 5000));
            var station10A = new Station<Position2f>(new Position2f(8000, 6000));
            var station11A = new Station<Position2f>(new Position2f(7500, 7000));
            var station12A = new Station<Position2f>(new Position2f(7500, 8000));
            var station13A = new Station<Position2f>(new Position2f(8000, 9000));

            var station1B = new Station<Position2f>(new Position2f(1500, 1020));
            var station2B = new Station<Position2f>(new Position2f(2500, 1020));
            var station3B = new Station<Position2f>(new Position2f(3500, 1520));
            var station4B = new Station<Position2f>(new Position2f(4000, 2520));
            var station5B = new Station<Position2f>(new Position2f(4500, 3520));
            var station6B = new Station<Position2f>(new Position2f(5500, 4020));
            var station7B = new Station<Position2f>(new Position2f(6500, 4020));
            var station8B = new Station<Position2f>(new Position2f(7500, 4020));
            var station9B = new Station<Position2f>(new Position2f(8000, 5020));
            var station10B = new Station<Position2f>(new Position2f(8000, 6020));
            var station11B = new Station<Position2f>(new Position2f(7500, 7020));
            var station12B = new Station<Position2f>(new Position2f(7500, 8020));
            var station13B = new Station<Position2f>(new Position2f(8000, 9020));

            var route1A = new Route<Position2f>(new[] { station1A, station2A, station3A, station4A, station5A, station6A, station7A, station8A, station9A, station10A, station11A, station12A, station13A }, 300f);
            var route1B = new Route<Position2f>(new[] { station1B, station2B, station3B, station4B, station5B, station6B, station7B, station8B, station9B, station10B, station11B, station12B, station13B }, 300f);
            var line1 = new Line<Position2f>("1", route1A, route1B);

            var transferStation1 = new TransferStation<Position2f>("1_1", station1A, station1B);

            table.AddEntry(new WeekTimePoint(3, 11, 24, 31), new WeekTimePoint(3, 11, 25, 31), line1, route1A, transferStation1, station1A);
            table.AddEntry(new WeekTimePoint(3, 12, 25, 36), new WeekTimePoint(3, 12, 26, 36), line1, route1A, transferStation1, station1A);
        }

        [TestMethod]
        public void QueryTimepointTest()
        {
            var table = new Timetable<Position2f>();

            var station1A = new Station<Position2f>(new Position2f(1500, 1000));
            var station2A = new Station<Position2f>(new Position2f(2500, 1000));
            var station3A = new Station<Position2f>(new Position2f(3500, 1500));
            var station4A = new Station<Position2f>(new Position2f(4000, 2500));
            var station5A = new Station<Position2f>(new Position2f(4500, 3500));
            var station6A = new Station<Position2f>(new Position2f(5500, 4000));
            var station7A = new Station<Position2f>(new Position2f(6500, 4000));
            var station8A = new Station<Position2f>(new Position2f(7500, 4000));
            var station9A = new Station<Position2f>(new Position2f(8000, 5000));
            var station10A = new Station<Position2f>(new Position2f(8000, 6000));
            var station11A = new Station<Position2f>(new Position2f(7500, 7000));
            var station12A = new Station<Position2f>(new Position2f(7500, 8000));
            var station13A = new Station<Position2f>(new Position2f(8000, 9000));

            var station1B = new Station<Position2f>(new Position2f(1500, 1020));
            var station2B = new Station<Position2f>(new Position2f(2500, 1020));
            var station3B = new Station<Position2f>(new Position2f(3500, 1520));
            var station4B = new Station<Position2f>(new Position2f(4000, 2520));
            var station5B = new Station<Position2f>(new Position2f(4500, 3520));
            var station6B = new Station<Position2f>(new Position2f(5500, 4020));
            var station7B = new Station<Position2f>(new Position2f(6500, 4020));
            var station8B = new Station<Position2f>(new Position2f(7500, 4020));
            var station9B = new Station<Position2f>(new Position2f(8000, 5020));
            var station10B = new Station<Position2f>(new Position2f(8000, 6020));
            var station11B = new Station<Position2f>(new Position2f(7500, 7020));
            var station12B = new Station<Position2f>(new Position2f(7500, 8020));
            var station13B = new Station<Position2f>(new Position2f(8000, 9020));

            var route1A = new Route<Position2f>(new[] { station1A, station2A, station3A, station4A, station5A, station6A, station7A, station8A, station9A, station10A, station11A, station12A, station13A }, 300f);
            var route1B = new Route<Position2f>(new[] { station1B, station2B, station3B, station4B, station5B, station6B, station7B, station8B, station9B, station10B, station11B, station12B, station13B }, 300f);
            var line1 = new Line<Position2f>("1", route1A, route1B);

            var transferStation1 = new TransferStation<Position2f>("1_1", station1A, station1B);

            table.AddEntry(new WeekTimePoint(3, 11, 24, 31), new WeekTimePoint(3, 11, 25, 31), line1, route1A, transferStation1, station1A);
            table.AddEntry(new WeekTimePoint(3, 12, 25, 36), new WeekTimePoint(3, 12, 26, 36), line1, route1A, transferStation1, station1A);

            var query = new TimePointQuery<Position2f>(new WeekTimePoint(3, 12));

            var entries = table.Query(query);
            Assert.AreEqual(entries.Count(), 1);
        }

        [TestMethod]
        public void AddRouteTest()
        {
            var manager = new TimetableManager<Position2f>();

            var transferStationDictionary = new Dictionary<string, TransferStation<Position2f>>();

            var line1 = CreateLine1(transferStationDictionary);
            var line2 = CreateLine2(transferStationDictionary);

            var coll1A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line1.Routes.ElementAt(0).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll1B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line1.Routes.ElementAt(1).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll2A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line2.Routes.ElementAt(0).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll2B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line2.Routes.ElementAt(1).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });

            manager.AddRoute(line1, line1.Routes.ElementAt(0), coll1A, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line1, line1.Routes.ElementAt(1), coll1B, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line2, line2.Routes.ElementAt(0), coll2A, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line2, line2.Routes.ElementAt(1), coll2B, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);

            var entries = manager.GetEntriesInRange(new WeekTimePoint(DayOfWeek.Tuesday, 11, 30), new WeekTimePoint(DayOfWeek.Tuesday, 11, 40));
            var count = entries.Count();
        }

        [TestMethod]
        public void RaptorTest()
        {
            var manager = new TimetableManager<Position2f>();

            var transferStationDictionary = new Dictionary<string, TransferStation<Position2f>>();

            var line1 = CreateLine1(transferStationDictionary);
            var line2 = CreateLine2(transferStationDictionary);

            var coll1A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line1.Routes.ElementAt(0).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll1B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line1.Routes.ElementAt(1).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll2A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line2.Routes.ElementAt(0).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll2B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line2.Routes.ElementAt(1).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });

            manager.AddRoute(line1, line1.Routes.ElementAt(0), coll1A, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line1, line1.Routes.ElementAt(1), coll1B, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line2, line2.Routes.ElementAt(0), coll2A, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line2, line2.Routes.ElementAt(1), coll2B, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);

            var raptor = new Raptor<Position2f>(manager);

            raptor.Compute(transferStationDictionary.Values.ElementAt(0), new WeekTimePoint(DayOfWeek.Monday, 5), transferStationDictionary.Values.ElementAt(4), new List<TransferStation<Position2f>>(transferStationDictionary.Values));
            raptor.Compute(transferStationDictionary["Stanmore"], new WeekTimePoint(DayOfWeek.Tuesday, 11, 30), transferStationDictionary["Stockwell"], new List<TransferStation<Position2f>>(transferStationDictionary.Values));
        }

        [TestMethod]
        public void RaptorTest2()
        {
            var manager = new TimetableManager<Position2f>();

            var transferStationDictionary = new Dictionary<string, TransferStation<Position2f>>();

            var line1 = CreateLine1(transferStationDictionary);
            var line2 = CreateLine2(transferStationDictionary);

            var coll1A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line1.Routes.ElementAt(0).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll1B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line1.Routes.ElementAt(1).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll2A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line2.Routes.ElementAt(0).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll2B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line2.Routes.ElementAt(1).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });

            manager.AddRoute(line1, line1.Routes.ElementAt(0), coll1A, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line1, line1.Routes.ElementAt(1), coll1B, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line2, line2.Routes.ElementAt(0), coll2A, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line2, line2.Routes.ElementAt(1), coll2B, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);

            var raptor = new Raptor<Position2f>(manager);

            raptor.Compute(transferStationDictionary["Stanmore"].Stations.ElementAt(0), new WeekTimePoint(DayOfWeek.Tuesday, 11, 30), transferStationDictionary["Stockwell"], new List<TransferStation<Position2f>>(transferStationDictionary.Values));
        }

        [TestMethod]
        public void RaptorTest3()
        {
            var manager = new TimetableManager<Position2f>();

            var transferStationDictionary = new Dictionary<string, TransferStation<Position2f>>();

            var line1 = CreateLine1(transferStationDictionary);
            var line2 = CreateLine2(transferStationDictionary);

            var coll1A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line1.Routes.ElementAt(0).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll1B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line1.Routes.ElementAt(1).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll2A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line2.Routes.ElementAt(0).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll2B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line2.Routes.ElementAt(1).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });

            manager.AddRoute(line1, line1.Routes.ElementAt(0), coll1A, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line1, line1.Routes.ElementAt(1), coll1B, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line2, line2.Routes.ElementAt(0), coll2A, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line2, line2.Routes.ElementAt(1), coll2B, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);

            var raptor = new Raptor<Position2f>(manager);

            var arrivalTime = raptor.Compute(new Position2f(500, 500), new WeekTimePoint(DayOfWeek.Tuesday, 11, 30), transferStationDictionary["Stockwell"], new List<TransferStation<Position2f>>(transferStationDictionary.Values));
        }

        [TestMethod]
        public void RaptorTest4()
        {
            var transferStationDictionary = new Dictionary<string, TransferStation<Position2f>>();
            var raptor = new Raptor<Position2f>(CreateManager(transferStationDictionary));

            var connectionList = raptor.Compute(new Position2f(500, 500), new WeekTimePoint(DayOfWeek.Tuesday, 11, 30), new Position2f(7800, 1200), new List<TransferStation<Position2f>>(transferStationDictionary.Values));
            connectionList.ForEach(Console.WriteLine);
            Console.WriteLine($"Total travel time: {(connectionList.Last().TargetTime - connectionList.First().SourceTime).ToString(@"hh\:mm\:ss")} h");

            connectionList = raptor.Compute(new Position2f(3333, 3333), new WeekTimePoint(DayOfWeek.Tuesday, 9, 26, 37), new Position2f(6666, 1111), new List<TransferStation<Position2f>>(transferStationDictionary.Values));
            connectionList.ForEach(Console.WriteLine);
            Console.WriteLine($"Total travel time: {(connectionList.Last().TargetTime - connectionList.First().SourceTime).ToString(@"hh\:mm\:ss")} h");
        }

        [TestMethod]
        public void QueryIntoNewWeekTest()
        {
            var manager = new TimetableManager<Position2f>();

            var transferStationDictionary = new Dictionary<string, TransferStation<Position2f>>();

            var line1 = CreateLine1(transferStationDictionary);
            var line2 = CreateLine2(transferStationDictionary);

            var coll1A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line1.Routes.ElementAt(0).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll1B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line1.Routes.ElementAt(1).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll2A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line2.Routes.ElementAt(0).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll2B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line2.Routes.ElementAt(1).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });

            manager.AddRoute(line1, line1.Routes.ElementAt(0), coll1A, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line1, line1.Routes.ElementAt(1), coll1B, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line2, line2.Routes.ElementAt(0), coll2A, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line2, line2.Routes.ElementAt(1), coll2B, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);

            var departures = manager.GetDepartures(transferStationDictionary["Stanmore"], new WeekTimePoint(DayOfWeek.Friday, 23, 50), new WeekTimePoint(DayOfWeek.Monday, 5, 10));
        }

        private TimetableManager<Position2f> CreateManager(Dictionary<string, TransferStation<Position2f>> transferStationDictionary)
        {
            var manager = new TimetableManager<Position2f>();


            var line1 = CreateLine1(transferStationDictionary);
            var line2 = CreateLine2(transferStationDictionary);

            var coll1A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line1.Routes.ElementAt(0).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll1B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line1.Routes.ElementAt(1).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll2A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line2.Routes.ElementAt(0).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll2B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line2.Routes.ElementAt(1).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });

            manager.AddRoute(line1, line1.Routes.ElementAt(0), coll1A, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line1, line1.Routes.ElementAt(1), coll1B, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line2, line2.Routes.ElementAt(0), coll2A, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line2, line2.Routes.ElementAt(1), coll2B, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);

            return manager;
        }

        private Station<Position2f> CreateStation(Position2f pos, string name, Dictionary<string, TransferStation<Position2f>> tsd)
        {
            var station = new Station<Position2f>(pos);
            if (!tsd.ContainsKey(name))
            {
                tsd[name] = new TransferStation<Position2f>(name);
            }

            tsd[name].AddStation(station);
            return station;
        }

        private TimeEdgeCost SubwayTravelTimeFunc(Station<Position2f> a, Station<Position2f> b)
        {
            const float meanAcceleration = 0.6f;
            const float maximalSpeed = 70f / 3.6f; // 70 km/h
            const float timeToReachMaximalSpeed = maximalSpeed / meanAcceleration;
            const float neededDistanceToReachMaximalSpeed = meanAcceleration / 2 * timeToReachMaximalSpeed * timeToReachMaximalSpeed;
            var distance = a.Position.DistanceTo(b.Position);
            var baseTime = 30f; // waiting time at station
            if (distance < 2 * neededDistanceToReachMaximalSpeed) // distance is too small to reach maximalSpeed
            {
                baseTime += 2 * (float)Math.Sqrt(distance / meanAcceleration);
            }
            else
            {
                var remainingDistance = distance - 2 * neededDistanceToReachMaximalSpeed;
                baseTime += 2 * timeToReachMaximalSpeed + remainingDistance / maximalSpeed;
            }

            return new TimeEdgeCost(baseTime);
        }

        private Line<Position2f> CreateLine1(Dictionary<string, TransferStation<Position2f>> tsd)
        {
            var station1A = CreateStation(new Position2f(1500, 996), "Stanmore", tsd);
            var station2A = CreateStation(new Position2f(2502, 996), "Canons Park", tsd);
            var station3A = CreateStation(new Position2f(3506, 1498), "Queensbury", tsd);
            var station4A = CreateStation(new Position2f(4018, 2522), "Kingsbury", tsd);
            var station5A = CreateStation(new Position2f(4506, 3498), "Wembley Park", tsd);
            var station6A = CreateStation(new Position2f(5502, 3996), "Neasden", tsd);
            var station7A = CreateStation(new Position2f(6524, 3996), "Clapham South", tsd);
            var station8A = CreateStation(new Position2f(7505, 3996), "Dollis Hill", tsd);
            var station9A = CreateStation(new Position2f(8008, 5003), "Willesden Green", tsd);
            var station10A = CreateStation(new Position2f(8008, 6006), "Kilburn", tsd);
            var station11A = CreateStation(new Position2f(7508, 7006), "West Hampstead", tsd);
            var station12A = CreateStation(new Position2f(7508, 8003), "Finchley Road", tsd);
            var station13A = CreateStation(new Position2f(8007, 9001), "Swiss Cottage", tsd);

            var station1B = CreateStation(new Position2f(7993, 9008), "Swiss Cottage", tsd);
            var station2B = CreateStation(new Position2f(7492, 8006), "Finchley Road", tsd);
            var station3B = CreateStation(new Position2f(7492, 7003), "West Hampstead", tsd);
            var station4B = CreateStation(new Position2f(7992, 6003), "Kilburn", tsd);
            var station5B = CreateStation(new Position2f(7992, 5006), "Willesden Green", tsd);
            var station6B = CreateStation(new Position2f(7495, 4012), "Dollis Hill", tsd);
            var station7B = CreateStation(new Position2f(6524, 4012), "Clapham South", tsd);
            var station8B = CreateStation(new Position2f(5498, 4012), "Neasden", tsd);
            var station9B = CreateStation(new Position2f(4494, 3510), "Wembley Park", tsd);
            var station10B = CreateStation(new Position2f(3999, 2520), "Kingsbury", tsd);
            var station11B = CreateStation(new Position2f(3494, 1510), "Queensbury", tsd);
            var station12B = CreateStation(new Position2f(2498, 1012), "Canons Park", tsd);
            var station13B = CreateStation(new Position2f(1500, 1012), "Stanmore", tsd);

            const float frequency = 300f;
            var route1A = new Route<Position2f>(new[] { station1A, station2A, station3A, station4A, station5A, station6A, station7A, station8A, station9A, station10A, station11A, station12A, station13A }, frequency);
            var route1B = new Route<Position2f>(new[] { station1B, station2B, station3B, station4B, station5B, station6B, station7B, station8B, station9B, station10B, station11B, station12B, station13B }, frequency);
            return new Line<Position2f>("1", route1A, route1B);
        }

        private Line<Position2f> CreateLine2(Dictionary<string, TransferStation<Position2f>> tsd)
        {
            var station1A = CreateStation(new Position2f(1500, 8000), "Morden", tsd);
            var station2A = CreateStation(new Position2f(2500, 7500), "South Wimbledon", tsd);
            var station3A = CreateStation(new Position2f(3500, 7000), "Colliers Wood", tsd);
            var station4A = CreateStation(new Position2f(4500, 6500), "Tooting Broadway", tsd);
            var station5A = CreateStation(new Position2f(5500, 5500), "Tooting Bec", tsd);
            var station6A = CreateStation(new Position2f(6000, 4750), "Balham", tsd);
            var station7A = CreateStation(new Position2f(6500, 4000), "Clapham South", tsd);
            var station8A = CreateStation(new Position2f(7000, 3000), "Clapham Common", tsd);
            var station9A = CreateStation(new Position2f(7000, 2000), "Clapham North", tsd);
            var station10A = CreateStation(new Position2f(8000, 1500), "Stockwell", tsd);
            var station11A = CreateStation(new Position2f(9000, 1500), "Oval", tsd);

            var station1B = CreateStation(new Position2f(9000, 1520), "Oval", tsd);
            var station2B = CreateStation(new Position2f(8000, 1520), "Stockwell", tsd);
            var station3B = CreateStation(new Position2f(7000, 2020), "Clapham North", tsd);
            var station4B = CreateStation(new Position2f(7000, 3020), "Clapham Common", tsd);
            var station5B = CreateStation(new Position2f(6500, 4020), "Clapham South", tsd);
            var station6B = CreateStation(new Position2f(6000, 4770), "Balham", tsd);
            var station7B = CreateStation(new Position2f(5500, 5520), "Tooting Bec", tsd);
            var station8B = CreateStation(new Position2f(4500, 6520), "Tooting Broadway", tsd);
            var station9B = CreateStation(new Position2f(3500, 7020), "Colliers Wood", tsd);
            var station10B = CreateStation(new Position2f(2500, 7520), "South Wimbledon", tsd);
            var station11B = CreateStation(new Position2f(1500, 8020), "Morden", tsd);

            const float frequency = 240f;
            var route1A = new Route<Position2f>(new[] { station1A, station2A, station3A, station4A, station5A, station6A, station7A, station8A, station9A, station10A, station11A }, frequency);
            var route1B = new Route<Position2f>(new[] { station1B, station2B, station3B, station4B, station5B, station6B, station7B, station8B, station9B, station10B, station11B }, frequency);
            return new Line<Position2f>("2", route1A, route1B);
        }
    }

    internal class Connection
    {
        internal Connection(Station<Position2f> departureStation, Station<Position2f> arrivalStation, WeekTimePoint departureWeekTimePoint, WeekTimePoint arrivalWeekTimePoint)
        {
            DepartureStation = departureStation;
            ArrivalStation = arrivalStation;
            DepartureWeekTimePoint = departureWeekTimePoint;
            ArrivalWeekTimePoint = arrivalWeekTimePoint;
        }

        internal Station<Position2f> DepartureStation { get; }

        internal Station<Position2f> ArrivalStation { get; }

        internal WeekTimePoint DepartureWeekTimePoint { get; }

        internal WeekTimePoint ArrivalWeekTimePoint { get; }
    }

    internal class Timetable
    {
        internal List<Connection> Connections { get; } = new List<Connection>();

        internal void AddConnection(Connection c)
        {
            Connections.Add(c);
        }
    }

    internal class CSA
    {
        private readonly Timetable _timetable = new Timetable();
        private readonly Dictionary<Station<Position2f>, Connection> _inConnectionDictionary = new Dictionary<Station<Position2f>, Connection>();
        private readonly Dictionary<Station<Position2f>, WeekTimePoint> _earlierstArrivalDictionary = new Dictionary<Station<Position2f>, WeekTimePoint>();

        

        internal void Execute(Station<Position2f> departureStation, Station<Position2f> arrivalStation, WeekTimePoint departureWeekTimePoint)
        {
            _earlierstArrivalDictionary.Add(departureStation, departureWeekTimePoint);

            WeekTimePoint earliest = null;
            foreach (var connection in _timetable.Connections)
            {
                if (!_earlierstArrivalDictionary.ContainsKey(connection.DepartureStation))
                {
                    _earlierstArrivalDictionary.Add(connection.DepartureStation, null);
                }

                if (!_earlierstArrivalDictionary.ContainsKey(connection.ArrivalStation))
                {
                    _earlierstArrivalDictionary.Add(connection.ArrivalStation, null);
                }

                if (connection.DepartureWeekTimePoint >= _earlierstArrivalDictionary[connection.DepartureStation] && connection.ArrivalWeekTimePoint < _earlierstArrivalDictionary[connection.ArrivalStation])
                {
                    _earlierstArrivalDictionary[connection.ArrivalStation] = connection.ArrivalWeekTimePoint;
                    _inConnectionDictionary[connection.ArrivalStation] = connection;

                    if (connection.ArrivalStation == arrivalStation)
                    {
                        if (earliest == null || connection.ArrivalWeekTimePoint < earliest)
                        {
                            earliest = connection.ArrivalWeekTimePoint;
                        }
                    }
                }
                else if (connection.ArrivalWeekTimePoint > earliest)
                {
                    return;
                }
            }
        }
    }
}
