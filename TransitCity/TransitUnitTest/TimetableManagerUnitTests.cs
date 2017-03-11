using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PathFinding.Network;
using Time;
using Transit;
using Transit.Timetable;
using Transit.Timetable.Managers;
using Utility.Timing;

namespace TransitUnitTest
{
    [TestClass]
    public class TimetableManagerUnitTests
    {
        [TestMethod]
        public void TestGetDeparturesTimings()
        {
            var tsd1 = new Dictionary<string, TransferStation<Position2f>>();
            var tsd2 = new Dictionary<string, TransferStation<Position2f>>();
            var tsd3 = new Dictionary<string, TransferStation<Position2f>>();

            ITimetableManager<Position2f, Entry<Position2f>> manager1 = CreateManager<TimetableManager<Position2f>, Entry<Position2f>>(tsd1);
            ITimetableManager<Position2f, LinkedEntry<Position2f>> manager2 = CreateManager<LinkedTimetableManager<Position2f>, LinkedEntry<Position2f>>(tsd2);
            ITimetableManager<Position2f, LinkedEntry<Position2f>> manager3 = CreateManager<DictionaryTimetableManager<Position2f>, LinkedEntry<Position2f>>(tsd3);

            const uint iterations = 1000u;

            var tmp = 0;
            var tuple1 = Timing.Profile(() => manager1.GetDepartures(tsd1.Values.ElementAt(0).Stations.ElementAt(0), new WeekTimePoint(DayOfWeek.Monday, 6), new WeekTimePoint(DayOfWeek.Monday, 7)).ToList(), iterations);
            foreach (var result in tuple1.results)
            {
                tmp += result.Count;
            }
            Console.WriteLine($"TimetableManager.GetDepartures: {tuple1.timespan}");

            var tuple2 = Timing.Profile(() => manager2.GetDepartures(tsd2.Values.ElementAt(0).Stations.ElementAt(0), new WeekTimePoint(DayOfWeek.Monday, 6), new WeekTimePoint(DayOfWeek.Monday, 7)).ToList(), iterations);
            foreach (var result in tuple2.results)
            {
                tmp += result.Count;
            }
            Console.WriteLine($"LinkedTimetableManager.GetDepartures: {tuple2.timespan}");

            var tuple3 = Timing.Profile(() => manager3.GetDepartures(tsd3.Values.ElementAt(0).Stations.ElementAt(0), new WeekTimePoint(DayOfWeek.Monday, 6), new WeekTimePoint(DayOfWeek.Monday, 7)).ToList(), iterations);
            foreach (var result in tuple3.results)
            {
                tmp += result.Count;
            }
            Console.WriteLine($"DictionaryTimetableManager.GetDepartures: {tuple3.timespan}");
            Console.WriteLine(tmp);
        }

        [TestMethod]
        public void TestGetNextEntriesTimings()
        {
            var tsd1 = new Dictionary<string, TransferStation<Position2f>>();
            var tsd2 = new Dictionary<string, TransferStation<Position2f>>();
            var tsd3 = new Dictionary<string, TransferStation<Position2f>>();

            ITimetableManager<Position2f, Entry<Position2f>> manager1 = CreateManager<TimetableManager<Position2f>, Entry<Position2f>>(tsd1);
            ITimetableManager<Position2f, LinkedEntry<Position2f>> manager2 = CreateManager<LinkedTimetableManager<Position2f>, LinkedEntry<Position2f>>(tsd2);
            ITimetableManager<Position2f, LinkedEntry<Position2f>> manager3 = CreateManager<DictionaryTimetableManager<Position2f>, LinkedEntry<Position2f>>(tsd3);

            const uint iterations = 100u;

            var entry1 = manager1.GetDepartures(tsd1.Values.ElementAt(0).Stations.ElementAt(0), new WeekTimePoint(DayOfWeek.Monday, 5), new WeekTimePoint(DayOfWeek.Monday, 5, 10)).ElementAt(0);
            var tuple1 = Timing.Profile(() => manager1.GetNextEntries(entry1).ToList(), iterations);
            Console.WriteLine($"TimetableManager.GetNextEntries: {tuple1.timespan}");
            var entry2 = manager2.GetDepartures(tsd2.Values.ElementAt(0).Stations.ElementAt(0), new WeekTimePoint(DayOfWeek.Monday, 5), new WeekTimePoint(DayOfWeek.Monday, 5, 10)).ElementAt(0);
            var tuple2 = Timing.Profile(() => manager2.GetNextEntries(entry2).ToList(), iterations);
            Console.WriteLine($"LinkedTimetableManager.GetNextEntries: {tuple2.timespan}");
            var entry3 = manager3.GetDepartures(tsd3.Values.ElementAt(0).Stations.ElementAt(0), new WeekTimePoint(DayOfWeek.Monday, 5), new WeekTimePoint(DayOfWeek.Monday, 5, 10)).ElementAt(0);
            var tuple3 = Timing.Profile(() => manager3.GetNextEntries(entry3).ToList(), iterations);
            Console.WriteLine($"DictionaryTimetableManager.GetNextEntries: {tuple3.timespan}");
        }

