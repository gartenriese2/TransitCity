using System;
using System.Collections.Generic;
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
            //DrawActiveVehicles();
            TestCoordinateSystems();
        }

        private static void TestCoordinateSystems()
        {
            var worldSize = new Size(10000, 10000);
            var renderSize = new Size(800, 600);
            var centerPoint = new Point(5000, 5000);
            var zoom = 1.0;
            //TestCases(worldSize, renderSize, zoom, centerPoint, new List<(Point, Point)>
            //    {
            //        (new Point(0, 0), new Point(100, 0)),
            //        (new Point(5000, 5000), new Point(400, 300)),
            //        (new Point(10000, 10000), new Point(700, 600)),
            //        (new Point(0, 10000), new Point(100, 600)),
            //        (new Point(10000, 0), new Point(700, 0))
            //    }
            //);
            centerPoint = new Point(0, 0);
            //TestCases(worldSize, renderSize, zoom, centerPoint, new List<(Point, Point)>
            //    {
            //        (new Point(0, 0), new Point(400, 300)),
            //        (new Point(5000, 5000), new Point(700, 600)),
            //        (new Point(10000, 10000), new Point(1000, 900)),
            //        (new Point(0, 10000), new Point(400, 900)),
            //        (new Point(10000, 0), new Point(1000, 300))
            //    }
            //);
            //zoom = 2.0;
            //TestCases(worldSize, renderSize, zoom, centerPoint, new List<(Point, Point)>
            //    {
            //        (new Point(0, 0), new Point(400, 300)),
            //        (new Point(5000, 5000), new Point(1000, 900)),
            //        (new Point(10000, 10000), new Point(1600, 1500)),
            //        (new Point(0, 10000), new Point(400, 1500)),
            //        (new Point(10000, 0), new Point(1600, 300))
            //    }
            //);
            zoom = 0.5;
            //TestCases(worldSize, renderSize, zoom, centerPoint, new List<(Point, Point)>
            //    {
            //        (new Point(0, 0), new Point(400, 300)),
            //        (new Point(5000, 5000), new Point(550, 450)),
            //        (new Point(10000, 10000), new Point(700, 600)),
            //        (new Point(0, 10000), new Point(400, 600)),
            //        (new Point(10000, 0), new Point(700, 300))
            //    }
            //);
            centerPoint = new Point(5000, 5000);
            TestCases(worldSize, renderSize, zoom, centerPoint, new List<(Point, Point)>
                {
                    (new Point(0, 0), new Point(250, 150)),
                    (new Point(5000, 5000), new Point(400, 300)),
                    (new Point(10000, 10000), new Point(550, 450)),
                    (new Point(0, 10000), new Point(250, 450)),
                    (new Point(10000, 0), new Point(550, 150))
                }
            );
            zoom = 2.0;
            TestCases(worldSize, renderSize, zoom, centerPoint, new List<(Point, Point)>
                {
                    (new Point(0, 0), new Point(-200, -300)),
                    (new Point(5000, 5000), new Point(400, 300)),
                    (new Point(10000, 10000), new Point(1000, 900)),
                    (new Point(0, 10000), new Point(-200, 900)),
                    (new Point(10000, 0), new Point(1000, -300))
                }
            );
        }

        private static void TestCases(Size worldSize, Size renderSize, double zoom, Point centerPoint, IEnumerable<(Point, Point)> pointsList)
        {
            Console.WriteLine("------------------------------------------------------------------------");
            Console.WriteLine($"WorldSize: ({worldSize.Width}|{worldSize.Height})\tRenderSize: ({renderSize.Width}|{renderSize.Height})\tZoom: {zoom:F}\tCenterpoint: ({centerPoint.X}|{centerPoint.Y})");
            foreach (var (worldPoint, expectedRenderPoint) in pointsList)
            {
                var renderPoint = ModelToView(worldSize, renderSize, zoom, centerPoint, worldPoint);
                Console.WriteLine($"WorldPoint: ({worldPoint.X}|{worldPoint.Y})\tExpectedRenderPoint: ({expectedRenderPoint.X}|{expectedRenderPoint.Y})\tActualRenderPoint: ({renderPoint.X}|{renderPoint.Y})");
            }
            Console.WriteLine("------------------------------------------------------------------------");
        }

        private static Point ModelToView(Size worldSize, Size renderSize, double zoom, Point centerPoint, Point worldPoint)
        {
            var worldWidth = (double) worldSize.Width;
            var worldHeight = (double) worldSize.Height;
            var renderWidth = (double) renderSize.Width * zoom;
            var renderHeight = (double) renderSize.Height * zoom;
            var worldRatio = worldWidth / worldHeight;
            var renderRatio = renderWidth / renderHeight;
            var fitWidth = worldRatio >= renderRatio;
            var worldToRenderConversion = fitWidth ? renderWidth / worldWidth : renderHeight / worldHeight;
            var translateX = fitWidth ? 0 : (renderWidth - worldWidth * worldToRenderConversion) / 2;
            var translateY = fitWidth ? (renderHeight - worldHeight * worldToRenderConversion) / 2 : 0;

            var centerTranslateX = (worldWidth / 2 - centerPoint.X) * worldToRenderConversion;
            var centerTranslateY = (worldHeight / 2 - centerPoint.Y) * worldToRenderConversion;

            var x = (int) (worldPoint.X * worldToRenderConversion + translateX + centerTranslateX);
            var y = (int) (worldPoint.Y * worldToRenderConversion + translateY + centerTranslateY);
            return new Point(x, y);
        }

        private static void DrawActiveVehicles()
        {
            var dataManager = new TestTransitData().DataManager;
            var numDocuments = 10;
            var numSeconds = 10;
            for (var i = 0; i < numDocuments; ++i)
            {
                var activeVehicles = dataManager.GetActiveVehiclePositionsAndDirections(new WeekTimePoint(DayOfWeek.Monday, 8) + TimeSpan.FromSeconds(numSeconds * i));
                var document = new SvgDocumentWrapper(10000, 10000);
                document.Add(new SvgRectangle
                {
                    Width = 10000,
                    Height = 10000,
                    Fill = new SvgColourServer(Color.White)
                });

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
                            polyline.Points.Add((SvgUnit) pos.X);
                            polyline.Points.Add((SvgUnit) pos.Y);
                        }
                        document.Add(polyline);
                    }
                }

                foreach (var station in dataManager.AllStations)
                {
                    var c = new SvgCircle
                    {
                        CenterX = (SvgUnit) station.Position.X,
                        CenterY = (SvgUnit) station.Position.Y,
                        Radius = 8f,
                        Fill = new SvgColourServer(Color.White),
                        Stroke = new SvgColourServer(Color.Black),
                        StrokeWidth = 2f
                    };
                    document.Add(c);
                }

                foreach (var (_, pos, vec) in activeVehicles)
                {
                    var tip = pos + vec.Normalize() * 32f;
                    var right = pos - vec.Normalize() * 32f + vec.RotateRight().Normalize() * 16f;
                    var left = pos - vec.Normalize() * 32f + vec.RotateLeft().Normalize() * 16f;
                    var t = new SvgPolygon
                    {
                        Points = new SvgPointCollection { (SvgUnit) tip.X, (SvgUnit) tip.Y, (SvgUnit) right.X, (SvgUnit) right.Y, (SvgUnit) left.X, (SvgUnit) left.Y },
                        Fill = new SvgColourServer(Color.Gray),
                        Stroke = new SvgColourServer(Color.Black),
                        StrokeWidth = 4f
                    };
                    document.Add(t);
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
            var raptor = new Raptor(TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(15), dataManager);

            var workerConnectionsDictionary = new Dictionary<Resident, List<List<Connection>>>();
            foreach (var (worker, schedule) in workerScheduleTuples)
            {
                var workerTaskList = new List<Task<List<Connection>>>();
                foreach (var scheduleWts in schedule.WeekTimeSpans)
                {
                    workerTaskList.Add(Task.Factory.StartNew(() => raptor.ComputeReverse(worker.Position, scheduleWts.Begin, worker.Job.Position, Speed.FromKilometersPerHour(8))));
                    workerTaskList.Add(Task.Factory.StartNew(() => raptor.Compute(worker.Job.Position, scheduleWts.End, worker.Position, Speed.FromKilometersPerHour(8))));
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
            var raptor = new Raptor(TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(15), dataManager);

            const int count = 1000000;
            var random = new Random();

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

            var document = new SvgDocumentWrapper(10000, 10000);

            foreach (var station in dataManager.AllStations)
            {
                var c = new SvgCircle
                {
                    CenterX = (SvgUnit) station.Position.X,
                    CenterY = (SvgUnit) station.Position.Y,
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
                        polyline.Points.Add((SvgUnit) pos.X);
                        polyline.Points.Add((SvgUnit) pos.Y);
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
                midpoint = new Position2d(midpoint.X / stations.Count, midpoint.Y / stations.Count);

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
                    CenterX = (SvgUnit) midpoint.X,
                    CenterY = (SvgUnit) midpoint.Y,
                    Radius = radius,
                    Fill = new SvgColourServer(Color.White),
                    Stroke = new SvgColourServer(Color.Black),
                    StrokeWidth = 6f
                };
                document.Add(c);

                var stationText = new SvgText(transferStation.Name)
                {
                    FontSize = new SvgUnit(64),
                    X = new SvgUnitCollection { (SvgUnit) midpoint.X },
                    Y = new SvgUnitCollection { (SvgUnit) (midpoint.Y + radius + 64) }
                };
                document.Add(stationText);

                var peopleText = new SvgText(people.ToString())
                {
                    FontSize = new SvgUnit(64),
                    X = new SvgUnitCollection {(SvgUnit) midpoint.X},
                    Y = new SvgUnitCollection {(SvgUnit) (midpoint.Y + radius + 128) }
                };
                document.Add(peopleText);
            }

            document.Save("busiestStations.svg");

            Position2d CreateRandomPosition()
            {
                var x = random.NextDouble() * 10000;
                var y = random.NextDouble() * 10000;
                return new Position2d(x, y);
            }

            Dictionary<Station, uint> GetBusiestStations(IEnumerable<List<Connection>> connectionsLists)
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
        }
    }
}
