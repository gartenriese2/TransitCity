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
            var district1 = new RandomDistrict("District1", new Circle(new Position2f(1000, 1000), 1000), 5000, 2000);
            var district2 = new RandomDistrict("District2", new Circle(new Position2f(3000, 1000), 1000), 6000, 1000);
            var district3 = new RandomDistrict("District3", new Circle(new Position2f(5000, 1000), 1000), 5000, 9000);
            var district4 = new RandomDistrict("District4", new Circle(new Position2f(7000, 1000), 1000), 8000, 4000);
            var district5 = new RandomDistrict("District5", new Circle(new Position2f(9000, 1000), 1000), 4000, 1500);

            var city = new City(new List<IDistrict>{district1, district2, district3, district4, district5});

            Console.WriteLine($"Populationdensity: {city.PopulationDensity}");
            Console.WriteLine($"Jobdensity: {city.JobDensity}");
        }

        [TestMethod]
        public void DrawCityTest()
        {
            var district1 = new RandomDistrict("District1", new Circle(new Position2f(1000, 1000), 1000), 5000, 2000);
            var district2 = new RandomDistrict("District2", new Circle(new Position2f(3000, 1000), 1000), 6000, 1000);
            var district3 = new RandomDistrict("District3", new Circle(new Position2f(5000, 1000), 1000), 5000, 9000);
            var district4 = new RandomDistrict("District4", new Circle(new Position2f(7000, 1000), 1000), 8000, 4000);
            var district5 = new RandomDistrict("District5", new Circle(new Position2f(9000, 1000), 1000), 4000, 1500);

            var city = new City(new List<IDistrict> { district1, district2, district3, district4, district5 });

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
            var raptor = new RaptorWithDataManagerBinarySearchTripLookup(Speed.FromKilometersPerHour(5).MetersPerSecond, TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(15), dataManager);
            var time = new WeekTimePoint(DayOfWeek.Wednesday, 7, 30);
            var taskList = new List<Task<List<Connection<Position2f>>>>();
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

        private void AddNetworkToSvgDocument(DataManager dataManager, SvgDocumentWrapper document)
        {
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

            return new City(new List<IDistrict> { balham, buckhurst, epping, morden, neasden });
        }
    }
}
