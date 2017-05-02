using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CitySimulation;
using Geometry;
using Geometry.Shapes;
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

namespace TestApp
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            //RaptorPerformanceTest2();
            //DrawBusiestStations();
            //LineChartTest();
            //TransitWithSchedules();
            DrawActiveVehicles();
        }

        private static void DrawActiveVehicles()
        {
            var dataManager = new TestTransitData().DataManager;
            var numDocuments = 10;
            var numSeconds = 20;
            for (var i = 0; i < numDocuments; ++i)
            {
                var activeVehicles = dataManager.GetActiveVehiclePositions(new WeekTimePoint(DayOfWeek.Monday, 8) + TimeSpan.FromSeconds(numSeconds * i));
                var document = new SvgDocumentWrapper(10000, 10000);
                document.Add(new SvgRectangle
                {
                    Width = 10000,
                    Height = 10000,
                    Fill = new SvgColourServer(Color.White)
                });

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

                foreach (var activeVehicle in activeVehicles)
                {
                    var c = new SvgCircle
                    {
                        CenterX = activeVehicle.X,
                        CenterY = activeVehicle.Y,
                        Radius = 64f,
                        Fill = new SvgColourServer(Color.Gray),
                        Stroke = new SvgColourServer(Color.Black),
                        StrokeWidth = 4f
                    };
                    document.Add(c);
                }

                var numWithLeadingZeroes = i < 10 ? $"0{i}" : $"{i}";
                document.Save($"activeVehicles{numWithLeadingZeroes}.svg");
            }            
        }

        private static void TransitWithSchedules()
        {
            var city = CreateCity();
            var rnd = new Random();
            var workerScheduleTuples = city.Residents.Where(r => r.HasJob).Select(r => (r, JobSchedule.CreateRandom(rnd))).ToList();
            var dataManager = new TestTransitData().DataManager;
            var raptor = new RaptorWithDataManagerBinarySearchTripLookup(Speed.FromKilometersPerHour(5).MetersPerSecond, TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(15), dataManager);

            var workerConnectionsDictionary = new Dictionary<Resident, List<List<Connection<Position2f>>>>();
            foreach (var (worker, schedule) in workerScheduleTuples)
            {
                var workerTaskList = new List<Task<List<Connection<Position2f>>>>();
                foreach (var scheduleWts in schedule.WeekTimeSpans)
                {
                    workerTaskList.Add(Task.Factory.StartNew(() => raptor.ComputeReverse(worker.Position, scheduleWts.Begin, worker.Job.Position)));
                    workerTaskList.Add(Task.Factory.StartNew(() => raptor.Compute(worker.Job.Position, scheduleWts.End, worker.Position)));
                }

                Task.WaitAll(workerTaskList.ToArray());
                workerConnectionsDictionary.Add(worker, workerTaskList.Select(t => t.Result).ToList());
            }

            var info = new TransitConnectionInfo(workerConnectionsDictionary);
            Console.WriteLine($"{info.GetPercentagOfConnectionsWithTransit()}% of connections are made with transit.");
            Console.WriteLine($"{info.GetPercentageOfWorkersUsingTransitAtLeastOnce()}% of workers use transit at least once.");
            Console.WriteLine($"{info.GetPercentageOfWorkersUsingTransitOnly()}% of workers use transit only.");
        }

        private static City CreateCity()
        {
            var balham = new RandomDistrict("Balham", new Polygon(
                5000, 4500,
                5250, 4550,
                5500, 4625,
                5750, 4675,
                6000, 4700,
                6250, 4610,
                6500, 4500,
                6750, 4340,
                7000, 4150,
                7000, 4500,
                6500, 5500,
                5500, 6000,
                5000, 5500
            ), 4500, 16000);

            var buckhurst = new RandomDistrict("Buckhurst", new Polygon(
                2500, 5400,
                2750, 5450,
                3000, 5410,
                3250, 5350,
                3500, 5260,
                3750, 5125,
                4000, 5000,
                4250, 4850,
                4500, 4670,
                4750, 4570,
                5000, 4500,
                5000, 5500,
                5500, 6000,
                4500, 6500,
                2500, 6500
            ), 8000, 3000);

            var epping = new RandomDistrict("Epping", new Polygon(
                0, 3600,
                250, 3620,
                500, 3660,
                750, 3750,
                1000, 4000,
                1850, 5000,
                2000, 5120,
                2250, 5270,
                2500, 5400,
                2500, 6500,
                1000, 7000,
                0, 8000
            ), 6000, 2000);

            var morden = new RandomDistrict("Morden", new Polygon(
                0, 10000,
                0, 8000,
                1000, 7000,
                2500, 6500,
                3000, 6500,
                3000, 8000,
                4500, 10000
            ), 10000, 5000);

            var neasden = new RandomDistrict("Neasden", new Polygon(
                4500, 3500,
                5000, 3000,
                6000, 2500,
                6000, 4270,
                5750, 4300,
                5500, 4260,
                5250, 4150,
                5000, 4120,
                4750, 4160,
                4500, 4250
            ), 5000, 14000);

            var clapham = new RandomDistrict("Clapham", new Polygon(
                6000, 4270,
                6000, 1500,
                7000, 1000,
                7500, 1500,
                7500, 2740,
                7250, 3010,
                7000, 3400,
                6750, 3840,
                6500, 4050,
                6250, 4170
            ), 12000, 6000);

            var stockwell = new RandomDistrict("Stockwell", new Polygon(
                7500, 1500,
                7000, 1000,
                7500, 0,
                10000, 0,
                10000, 2300,
                9750, 2200,
                9500, 2100,
                9250, 2060,
                9000, 2040,
                8750, 2080,
                8500, 2150,
                8250, 2250,
                8000, 2390,
                7750, 2550,
                7500, 2740
            ), 2000, 5000);

            var debden = new RandomDistrict("Debden", new Polygon(
                0, 3000,
                5000, 3000,
                4500, 3500,
                4500, 4250,
                3500, 4750,
                3000, 4950,
                2750, 5000,
                2500, 4950,
                2250, 4800,
                2000, 4630,
                1500, 4000,
                1250, 3700,
                1000, 3490,
                750, 3350,
                500, 3270,
                0, 3270
            ), 8000, 3000);

            var amersham = new RandomDistrict("Amersham", new Polygon(
                3000, 3000,
                4000, 1500,
                6000, 1500,
                6000, 2500,
                5000, 3000
            ), 7500, 4000);

            var chesham = new RandomDistrict("Chesham", new Polygon(
                4000, 1500,
                4000, 0,
                7500, 0,
                7000, 1000,
                6000, 1500
            ), 3000, 2000);

            var stanmore = new RandomDistrict("Stanmore", new Polygon(
                0, 3000,
                0, 0,
                4000, 0,
                4000, 1500,
                3000, 3000
            ), 4500, 1500);

            var tooting = new RandomDistrict("Tooting", new Polygon(
                3000, 6500,
                4500, 6500,
                6500, 5500,
                7500, 7500,
                8000, 10000,
                4500, 10000,
                3000, 8000
            ), 3000, 500);

            var hampstead = new RandomDistrict("Hampstead", new Polygon(
                7000, 6500,
                10000, 6500,
                10000, 10000,
                8000, 10000,
                7500, 7500
            ), 4000, 1000);

            var watfrord = new RandomDistrict("Watford", new Polygon(
                7000, 4500,
                10000, 4500,
                10000, 6500,
                7000, 6500,
                6500, 5500
            ), 8000, 4000);

            var chigwell = new RandomDistrict("Chigwell", new Polygon(
                7000, 4500,
                7000, 4150,
                7150, 4000,
                7500, 3500,
                7750, 3200,
                8000, 3000,
                8500, 2720,
                8750, 2640,
                9000, 2600,
                9500, 2660,
                9750, 2750,
                10000, 2900,
                10000, 4500
            ), 7000, 2500);

            return new City("London", new List<IDistrict> { balham, buckhurst, epping, morden, neasden, clapham, stockwell, debden, amersham, chesham, stanmore, tooting, hampstead, watfrord, chigwell });
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
