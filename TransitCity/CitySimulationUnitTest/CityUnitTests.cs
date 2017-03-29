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
                var circle = shape.ToSvg();
                circle.FillOpacity = 0.2f;
                circle.Fill = new SvgColourServer(Color.Green);
                document.Add(circle);
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
            var district1 = new RandomDistrict("District1", new Circle(new Position2f(1000, 1000), 1000), 5000, 2000);
            var district2 = new RandomDistrict("District2", new Circle(new Position2f(3000, 1000), 1000), 6000, 1000);
            var district3 = new RandomDistrict("District3", new Circle(new Position2f(5000, 1000), 1000), 5000, 9000);
            var district4 = new RandomDistrict("District4", new Circle(new Position2f(7000, 1000), 1000), 8000, 4000);
            var district5 = new RandomDistrict("District5", new Circle(new Position2f(9000, 1000), 1000), 4000, 1500);
            var city = new RandomDistrict("City", new Circle(new Position2f(6000, 5000), 1500), 7000, 15000);
            var tri = new RandomDistrict("Tri", new Triangle(new Position2f(3000, 4000), new Position2f(4000, 4000), new Position2f(3500, 3500)), 4000, 2000);

            return new City(new List<IDistrict> { district1, district2, district3, district4, district5, city, tri });
        }
    }
}
