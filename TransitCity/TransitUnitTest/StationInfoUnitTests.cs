using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Time;
using Transit;
using Transit.Data;
using Utility.Timing;

namespace TransitUnitTest
{
    [TestClass]
    public class StationInfoUnitTests
    {
        [TestMethod]
        public void ProfileBinarySearch()
        {
            var stationMock = new Mock<Station<Position2f>>(new Position2f(0, 0));
            var arrivalsMock = new Mock<WeekTimeCollection>();
            var tripMock = new Mock<Trip<Position2f>>(new List<(Station<Position2f>, (WeekTimePoint, WeekTimePoint))>());

            var departures = new WeekTimeCollection(new TimeSpan(0, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(10), new [] {DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday});
            var tripList = Enumerable.Repeat(tripMock.Object, departures.Count).ToList();

            var stationInfo = new StationInfo(stationMock.Object, arrivalsMock.Object, departures, tripList);

            var wtp = new WeekTimePoint(DayOfWeek.Tuesday, 11, 31, 27);
            var expectedWtp = new WeekTimePoint(DayOfWeek.Tuesday, 11, 31, 30);
            const uint iterations = 100000u;

            var (results, timespan) = Timing.Profile(() => stationInfo.GetNextDeparture(wtp), iterations);
            Assert.AreEqual(results[0], expectedWtp);
            Console.WriteLine($"GetNextDeparture: {timespan}");

            var (results2, timespan2) = Timing.Profile(() => stationInfo.GetNextDepartureArrayBinarySearch(wtp), iterations);
            Assert.AreEqual(results2[0], expectedWtp);
            Console.WriteLine($"GetNextDepartureArrayBinarySearch: {timespan2}");
        }

        [TestMethod]
        public void ProfileBinarySearchAndTrip()
        {
            var dataManager = new TestTransitData().DataManager;
            var stationInfo = dataManager.AllStationInfos.ElementAt(0);
            var wtp = new WeekTimePoint(DayOfWeek.Tuesday, 11, 31, 27);
            const uint iterations = 100000u;

            var (_, timespan) = Timing.Profile(WithoutTrip, iterations);
            Console.WriteLine($"WithoutTrip: {timespan}");

            var (_, timespan2) = Timing.Profile(WithTrip, iterations);
            Console.WriteLine($"WithTrip: {timespan2}");

            List<WeekTimePoint> WithoutTrip()
            {
                var nextDeparture = stationInfo.GetNextDepartureArrayBinarySearch(wtp);
                var (_, routeInfo, _) = dataManager.GetInfos(stationInfo.Station);
                return routeInfo.GetNextArrivalsOnTrip(stationInfo.Station, nextDeparture).ToList();
            }

            List<WeekTimePoint> WithTrip()
            {
                var (_, trip) = stationInfo.GetNextDepartureAndTripArrayBinarySearch(wtp);
                return trip.GetNextArrivals(stationInfo.Station).ToList();
            }
        }
    }
}
