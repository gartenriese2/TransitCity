using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CitySimulation;
using Geometry;
using Statistics.Charts;
using Statistics.Data;
using Svg;
using SvgDrawing;
using SvgDrawing.Charts;
using Time;
using Transit;
using Transit.Data;
using Transit.Timetable;
using Transit.Timetable.Algorithm;
using Utility.Units;

namespace ConsoleApp1
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            //RaptorPerformanceTest2();
            //DrawBusiestStations();
            LineChartTest();
        }

        private static void LineChartTest()
        {
            const int count = 1000000;
            var jobSchedules = Enumerable.Repeat(new Random(), count).Select(JobSchedule.CreateRandom).ToList();
            var data = new RangedData(0f, 1f, 24 * 7 - 1);
            for (var i = 0; i < 24 * 7 - 1; ++i)
            {
                data.AddDatapoint(new FloatDatapoint(i + 0.5f, CountWorkers(jobSchedules, new WeekTimeSpan(new WeekTimePoint((byte)(i / 24), (byte)(i % 24)), new WeekTimePoint((byte)((i + 1) / 24), (byte)((i + 1) % 24))))));
                Console.WriteLine($"done with {i}");
            }
            var chart = new LineChart(data);
            var svgChart = new SvgLineChart(chart, 1024, 512, 12);
            svgChart.Save("workersPerHour.svg");
        }

        private static int CountWorkers(IEnumerable<JobSchedule> schedules, WeekTimeSpan targetWts)
        {
            return schedules.Count(js => js.WeekTimeSpans.Any(wts => targetWts.IsInside(wts.Begin) || wts.IsInside(targetWts.Begin)));
        }

        private static void DrawBusiestStations()
        {
            var dataManager = new TestTransitData().DataManager;
            var raptor = new RaptorWithDataManagerBinarySearchTripLookup(Speed.FromKilometersPerHour(8).MetersPerSecond, TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(15), dataManager);

            const int count = 1000000;
            var random = new Random();

            var sourceList = new List<Position2f>();
            var targetList = new List<Position2f>();
            for (var i = 0; i < count; ++i)
            {
                sourceList.Add(CreateRandomPosition());
                targetList.Add(CreateRandomPosition());
            }

            var time = new WeekTimePoint(DayOfWeek.Wednesday, 7, 30);

            var taskList = new List<Task<List<Connection<Position2f>>>>();
            for (var i = 0; i < count; ++i)
            {
                var source = sourceList[i];
                var target = targetList[i];
                taskList.Add(Task.Factory.StartNew(() => raptor.Compute(source, time, target)));
            }

            Task.WaitAll(taskList.ToArray());

            var results = taskList.Select(t => t.Result).ToList();

            var busiestStations = GetBusiestStations(results).OrderByDescending(x => x.Value);
            var transferStationDic = new Dictionary<TransferStation<Position2f>, uint>();
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

            var document = new SvgDocumentWrapper(10000, 10000);

            foreach (var station in dataManager.AllStations)
            {
                var c = new SvgCircle
                {
                    CenterX = station.Position.X,
                    CenterY = station.Position.Y,
                    Radius = 8f,
                    Fill = new SvgColourServer(Color.White),
                    Stroke = new SvgColourServer(Color.Black),
                    StrokeWidth = 2f
                };
                document.Add(c);
            }

            foreach (var lineInfo in dataManager.AllLineInfos)
            {
                Color c;
                if (lineInfo.Line.Name == "1")
                {
                    c = Color.Red;
                }
                else if (lineInfo.Line.Name == "2")
                {
                    c = Color.DarkGreen;
                }
                else if (lineInfo.Line.Name == "3")
                {
                    c = Color.DarkBlue;
                }
                else
                {
                    c = Color.Orange;
                }

                foreach (var path in lineInfo.RouteInfos.Select(ri => ri.Path))
                {
                    var polyline = new SvgPolyline
                    {
                        StrokeWidth = 4f,
                        Stroke = new SvgColourServer(c),
                        Points = new SvgPointCollection(),
                        Fill = SvgPaintServer.None
                    };
                    foreach (var pos in path)
                    {
                        polyline.Points.Add(pos.X);
                        polyline.Points.Add(pos.Y);
                    }
                    document.Add(polyline);
                }
            }

            var maxPeople = transferStationDic.Values.Max();
            var minRadius = 16f;
            var maxRadius = 64f;
            foreach (var transferStation in dataManager.AllTransferStations)
            {
                var stations = transferStation.Stations.ToList();
                var midpoint = stations[0].Position;
                for (var i = 1; i < stations.Count; ++i)
                {
                    midpoint += stations[i].Position;
                }
                midpoint = new Position2f(midpoint.X / stations.Count, midpoint.Y / stations.Count);

                var radius = minRadius;
                var people = 0u;
                if (transferStationDic.ContainsKey(transferStation))
                {
                    people = transferStationDic[transferStation];
                    var ratio = people / (float)maxPeople;
                    radius = minRadius + ratio * (maxRadius + minRadius);
                }

                var c = new SvgCircle
                {
                    CenterX = midpoint.X,
                    CenterY = midpoint.Y,
                    Radius = radius,
                    Fill = new SvgColourServer(Color.White),
                    Stroke = new SvgColourServer(Color.Black),
                    StrokeWidth = 6f
                };
                document.Add(c);

                var stationText = new SvgText(transferStation.Name)
                {
                    FontSize = new SvgUnit(64),
                    X = new SvgUnitCollection { midpoint.X },
                    Y = new SvgUnitCollection { midpoint.Y + radius + 64 }
                };
                document.Add(stationText);

                var peopleText = new SvgText(people.ToString())
                {
                    FontSize = new SvgUnit(64),
                    X = new SvgUnitCollection {midpoint.X},
                    Y = new SvgUnitCollection {midpoint.Y + radius + 128}
                };
                document.Add(peopleText);
            }

            document.Save("busiestStations.svg");

            Position2f CreateRandomPosition()
            {
                var x = random.NextDouble() * 10000;
                var y = random.NextDouble() * 10000;
                return new Position2f((float)x, (float)y);
            }

            Dictionary<Station<Position2f>, uint> GetBusiestStations(IEnumerable<List<Connection<Position2f>>> connectionsLists)
            {
                var dic = new Dictionary<Station<Position2f>, uint>();
                foreach (var connectionsList in connectionsLists)
                {
                    foreach (var connection in connectionsList)
                    {
                        if (connection.Type == Connection<Position2f>.TypeEnum.Ride || connection.Type == Connection<Position2f>.TypeEnum.Undefined || connection.Type == Connection<Position2f>.TypeEnum.Walk)
                        {
                            continue;
                        }

                        if (connection.Type == Connection<Position2f>.TypeEnum.WalkToStation)
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
                        else if (connection.Type == Connection<Position2f>.TypeEnum.WalkFromStation)
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
        }

        private static void RaptorPerformanceTest2()
        {
            var walkingSpeed = 2.2f;
            var maxWalkingTime = TimeSpan.FromMinutes(10);
            var maxWaitingTime = TimeSpan.FromMinutes(15);
            var dataManager = new TestTransitData().DataManager;
            var raptorWithDataManager = new RaptorWithDataManager(walkingSpeed, maxWalkingTime, maxWaitingTime, dataManager);
            var raptorWithDataManagerBinarySearch = new RaptorWithDataManagerBinarySearch(walkingSpeed, maxWalkingTime, maxWaitingTime, dataManager);
            var parallelRaptorWithDataManager = new ParallelRaptorWithDataManager(walkingSpeed, maxWalkingTime, maxWaitingTime, dataManager);
            var raptorWithDataManagerBinarySearchTripLookup = new RaptorWithDataManagerBinarySearchTripLookup(walkingSpeed, maxWalkingTime, maxWaitingTime, dataManager);

            var source = new Position2f(500, 500);
            var target = new Position2f(7800, 1200);
            var time = new WeekTimePoint(DayOfWeek.Tuesday, 11, 30);

            const int tasks = 200000;

            var taskList1 = new List<Task>();
            var taskList2 = new List<Task>();
            var taskList3 = new List<Task>();
            var taskList4 = new List<Task>();

            for (var i = 0; i < tasks; ++i)
            {
                taskList1.Add(Task.Factory.StartNew(() => raptorWithDataManager.Compute(source, time, target)));
            }

            var sw1 = Stopwatch.StartNew();
            Task.WaitAll(taskList1.ToArray());
            sw1.Stop();
            Console.WriteLine($"RaptorWithDataManager: {sw1.ElapsedMilliseconds}ms ({sw1.ElapsedTicks / tasks} ticks per task)");

            for (var i = 0; i < tasks; ++i)
            {
                taskList2.Add(Task.Factory.StartNew(() => raptorWithDataManagerBinarySearch.Compute(source, time, target)));
            }

            var sw2 = Stopwatch.StartNew();
            Task.WaitAll(taskList2.ToArray());
            sw2.Stop();
            Console.WriteLine($"RaptorWithDataManagerBinarySearch: {sw2.ElapsedMilliseconds}ms ({sw2.ElapsedTicks / tasks} ticks per task)");

            for (var i = 0; i < tasks; ++i)
            {
                taskList3.Add(Task.Factory.StartNew(() => parallelRaptorWithDataManager.Compute(source, time, target)));
            }

            var sw3 = Stopwatch.StartNew();
            Task.WaitAll(taskList3.ToArray());
            sw3.Stop();
            Console.WriteLine($"ParallelRaptorWithDataManager: {sw3.ElapsedMilliseconds}ms ({sw3.ElapsedTicks / tasks} ticks per task)");

            for (var i = 0; i < tasks; ++i)
            {
                taskList4.Add(Task.Factory.StartNew(() => raptorWithDataManagerBinarySearchTripLookup.Compute(source, time, target)));
            }

            var sw4 = Stopwatch.StartNew();
            Task.WaitAll(taskList4.ToArray());
            sw4.Stop();
            Console.WriteLine($"RaptorWithDataManagerBinarySearchTripLookup: {sw4.ElapsedMilliseconds}ms ({sw4.ElapsedTicks / tasks} ticks per task)");
        }
    }
}
