using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PathFinding.Network;
using Time;
using Transit;
using Transit.Timetable;
using Transit.Timetable.Algorithm;
using Transit.Timetable.Managers;
using Transit.Timetable.Queries;
using Utility.Timing;
using Utility.Extensions;

namespace TransitUnitTest
{
    [TestClass]
    public class TimetableUnitTest
    {
        [TestMethod]
        public void AddEntryTest()
        {
            var table = new Timetable<Position2f>();

            var station1A = new Station<Position2f>(new Position2f(1500, 1000));
            var station2A = new Station<Position2f>(new Position2f(2500, 1000));
            var station3A = new Station<Position2f>(new Position2f(3500, 1500));
            var station4A = new Station<Position2f>(new Position2f(4000, 2500));
            var station5A = new Station<Position2f>(new Position2f(4500, 3500));
            var station6A = new Station<Position2f>(new Position2f(5500, 4000));
            var station7A = new Station<Position2f>(new Position2f(6500, 4000));
            var station8A = new Station<Position2f>(new Position2f(7500, 4000));
            var station9A = new Station<Position2f>(new Position2f(8000, 5000));
            var station10A = new Station<Position2f>(new Position2f(8000, 6000));
            var station11A = new Station<Position2f>(new Position2f(7500, 7000));
            var station12A = new Station<Position2f>(new Position2f(7500, 8000));
            var station13A = new Station<Position2f>(new Position2f(8000, 9000));

            var station1B = new Station<Position2f>(new Position2f(1500, 1020));
            var station2B = new Station<Position2f>(new Position2f(2500, 1020));
            var station3B = new Station<Position2f>(new Position2f(3500, 1520));
            var station4B = new Station<Position2f>(new Position2f(4000, 2520));
            var station5B = new Station<Position2f>(new Position2f(4500, 3520));
            var station6B = new Station<Position2f>(new Position2f(5500, 4020));
            var station7B = new Station<Position2f>(new Position2f(6500, 4020));
            var station8B = new Station<Position2f>(new Position2f(7500, 4020));
            var station9B = new Station<Position2f>(new Position2f(8000, 5020));
            var station10B = new Station<Position2f>(new Position2f(8000, 6020));
            var station11B = new Station<Position2f>(new Position2f(7500, 7020));
            var station12B = new Station<Position2f>(new Position2f(7500, 8020));
            var station13B = new Station<Position2f>(new Position2f(8000, 9020));

            var route1A = new Route<Position2f>(new[] { station1A, station2A, station3A, station4A, station5A, station6A, station7A, station8A, station9A, station10A, station11A, station12A, station13A }, 300f);
            var route1B = new Route<Position2f>(new[] { station1B, station2B, station3B, station4B, station5B, station6B, station7B, station8B, station9B, station10B, station11B, station12B, station13B }, 300f);
            var line1 = new Line<Position2f>("1", route1A, route1B);

            var transferStation1 = new TransferStation<Position2f>("1_1", station1A, station1B);

            table.AddEntry(new WeekTimePoint(3, 11, 24, 31), new WeekTimePoint(3, 11, 25, 31), line1, route1A, transferStation1, station1A);
            table.AddEntry(new WeekTimePoint(3, 12, 25, 36), new WeekTimePoint(3, 12, 26, 36), line1, route1A, transferStation1, station1A);
        }

