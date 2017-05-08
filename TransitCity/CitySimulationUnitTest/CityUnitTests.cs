using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CitySimulation;
using Geometry;
using Geometry.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Svg;
using SvgDrawing;
using Time;
using Transit.Data;
using Transit.Timetable;
using Transit.Timetable.Algorithm;
using Utility.Units;

namespace CitySimulationUnitTest
{
    [TestClass]
    public class CityUnitTests
    {
        [TestMethod]
        public void CreateCityTest()
        {
            var district1 = new RandomDistrict("District1", new Circle(new Position2d(1000, 1000), 1000), 5000, 2000);
            var district2 = new RandomDistrict("District2", new Circle(new Position2d(3000, 1000), 1000), 6000, 1000);
            var district3 = new RandomDistrict("District3", new Circle(new Position2d(5000, 1000), 1000), 5000, 9000);
            var district4 = new RandomDistrict("District4", new Circle(new Position2d(7000, 1000), 1000), 8000, 4000);
            var district5 = new RandomDistrict("District5", new Circle(new Position2d(9000, 1000), 1000), 4000, 1500);

            var city = new City("City", new List<IDistrict>{district1, district2, district3, district4, district5});

            Console.WriteLine($"Populationdensity: {city.PopulationDensity}");
            Console.WriteLine($"Jobdensity: {city.JobDensity}");
        }

        [TestMethod]
        public void DrawCityTest()
        {
            var district1 = new RandomDistrict("District1", new Circle(new Position2d(1000, 1000), 1000), 5000, 2000);
            var district2 = new RandomDistrict("District2", new Circle(new Position2d(3000, 1000), 1000), 6000, 1000);
            var district3 = new RandomDistrict("District3", new Circle(new Position2d(5000, 1000), 1000), 5000, 9000);
            var district4 = new RandomDistrict("District4", new Circle(new Position2d(7000, 1000), 1000), 8000, 4000);
            var district5 = new RandomDistrict("District5", new Circle(new Position2d(9000, 1000), 1000), 4000, 1500);

            var city = new City("City", new List<IDistrict> { district1, district2, district3, district4, district5 });

            var lowerLeft = new Position2f(0,0);
            var upperRight = new Position2f(10000, 10000);
            Assert.IsTrue(city.Districts.Select(d => d.Shape.Bounds).All(b => b.Item1.X >= lowerLeft.X && b.Item1.Y >= lowerLeft.Y && b.Item2.X <= upperRight.X && b.Item2.Y <= upperRight.Y));

            var svgDoc = new SvgDocumentWrapper((int) (upperRight.X - lowerLeft.X), (int) (upperRight.Y - lowerLeft.Y));
            foreach (var shape in city.Districts.Select(d => d.Shape))
            {
                var circle = shape.ToSvg();
                circle.FillOpacity = 0.5f;
                circle.Fill = new SvgColourServer(Color.Green);
                svgDoc.Add(circle);
            }

            svgDoc.Save("city.svg");
        }

        [TestMethod]
        public void CityTransitTest()
        {
            var city = CreateCity();
            var dataManager = new TestTransitData().DataManager;
            var raptor = new RaptorWithDataManagerBinarySearchTripLookup(Speed.FromKilometersPerHour(5), TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(15), dataManager);
            var time = new WeekTimePoint(DayOfWeek.Wednesday, 7, 30);
            var taskList = new List<Task<List<Connection>>>();
            foreach (var resident in city.Residents.Where(r => r.HasJob))
            {
                taskList.Add(Task.Factory.StartNew(() => raptor.Compute(resident.Position, time, resident.Job.Position)));
            }
            Task.WaitAll(taskList.ToArray());
            var results = taskList.Select(t => t.Result).ToList();

            var percentageTransit = (float)results.Count(r => r.Count > 1) / results.Count * 100f;
            Console.WriteLine($"{percentageTransit}% of workers use transit.");

            var document = new SvgDocumentWrapper(10000, 10000);
            foreach (var shape in city.Districts.Select(d => d.Shape))
            {
                var svgVisualElement = shape.ToSvg();
                svgVisualElement.FillOpacity = 0.2f;
                svgVisualElement.Fill = new SvgColourServer(Color.Green);
                svgVisualElement.StrokeWidth = 8;
                svgVisualElement.Stroke = new SvgColourServer(Color.Gray);
                document.Add(svgVisualElement);
            }

            AddNetworkToSvgDocument(dataManager, document);

            document.Save("linesAndDistricts.svg");
        }

        [TestMethod]
        public void CityDensityTest()
        {
            var city = CreateCity();
            Console.WriteLine("Population density");
            Console.WriteLine($"{city.Name}: {city.PopulationDensity}");
            foreach (var district in city.Districts.OrderByDescending(d => d.PopulationDensity))
            {
                Console.WriteLine($"District {district.Name}: {district.PopulationDensity}");
            }
            Console.WriteLine();
            Console.WriteLine("Job density");
            Console.WriteLine($"{city.Name}: {city.JobDensity}");
            foreach (var district in city.Districts.OrderByDescending(d => d.JobDensity))
            {
                Console.WriteLine($"District {district.Name}: {district.JobDensity}");
            }

            var maxDensity = city.Districts.Max(d => d.PopulationDensity);
            var maxDensity2 = city.Districts.Max(d => d.JobDensity);

            var document1 = new SvgDocumentWrapper(10000, 10000);
            var document2 = new SvgDocumentWrapper(10000, 10000);
            foreach (var district in city.Districts)
            {
                var density = district.PopulationDensity;
                var density2 = district.JobDensity;
                var shape = district.Shape;
                var svgVisualElement = shape.ToSvg();
                svgVisualElement.FillOpacity = (float) (density / maxDensity);
                svgVisualElement.Fill = new SvgColourServer(Color.Green);
                svgVisualElement.StrokeWidth = 16;
                svgVisualElement.Stroke = new SvgColourServer(Color.Black);
                document1.Add(svgVisualElement);
                var svgVisualElement2 = shape.ToSvg();
                svgVisualElement2.FillOpacity = (float) (density2 / maxDensity2);
                svgVisualElement2.Fill = new SvgColourServer(Color.Blue);
                svgVisualElement2.StrokeWidth = 16;
                svgVisualElement2.Stroke = new SvgColourServer(Color.Black);
                document2.Add(svgVisualElement2);

                var centroid = shape.Centroid;
                var textElem = new SvgText(district.Name)
                {
                    FontSize = new SvgUnit(128),
                    X = new SvgUnitCollection {(SvgUnit) centroid.X},
                    Y = new SvgUnitCollection {(SvgUnit) centroid.Y}
                };
                document1.Add(textElem);
                document2.Add(textElem);
            }

            document1.Save("populationDensity.svg");
            document2.Save("jobDensity.svg");
        }

        private void AddNetworkToSvgDocument(DataManager dataManager, SvgDocumentWrapper document)
        {
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
        }

        private City CreateCity()
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
    }
}
