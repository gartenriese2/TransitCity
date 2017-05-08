using System;
using System.Linq;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Time;
using Transit.Data;
using Transit.Timetable.Algorithm;
using Utility.Units;

namespace TransitUnitTest
{
    [TestClass]
    public class RaptorUnitTests
    {
        [TestMethod]
        public void TestReverse()
        {
            var dataManager = new TestTransitData().DataManager;
            var raptor = new RaptorWithDataManagerBinarySearchTripLookup(Speed.FromKilometersPerHour(8), TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(15), dataManager);

            var source = new Position2d(1000, 1000);
            var departure = new WeekTimePoint(DayOfWeek.Wednesday, 7, 30);
            var target = new Position2d(8000, 2000);
            var connectionList = raptor.Compute(source, departure, target);
            var arrival = connectionList.Last().TargetTime;
            var reverseConnectionList = raptor.ComputeReverse(source, arrival, target);
            Console.WriteLine($"Departure at {reverseConnectionList[0].SourceTime} instead of {departure}");
            Assert.IsTrue(reverseConnectionList[0].SourceTime >= departure);
        }
    }
}