        [TestMethod]
        public void QueryTimepointTest()
        {
            var table = new Timetable<Position2f>();

            var station1A = new Station<Position2f>(new Position2f(1500, 1000));
            var station2A = new Station<Position2f>(new Position2f(2500, 1000));
            var station3A = new Station<Position2f>(new Position2f(3500, 1500));
            var station4A = new Station<Position2f>(new Position2f(4000, 2500));
            var station5A = new Station<Position2f>(new Position2f(4500, 3500));
            var station6A = new Station<Position2f>(new Position2f(5500, 4000));
            var station7A = new Station<Position2f>(new Position2f(6500, 4000));
            var station8A = new Station<Position2f>(new Position2f(7500, 4000));
            var station9A = new Station<Position2f>(new Position2f(8000, 5000));
            var station10A = new Station<Position2f>(new Position2f(8000, 6000));
            var station11A = new Station<Position2f>(new Position2f(7500, 7000));
            var station12A = new Station<Position2f>(new Position2f(7500, 8000));
            var station13A = new Station<Position2f>(new Position2f(8000, 9000));

            var station1B = new Station<Position2f>(new Position2f(1500, 1020));
            var station2B = new Station<Position2f>(new Position2f(2500, 1020));
            var station3B = new Station<Position2f>(new Position2f(3500, 1520));
            var station4B = new Station<Position2f>(new Position2f(4000, 2520));
            var station5B = new Station<Position2f>(new Position2f(4500, 3520));
            var station6B = new Station<Position2f>(new Position2f(5500, 4020));
            var station7B = new Station<Position2f>(new Position2f(6500, 4020));
            var station8B = new Station<Position2f>(new Position2f(7500, 4020));
            var station9B = new Station<Position2f>(new Position2f(8000, 5020));
            var station10B = new Station<Position2f>(new Position2f(8000, 6020));
            var station11B = new Station<Position2f>(new Position2f(7500, 7020));
            var station12B = new Station<Position2f>(new Position2f(7500, 8020));
            var station13B = new Station<Position2f>(new Position2f(8000, 9020));

            var route1A = new Route<Position2f>(new[] { station1A, station2A, station3A, station4A, station5A, station6A, station7A, station8A, station9A, station10A, station11A, station12A, station13A }, 300f);
            var route1B = new Route<Position2f>(new[] { station1B, station2B, station3B, station4B, station5B, station6B, station7B, station8B, station9B, station10B, station11B, station12B, station13B }, 300f);
            var line1 = new Line<Position2f>("1", route1A, route1B);

            var transferStation1 = new TransferStation<Position2f>("1_1", station1A, station1B);

            table.AddEntry(new WeekTimePoint(3, 11, 24, 31), new WeekTimePoint(3, 11, 25, 31), line1, route1A, transferStation1, station1A);
            table.AddEntry(new WeekTimePoint(3, 12, 25, 36), new WeekTimePoint(3, 12, 26, 36), line1, route1A, transferStation1, station1A);

            var query = new TimePointQuery<Position2f>(new WeekTimePoint(3, 12));

            var entries = table.Query(query);
            Assert.AreEqual(entries.Count(), 1);
        }

        [TestMethod]
        public void AddRouteTest()
        {
            var manager = new TimetableManager<Position2f>();

            var transferStationDictionary = new Dictionary<string, TransferStation<Position2f>>();

            var line1 = CreateLine1(transferStationDictionary);
            var line2 = CreateLine2(transferStationDictionary);

            var coll1A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line1.Routes.ElementAt(0).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll1B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line1.Routes.ElementAt(1).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll2A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line2.Routes.ElementAt(0).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var coll2B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line2.Routes.ElementAt(1).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });

            manager.AddRoute(line1, line1.Routes.ElementAt(0), coll1A, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line1, line1.Routes.ElementAt(1), coll1B, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line2, line2.Routes.ElementAt(0), coll2A, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line2, line2.Routes.ElementAt(1), coll2B, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);