        private TManager CreateManager<TManager, TEntry>(Dictionary<string, TransferStation<Position2f>> transferStationDictionary) where TManager : ITimetableManager<Position2f, TEntry>, new()
        {
            var manager = new TManager();

            var line1 = CreateLine1(transferStationDictionary);
            var line2 = CreateLine2(transferStationDictionary);
            var line3 = CreateLine3(transferStationDictionary);

            var coll1A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line1.Routes.ElementAt(0).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll1B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line1.Routes.ElementAt(1).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll2A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line2.Routes.ElementAt(0).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll2B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line2.Routes.ElementAt(1).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll3A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line3.Routes.ElementAt(0).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll3B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line3.Routes.ElementAt(1).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });

            manager.AddRoute(line1, line1.Routes.ElementAt(0), coll1A, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line1, line1.Routes.ElementAt(1), coll1B, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line2, line2.Routes.ElementAt(0), coll2A, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line2, line2.Routes.ElementAt(1), coll2B, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line3, line3.Routes.ElementAt(0), coll3A, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line3, line3.Routes.ElementAt(1), coll3B, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);

            return manager;
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

        private Line<Position2f> CreateLine1(IDictionary<string, TransferStation<Position2f>> tsd)
        {
            var (t1, t2) = CreateRoutes(new Dictionary<Position2f, string>
            {
                [new Position2f(1500, 1000)] = "Stanmore",
                [new Position2f(2500, 1000)] = "Canons Park",
                [new Position2f(3500, 1500)] = "Queensbury",
                [new Position2f(4000, 2500)] = "Kingsbury",
                [new Position2f(4500, 3500)] = "Wembley Park",
                [new Position2f(5500, 4000)] = "Neasden",
                [new Position2f(6500, 4000)] = "Clapham South",
                [new Position2f(7500, 4000)] = "Dollis Hill",
                [new Position2f(8000, 5000)] = "Willesden Green",
                [new Position2f(8000, 6000)] = "Kilburn",
                [new Position2f(7500, 7000)] = "West Hampstead",
                [new Position2f(7500, 8000)] = "Finchley Road",
                [new Position2f(8000, 9000)] = "Swiss Cottage",
            }, 240f, tsd);

            return new Line<Position2f>("1", t1.Item1, t2.Item1);
        }

        private Line<Position2f> CreateLine2(IDictionary<string, TransferStation<Position2f>> tsd)
        {
            var (t1, t2) = CreateRoutes(new Dictionary<Position2f, string>
            {
                [new Position2f(1500, 8000)] = "Morden",
                [new Position2f(2500, 7500)] = "South Wimbledon",
                [new Position2f(3500, 7000)] = "Colliers Wood",
                [new Position2f(4500, 6500)] = "Tooting Broadway",
                [new Position2f(5500, 5500)] = "Tooting Bec",
                [new Position2f(6000, 4750)] = "Balham",
                [new Position2f(6500, 4000)] = "Clapham South",
                [new Position2f(7000, 3000)] = "Clapham Common",
                [new Position2f(7000, 2000)] = "Clapham North",
                [new Position2f(8000, 1500)] = "Stockwell",
                [new Position2f(9000, 1500)] = "Oval",
            }, 240f, tsd);

            return new Line<Position2f>("2", t1.Item1, t2.Item1);
        }

