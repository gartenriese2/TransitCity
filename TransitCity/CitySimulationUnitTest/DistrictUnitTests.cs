using System;
using CitySimulation;
using Geometry;
using Geometry.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CitySimulationUnitTest
{
    [TestClass]
    public class DistrictUnitTests
    {
        [TestMethod]
        public void CreateDistrictTest()
        {
            var shape = new Circle(new Position2f(5000f, 5000f), 961f);
            var disctrict = new RandomDistrict("City", shape, 7000, 300000);
            Console.WriteLine($"Populationsdensity: {disctrict.PopulationDensity} ppl/km²");
            Console.WriteLine($"Jobsdensity: {disctrict.JobDensity} jobs/km²");
            Console.WriteLine($"Area: {disctrict.Area.SquareKilometers}km²");
        }
    }
}