            var entries = manager.GetEntriesInRange(new WeekTimePoint(DayOfWeek.Tuesday, 11, 30), new WeekTimePoint(DayOfWeek.Tuesday, 11, 40));
            var count = entries.Count();
        }

        [TestMethod]
        public void AddRouteLinkedTest()
        {
            var tsd = new Dictionary<string, TransferStation<Position2f>>();
            var manager = CreateManager<DictionaryTimetableManager<Position2f>, LinkedEntry<Position2f>>(tsd);
            var entries = manager.GetDepartures(tsd["Stanmore"].Stations.ElementAt(0), new WeekTimePoint(DayOfWeek.Tuesday, 11, 30), new WeekTimePoint(DayOfWeek.Tuesday, 11, 40));
            var count = entries.Count();
        }

        [TestMethod]
        public void RaptorPerformanceTest()
        {
            var tsd1 = new Dictionary<string, TransferStation<Position2f>>();
            var tsd2 = new Dictionary<string, TransferStation<Position2f>>();
            var tsd3 = new Dictionary<string, TransferStation<Position2f>>();

            ITimetableManager<Position2f, Entry<Position2f>> manager1 = CreateManager<TimetableManager<Position2f>, Entry<Position2f>>(tsd1);
            ITimetableManager<Position2f, LinkedEntry<Position2f>> manager2 = CreateManager<LinkedTimetableManager<Position2f>, LinkedEntry<Position2f>>(tsd2);
            ITimetableManager<Position2f, LinkedEntry<Position2f>> manager3 = CreateManager<DictionaryTimetableManager<Position2f>, LinkedEntry<Position2f>>(tsd3);

            var raptor1 = new Raptor<Position2f>(manager1);
            var raptor2 = new ParallelRaptor<Position2f>(manager1, 2.2f);
            var raptor3 = new LinkedParallelRaptor<Position2f>(manager2, 2.2f);
            var raptor4 = new LinkedParallelRaptor<Position2f>(manager3, 2.2f);
            var raptor5 = new LinkedRaptor<Position2f>(manager2);
            var raptor6 = new LinkedRaptor<Position2f>(manager3);

            var source = new Position2f(500, 500);
            var target = new Position2f(7800, 1200);
            var time = new WeekTimePoint(DayOfWeek.Tuesday, 11, 30);

            const uint iterations = 5u;

            var tuple1 = Timing.Profile(() => raptor1.Compute(source, time, target, tsd1.Values), iterations);
            Console.WriteLine($"Raptor with TimetableManager: {tuple1.timespan}");
            var connectionList1 = tuple1.results[0];
            connectionList1.ForEach(Console.WriteLine);
            Console.WriteLine($"Total travel time: {(connectionList1.Last().TargetTime - connectionList1.First().SourceTime).ToString(@"hh\:mm\:ss")} h");
            Console.WriteLine();

            var tuple2 = Timing.Profile(() => raptor2.Compute(source, time, target, tsd1.Values), iterations);
            Console.WriteLine($"ParallelRaptor with TimetableManager: {tuple2.timespan}");
            var connectionList2 = tuple2.results[0];
            connectionList2.ForEach(Console.WriteLine);
            Console.WriteLine($"Total travel time: {(connectionList2.Last().TargetTime - connectionList2.First().SourceTime).ToString(@"hh\:mm\:ss")} h");
            Console.WriteLine();

            var tuple3 = Timing.Profile(() => raptor3.Compute(source, time, target, tsd2.Values), iterations);
            Console.WriteLine($"LinkedParallelRaptor with LinkedTimetableManager: {tuple3.timespan}");
            var connectionList3 = tuple3.results[0];
            connectionList3.ForEach(Console.WriteLine);
            Console.WriteLine($"Total travel time: {(connectionList3.Last().TargetTime - connectionList3.First().SourceTime).ToString(@"hh\:mm\:ss")} h");
            Console.WriteLine();

            var tuple4 = Timing.Profile(() => raptor4.Compute(source, time, target, tsd3.Values), iterations);
            Console.WriteLine($"LinkedParallelRaptor with DictionaryTimetableManager: {tuple4.timespan}");
            var connectionList4 = tuple4.results[0];
            connectionList4.ForEach(Console.WriteLine);
            Console.WriteLine($"Total travel time: {(connectionList4.Last().TargetTime - connectionList4.First().SourceTime).ToString(@"hh\:mm\:ss")} h");
            Console.WriteLine();

            var tuple5 = Timing.Profile(() => raptor5.Compute(source, time, target, tsd2.Values), iterations);
            Console.WriteLine($"LinkedRaptor with LinkedTimetableManager: {tuple5.timespan}");
            var connectionList5 = tuple5.results[0];
            connectionList5.ForEach(Console.WriteLine);
            Console.WriteLine($"Total travel time: {(connectionList5.Last().TargetTime - connectionList5.First().SourceTime).ToString(@"hh\:mm\:ss")} h");
            Console.WriteLine();

            var tuple6 = Timing.Profile(() => raptor6.Compute(source, time, target, tsd3.Values), iterations);
            Console.WriteLine($"LinkedRaptor with DictionaryTimetableManager: {tuple6.timespan}");
            var connectionList6 = tuple6.results[0];
            connectionList6.ForEach(Console.WriteLine);
            Console.WriteLine($"Total travel time: {(connectionList6.Last().TargetTime - connectionList6.First().SourceTime).ToString(@"hh\:mm\:ss")} h");
            Console.WriteLine();
        }

        [TestMethod]
        public void QueryIntoNewWeekTest()
        {
            var tsd = new Dictionary<string, TransferStation<Position2f>>();
            var manager = CreateManager<DictionaryTimetableManager<Position2f>, LinkedEntry<Position2f>>(tsd);
            var departures = manager.GetDepartures(tsd["Balham"], new WeekTimePoint(DayOfWeek.Saturday, 23, 50), new WeekTimePoint(DayOfWeek.Monday, 5, 5)).OrderBy(entry => entry.WeekTimePoint);
        }

        private TManager CreateManager<TManager, TEntry>(Dictionary<string, TransferStation<Position2f>> transferStationDictionary) where TManager : ITimetableManager<Position2f, TEntry>, new()
        {
            var manager = new TManager();

            var line1 = CreateLine1(transferStationDictionary);
            var line2 = CreateLine2(transferStationDictionary);
            var line3 = CreateLine3(transferStationDictionary);
            var line4 = CreateLine4(transferStationDictionary);

            var coll1A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line1.Routes.ElementAt(0).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday });
            var coll1B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line1.Routes.ElementAt(1).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday });
            var coll2A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line2.Routes.ElementAt(0).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday });
            var coll2B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line2.Routes.ElementAt(1).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday });
            var coll3A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line3.Routes.ElementAt(0).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday });
            var coll3B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line3.Routes.ElementAt(1).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday });
            var coll4A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line4.Routes.ElementAt(0).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday });
            var coll4B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(line4.Routes.ElementAt(1).Frequency), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday });

            manager.AddRoute(line1, line1.Routes.ElementAt(0), coll1A, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line1, line1.Routes.ElementAt(1), coll1B, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line2, line2.Routes.ElementAt(0), coll2A, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line2, line2.Routes.ElementAt(1), coll2B, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line3, line3.Routes.ElementAt(0), coll3A, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line3, line3.Routes.ElementAt(1), coll3B, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line4, line4.Routes.ElementAt(0), coll4A, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line4, line4.Routes.ElementAt(1), coll4B, new List<TransferStation<Position2f>>(transferStationDictionary.Values), SubwayTravelTimeFunc);

            return manager;
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

        private static TimeEdgeCost SubwayTravelTimeFunc(Station<Position2f> a, Station<Position2f> b)
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

        private static Line<Position2f> CreateLine1(IDictionary<string, TransferStation<Position2f>> tsd)
        {
            var (rA, rB) = CreateRoutes(new Dictionary<Position2f, string>
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
            }, 180f, tsd);

            return new Line<Position2f>("1", rA, rB);
        }

        private Line<Position2f> CreateLine2(IDictionary<string, TransferStation<Position2f>> tsd)
        {
            var (rA, rB) = CreateRoutes(new Dictionary<Position2f, string>
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

            return new Line<Position2f>("2", rA, rB);
        }

        private static Line<Position2f> CreateLine3(IDictionary<string, TransferStation<Position2f>> tsd)
        {
            var (rA, rB) = CreateRoutes(new Dictionary<Position2f, string>
            {
                [new Position2f(5500, 2000)] = "Chesham",
                [new Position2f(5500, 3000)] = "Amersham",
                [new Position2f(5500, 4000)] = "Neasden",
                [new Position2f(6000, 4750)] = "Balham",
                [new Position2f(6750, 5750)] = "Chalfont",
                [new Position2f(8000, 6000)] = "Kilburn",
                [new Position2f(9000, 6250)] = "Watford",
                [new Position2f(9900, 6300)] = "Croxley"
            }, 120f, tsd);

            return new Line<Position2f>("3", rA, rB);
        }

        private static Line<Position2f> CreateLine4(IDictionary<string, TransferStation<Position2f>> tsd)
        {
            var (rA, rB) = CreateRoutes(new Dictionary<Position2f, string>
            {
                [new Position2f(500, 5500)] = "Epping",
                [new Position2f(1500, 5000)] = "Theydon Bois",
                [new Position2f(2500, 4500)] = "Debden",
                [new Position2f(3500, 4500)] = "Loughton",
                [new Position2f(4750, 5000)] = "Buckhurst Hill",
                [new Position2f(5900, 4900)] = "Balham",
                [new Position2f(7000, 4500)] = "Woodford",
                [new Position2f(7500, 4100)] = "Dollis Hill",
                [new Position2f(8500, 3500)] = "Roding Valley",
                [new Position2f(9600, 3600)] = "Chigwell"
            }, 180f, tsd);

            return new Line<Position2f>("4", rA, rB);
        }

        private static Tuple<Route<Position2f>, Route<Position2f>> CreateRoutes(IDictionary<Position2f, string> dic, float frequency, IDictionary<string, TransferStation<Position2f>> tsd)
        {
            var positions = dic.Keys.ToList();
            var spline = GetSpline(positions, 4);

            var routeAList = new List<Station<Position2f>>();
            var routeBList = new List<Station<Position2f>>();
            foreach (var b in positions)
            {
                var idx = spline.FindIndex(p => p == b);
                var a = idx == 0 ? b : spline[idx - 1];
                var c = idx == spline.Count - 1 ? b : spline[idx + 1];
                var(p1, p2) = GetOffsetPoints(a, b, c, 4f);

                var stationA = CreateStation(p1, dic[b], tsd);
                var stationB = CreateStation(p2, dic[b], tsd);
                routeAList.Add(stationA);
                routeBList.Add(stationB);
            }

            var routeA = new Route<Position2f>(routeAList, frequency);
            routeBList.Reverse();
            var routeB = new Route<Position2f>(routeBList, frequency);

            return new Tuple<Route<Position2f>, Route<Position2f>>(routeA, routeB);
        }

        private static Tuple<Position2f, Position2f> GetOffsetPoints(Position2f a, Position2f b, Position2f c, float offset)
        {
            var vec = c - a;
            var vecRight = vec.RotateRight().Normalize() * offset;
            var vecLeft = vec.RotateLeft().Normalize() * offset;
            return new Tuple<Position2f, Position2f>(b + vecRight, b + vecLeft);

        }

        private static List<Position2f> GetSpline(List<Position2f> controlPoints, int subsegments)
        {
            if (controlPoints.Count < 2)
            {
                return null;
            }

            if (controlPoints.Count == 2)
            {
                return controlPoints;
            }

            var pts = new List<Position2f>
            {
                controlPoints.First()
            };

            var dt = 1f / subsegments;
            for (var i = 1; i < subsegments; ++i)
            {
                pts.Add(PointOnCurve(controlPoints[0], controlPoints[0], controlPoints[1], controlPoints[2], dt * i));
            }

            for (var idx = 1; idx < controlPoints.Count - 2; ++idx)
            {
                pts.Add(controlPoints[idx]);
                for (var i = 1; i < subsegments; ++i)
                {
                    pts.Add(PointOnCurve(controlPoints[idx - 1], controlPoints[idx], controlPoints[idx + 1], controlPoints[idx + 2], dt * i));
                }
            }

            pts.Add(controlPoints[controlPoints.Count - 2]);
            for (var i = 1; i < subsegments; ++i)
            {
                pts.Add(PointOnCurve(controlPoints[controlPoints.Count - 3], controlPoints[controlPoints.Count - 2], controlPoints[controlPoints.Count - 1], controlPoints[controlPoints.Count - 1], dt * i));
            }

            pts.Add(controlPoints.Last());

            return pts;
        }

        private static Position2f PointOnCurve(Position2f p0, Position2f p1, Position2f p2, Position2f p3, float t)
        {
            var t2 = t * t;
            var t3 = t2 * t;

            var x = 0.5f * ((2.0f * p1.X) +
            (-p0.X + p2.X) * t +
            (2.0f * p0.X - 5.0f * p1.X + 4 * p2.X - p3.X) * t2 +
            (-p0.X + 3.0f * p1.X - 3.0f * p2.X + p3.X) * t3);

            var y = 0.5f * ((2.0f * p1.Y) +
            (-p0.Y + p2.Y) * t +
            (2.0f * p0.Y - 5.0f * p1.Y + 4 * p2.Y - p3.Y) * t2 +
            (-p0.Y + 3.0f * p1.Y - 3.0f * p2.Y + p3.Y) * t3);

            return new Position2f(x, y);
        }
    }

    internal class Connection
    {
        internal Connection(Station<Position2f> departureStation, Station<Position2f> arrivalStation, WeekTimePoint departureWeekTimePoint, WeekTimePoint arrivalWeekTimePoint)
        {
            DepartureStation = departureStation;
            ArrivalStation = arrivalStation;
            DepartureWeekTimePoint = departureWeekTimePoint;
            ArrivalWeekTimePoint = arrivalWeekTimePoint;
        }

        internal Station<Position2f> DepartureStation { get; }

        internal Station<Position2f> ArrivalStation { get; }

        internal WeekTimePoint DepartureWeekTimePoint { get; }

        internal WeekTimePoint ArrivalWeekTimePoint { get; }
    }

    internal class Timetable
    {
        internal List<Connection> Connections { get; } = new List<Connection>();

        internal void AddConnection(Connection c)
        {
            Connections.Add(c);
        }
    }

    internal class CSA
    {
        private readonly Timetable _timetable = new Timetable();
        private readonly Dictionary<Station<Position2f>, Connection> _inConnectionDictionary = new Dictionary<Station<Position2f>, Connection>();
        private readonly Dictionary<Station<Position2f>, WeekTimePoint> _earlierstArrivalDictionary = new Dictionary<Station<Position2f>, WeekTimePoint>();

        

        internal void Execute(Station<Position2f> departureStation, Station<Position2f> arrivalStation, WeekTimePoint departureWeekTimePoint)
        {
            _earlierstArrivalDictionary.Add(departureStation, departureWeekTimePoint);

            WeekTimePoint earliest = null;
            foreach (var connection in _timetable.Connections)
            {
                if (!_earlierstArrivalDictionary.ContainsKey(connection.DepartureStation))
                {
                    _earlierstArrivalDictionary.Add(connection.DepartureStation, null);
                }

                if (!_earlierstArrivalDictionary.ContainsKey(connection.ArrivalStation))
                {
                    _earlierstArrivalDictionary.Add(connection.ArrivalStation, null);
                }

                if (connection.DepartureWeekTimePoint >= _earlierstArrivalDictionary[connection.DepartureStation] && connection.ArrivalWeekTimePoint < _earlierstArrivalDictionary[connection.ArrivalStation])
                {
                    _earlierstArrivalDictionary[connection.ArrivalStation] = connection.ArrivalWeekTimePoint;
                    _inConnectionDictionary[connection.ArrivalStation] = connection;

                    if (connection.ArrivalStation == arrivalStation)
                    {
                        if (earliest == null || connection.ArrivalWeekTimePoint < earliest)
                        {
                            earliest = connection.ArrivalWeekTimePoint;
                        }
                    }
                }
                else if (connection.ArrivalWeekTimePoint > earliest)
                {
                    return;
                }
            }
        }
    }
}
