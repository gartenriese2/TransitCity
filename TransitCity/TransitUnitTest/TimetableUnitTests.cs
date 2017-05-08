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

namespace TransitUnitTest
{
    [TestClass]
    public class TimetableUnitTests
    {
        [TestMethod]
        public void AddRouteLinkedTest()
        {
            var tsd = new Dictionary<string, TransferStation>();
            var manager = CreateManager<DictionaryTimetableManager, LinkedEntry>(tsd);
            var entries = manager.GetDepartures(tsd["Stanmore"].Stations.ElementAt(0), new WeekTimePoint(DayOfWeek.Tuesday, 11, 30), new WeekTimePoint(DayOfWeek.Tuesday, 11, 40));
            var count = entries.Count();
        }

        [TestMethod]
        public void QueryIntoNewWeekTest()
        {
            var tsd = new Dictionary<string, TransferStation>();
            var manager = CreateManager<DictionaryTimetableManager, LinkedEntry>(tsd);
            var departures = manager.GetDepartures(tsd["Balham"], new WeekTimePoint(DayOfWeek.Saturday, 23, 50), new WeekTimePoint(DayOfWeek.Monday, 5, 5)).OrderBy(entry => entry.WeekTimePoint);
        }

        private TManager CreateManager<TManager, TEntry>(Dictionary<string, TransferStation> transferStationDictionary) where TManager : ITimetableManager<TEntry>, new()
        {
            var manager = new TManager();

            var line1 = CreateLine1(transferStationDictionary);
            var line2 = CreateLine2(transferStationDictionary);
            var line3 = CreateLine3(transferStationDictionary);
            var line4 = CreateLine4(transferStationDictionary);

            var coll1A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(240f), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday });
            var coll1B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(240f), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday });
            var coll2A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(180f), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday });
            var coll2B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(180f), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday });
            var coll3A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(240f), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday });
            var coll3B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(240f), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday });
            var coll4A = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(180f), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday });
            var coll4B = new WeekTimeCollection(new TimeSpan(5, 0, 0), new TimeSpan(23, 59, 59), TimeSpan.FromSeconds(180f), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday });

            manager.AddRoute(line1, line1.Routes.ElementAt(0), coll1A, new List<TransferStation>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line1, line1.Routes.ElementAt(1), coll1B, new List<TransferStation>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line2, line2.Routes.ElementAt(0), coll2A, new List<TransferStation>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line2, line2.Routes.ElementAt(1), coll2B, new List<TransferStation>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line3, line3.Routes.ElementAt(0), coll3A, new List<TransferStation>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line3, line3.Routes.ElementAt(1), coll3B, new List<TransferStation>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line4, line4.Routes.ElementAt(0), coll4A, new List<TransferStation>(transferStationDictionary.Values), SubwayTravelTimeFunc);
            manager.AddRoute(line4, line4.Routes.ElementAt(1), coll4B, new List<TransferStation>(transferStationDictionary.Values), SubwayTravelTimeFunc);

            return manager;
        }

        private static Station CreateStation(Position2d pos, string name, IDictionary<string, TransferStation> tsd)
        {
            var station = new Station(pos);
            if (!tsd.ContainsKey(name))
            {
                tsd[name] = new TransferStation(name);
            }

            tsd[name].AddStation(station);
            return station;
        }

        private static TimeEdgeCost SubwayTravelTimeFunc(Station a, Station b)
        {
            const float meanAcceleration = 0.6f;
            const float maximalSpeed = 70f / 3.6f; // 70 km/h
            const float timeToReachMaximalSpeed = maximalSpeed / meanAcceleration;
            const float neededDistanceToReachMaximalSpeed = meanAcceleration / 2 * timeToReachMaximalSpeed * timeToReachMaximalSpeed;
            var distance = a.Position.DistanceTo(b.Position);
            var baseTime = 30.0; // waiting time at station
            if (distance < 2 * neededDistanceToReachMaximalSpeed) // distance is too small to reach maximalSpeed
            {
                baseTime += 2 * (float)Math.Sqrt(distance / meanAcceleration);
            }
            else
            {
                var remainingDistance = distance - 2 * neededDistanceToReachMaximalSpeed;
                baseTime += 2 * timeToReachMaximalSpeed + remainingDistance / maximalSpeed;
            }

            return new TimeEdgeCost((float) baseTime);
        }

        private static Line CreateLine1(IDictionary<string, TransferStation> tsd)
        {
            var (rA, rB) = CreateRoutes(new Dictionary<Position2d, string>
            {
                [new Position2d(1500, 1000)] = "Stanmore",
                [new Position2d(2500, 1000)] = "Canons Park",
                [new Position2d(3500, 1500)] = "Queensbury",
                [new Position2d(4000, 2500)] = "Kingsbury",
                [new Position2d(4500, 3500)] = "Wembley Park",
                [new Position2d(5500, 4000)] = "Neasden",
                [new Position2d(6500, 4000)] = "Clapham South",
                [new Position2d(7500, 4000)] = "Dollis Hill",
                [new Position2d(8000, 5000)] = "Willesden Green",
                [new Position2d(8000, 6000)] = "Kilburn",
                [new Position2d(7500, 7000)] = "West Hampstead",
                [new Position2d(7500, 8000)] = "Finchley Road",
                [new Position2d(8000, 9000)] = "Swiss Cottage",
            }, tsd);

            return new Line("1", rA, rB);
        }

        private Line CreateLine2(IDictionary<string, TransferStation> tsd)
        {
            var (rA, rB) = CreateRoutes(new Dictionary<Position2d, string>
            {
                [new Position2d(1500, 8000)] = "Morden",
                [new Position2d(2500, 7500)] = "South Wimbledon",
                [new Position2d(3500, 7000)] = "Colliers Wood",
                [new Position2d(4500, 6500)] = "Tooting Broadway",
                [new Position2d(5500, 5500)] = "Tooting Bec",
                [new Position2d(6000, 4750)] = "Balham",
                [new Position2d(6500, 4000)] = "Clapham South",
                [new Position2d(7000, 3000)] = "Clapham Common",
                [new Position2d(7000, 2000)] = "Clapham North",
                [new Position2d(8000, 1500)] = "Stockwell",
                [new Position2d(9000, 1500)] = "Oval",
            }, tsd);

            return new Line("2", rA, rB);
        }

        private static Line CreateLine3(IDictionary<string, TransferStation> tsd)
        {
            var (rA, rB) = CreateRoutes(new Dictionary<Position2d, string>
            {
                [new Position2d(5500, 2000)] = "Chesham",
                [new Position2d(5500, 3000)] = "Amersham",
                [new Position2d(5500, 4000)] = "Neasden",
                [new Position2d(6000, 4750)] = "Balham",
                [new Position2d(6750, 5750)] = "Chalfont",
                [new Position2d(8000, 6000)] = "Kilburn",
                [new Position2d(9000, 6250)] = "Watford",
                [new Position2d(9900, 6300)] = "Croxley"
            }, tsd);

            return new Line("3", rA, rB);
        }

        private static Line CreateLine4(IDictionary<string, TransferStation> tsd)
        {
            var (rA, rB) = CreateRoutes(new Dictionary<Position2d, string>
            {
                [new Position2d(500, 5500)] = "Epping",
                [new Position2d(1500, 5000)] = "Theydon Bois",
                [new Position2d(2500, 4500)] = "Debden",
                [new Position2d(3500, 4500)] = "Loughton",
                [new Position2d(4750, 5000)] = "Buckhurst Hill",
                [new Position2d(5900, 4900)] = "Balham",
                [new Position2d(7000, 4500)] = "Woodford",
                [new Position2d(7500, 4100)] = "Dollis Hill",
                [new Position2d(8500, 3500)] = "Roding Valley",
                [new Position2d(9600, 3600)] = "Chigwell"
            }, tsd);

            return new Line("4", rA, rB);
        }

        private static Tuple<Route, Route> CreateRoutes(IDictionary<Position2d, string> dic, IDictionary<string, TransferStation> tsd)
        {
            var positions = dic.Keys.ToList();
            var spline = GetSpline(positions, 4);

            var routeAList = new List<Station>();
            var routeBList = new List<Station>();
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

            var routeA = new Route(routeAList);
            routeBList.Reverse();
            var routeB = new Route(routeBList);

            return new Tuple<Route, Route>(routeA, routeB);
        }

        private static Tuple<Position2d, Position2d> GetOffsetPoints(Position2d a, Position2d b, Position2d c, double offset)
        {
            var vec = c - a;
            var vecRight = vec.RotateRight().Normalize() * offset;
            var vecLeft = vec.RotateLeft().Normalize() * offset;
            return new Tuple<Position2d, Position2d>(b + vecRight, b + vecLeft);

        }

        private static List<Position2d> GetSpline(List<Position2d> controlPoints, int subsegments)
        {
            if (controlPoints.Count < 2)
            {
                return null;
            }

            if (controlPoints.Count == 2)
            {
                return controlPoints;
            }

            var pts = new List<Position2d>
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

        private static Position2d PointOnCurve(Position2d p0, Position2d p1, Position2d p2, Position2d p3, double t)
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

            return new Position2d(x, y);
        }
    }
}