        private Line<Position2f> CreateLine3(IDictionary<string, TransferStation<Position2f>> tsd)
        {
            var (t1, t2) = CreateRoutes(new Dictionary<Position2f, string>
            {
                [new Position2f(5500, 2000)] = "Chesham",
                [new Position2f(5500, 3000)] = "Amersham",
                [new Position2f(5500, 4000)] = "Neasden",
                [new Position2f(6000, 4750)] = "Balham",
                [new Position2f(6750, 5750)] = "Chalfont",
                [new Position2f(8000, 6000)] = "Latimer",
                [new Position2f(9000, 6250)] = "Watford",
                [new Position2f(9900, 6300)] = "Croxley"
            }, 120f, tsd);

            return new Line<Position2f>("3", t1.Item1, t2.Item1);
        }

        private static Station<Position2f> CreateStation(Position2f pos, string name, IDictionary<string, TransferStation<Position2f>> tsd)
        {
            var station = new Station<Position2f>(pos);
            if (!tsd.ContainsKey(name))
            {
                tsd[name] = new TransferStation<Position2f>(name);
            }

            tsd[name].AddStation(station);
            return station;
        }

        private Tuple<Tuple<Route<Position2f>, List<Position2f>>, Tuple<Route<Position2f>, List<Position2f>>> CreateRoutes(Dictionary<Position2f, string> dic, float frequency, IDictionary<string, TransferStation<Position2f>> tsd)
        {
            var positions = dic.Keys.ToList();
            var ccrSpline = new CentripetalCatmullRomSpline(positions);
            var spline = ccrSpline.CalculateSegments(10);

            var routeAList = new List<Station<Position2f>>();
            var routeBList = new List<Station<Position2f>>();
            foreach (var b in positions)
            {
                var idx = spline.FindIndex(p => p == b);
                var a = idx == 0 ? b : spline[idx - 1];
                var c = idx == spline.Count - 1 ? b : spline[idx + 1];
                var (p1, p2) = GetOffsetPoints(a, b, c, 4f);

                var stationA = CreateStation(p1, dic[b], tsd);
                var stationB = CreateStation(p2, dic[b], tsd);
                routeAList.Add(stationA);
                routeBList.Add(stationB);
            }

            var routeA = new Route<Position2f>(routeAList, frequency);
            routeBList.Reverse();
            var routeB = new Route<Position2f>(routeBList, frequency);

            var offsetSplines = GetOffsetSplines(spline, 4f);

            return new Tuple<Tuple<Route<Position2f>, List<Position2f>>, Tuple<Route<Position2f>, List<Position2f>>>(new Tuple<Route<Position2f>, List<Position2f>>(routeA, offsetSplines.Item1), new Tuple<Route<Position2f>, List<Position2f>>(routeB, offsetSplines.Item2));
        }

        private static Tuple<Position2f, Position2f> GetOffsetPoints(Position2f a, Position2f b, Position2f c, float offset)
        {
            var vec = c - a;
            var vecRight = vec.RotateRight().Normalize() * offset;
            var vecLeft = vec.RotateLeft().Normalize() * offset;
            return new Tuple<Position2f, Position2f>(b + vecRight, b + vecLeft);

        }

        private static Tuple<List<Position2f>, List<Position2f>> GetOffsetSplines(IReadOnlyList<Position2f> positions, float offset)
        {
            var r1 = new List<Position2f>();
            var r2 = new List<Position2f>();
            for (var i = 0; i < positions.Count; ++i)
            {
                var b = positions[i];
                var a = i == 0 ? b : positions[i - 1];
                var c = i == positions.Count - 1 ? b : positions[i + 1];
                var (p1, p2) = GetOffsetPoints(a, b, c, offset);
                r1.Add(p1);
                r2.Add(p2);
            }

            r2.Reverse();
            return new Tuple<List<Position2f>, List<Position2f>>(r1, r2);
        }

    }
}
