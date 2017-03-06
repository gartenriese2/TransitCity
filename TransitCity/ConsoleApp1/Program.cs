using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using PathFinding.Network;
using Time;
using Transit;
using Transit.Timetable;

namespace ConsoleApp1
{
    internal static class Program
    {
        private static void Main(string[] args)
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

            var raptor = new Raptor<Position2f>(manager);

            raptor.Compute(transferStationDictionary.Values.ElementAt(0), new WeekTimePoint(DayOfWeek.Monday, 5), transferStationDictionary.Values.ElementAt(4), new List<TransferStation<Position2f>>(transferStationDictionary.Values));
            raptor.Compute(transferStationDictionary["Stanmore"], new WeekTimePoint(DayOfWeek.Tuesday, 11, 30), transferStationDictionary["Stockwell"], new List<TransferStation<Position2f>>(transferStationDictionary.Values));
        }

        private static Station<Position2f> CreateStation(Position2f pos, string name, Dictionary<string, TransferStation<Position2f>> tsd)
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

        private static Line<Position2f> CreateLine1(Dictionary<string, TransferStation<Position2f>> tsd)
        {
            var station1A = CreateStation(new Position2f(1500, 996), "Stanmore", tsd);
            var station2A = CreateStation(new Position2f(2502, 996), "Canons Park", tsd);
            var station3A = CreateStation(new Position2f(3506, 1498), "Queensbury", tsd);
            var station4A = CreateStation(new Position2f(4018, 2522), "Kingsbury", tsd);
            var station5A = CreateStation(new Position2f(4506, 3498), "Wembley Park", tsd);
            var station6A = CreateStation(new Position2f(5502, 3996), "Neasden", tsd);
            var station7A = CreateStation(new Position2f(6524, 3996), "Clapham South", tsd);
            var station8A = CreateStation(new Position2f(7505, 3996), "Dollis Hill", tsd);
            var station9A = CreateStation(new Position2f(8008, 5003), "Willesden Green", tsd);
            var station10A = CreateStation(new Position2f(8008, 6006), "Kilburn", tsd);
            var station11A = CreateStation(new Position2f(7508, 7006), "West Hampstead", tsd);
            var station12A = CreateStation(new Position2f(7508, 8003), "Finchley Road", tsd);
            var station13A = CreateStation(new Position2f(8007, 9001), "Swiss Cottage", tsd);

            var station1B = CreateStation(new Position2f(7993, 9008), "Swiss Cottage", tsd);
            var station2B = CreateStation(new Position2f(7492, 8006), "Finchley Road", tsd);
            var station3B = CreateStation(new Position2f(7492, 7003), "West Hampstead", tsd);
            var station4B = CreateStation(new Position2f(7992, 6003), "Kilburn", tsd);
            var station5B = CreateStation(new Position2f(7992, 5006), "Willesden Green", tsd);
            var station6B = CreateStation(new Position2f(7495, 4012), "Dollis Hill", tsd);
            var station7B = CreateStation(new Position2f(6524, 4012), "Clapham South", tsd);
            var station8B = CreateStation(new Position2f(5498, 4012), "Neasden", tsd);
            var station9B = CreateStation(new Position2f(4494, 3510), "Wembley Park", tsd);
            var station10B = CreateStation(new Position2f(3999, 2520), "Kingsbury", tsd);
            var station11B = CreateStation(new Position2f(3494, 1510), "Queensbury", tsd);
            var station12B = CreateStation(new Position2f(2498, 1012), "Canons Park", tsd);
            var station13B = CreateStation(new Position2f(1500, 1012), "Stanmore", tsd);

            const float frequency = 300f;
            var route1A = new Route<Position2f>(new[] { station1A, station2A, station3A, station4A, station5A, station6A, station7A, station8A, station9A, station10A, station11A, station12A, station13A }, frequency);
            var route1B = new Route<Position2f>(new[] { station1B, station2B, station3B, station4B, station5B, station6B, station7B, station8B, station9B, station10B, station11B, station12B, station13B }, frequency);
            return new Line<Position2f>("1", route1A, route1B);
        }

        private static Line<Position2f> CreateLine2(Dictionary<string, TransferStation<Position2f>> tsd)
        {
            var station1A = CreateStation(new Position2f(1500, 8000), "Morden", tsd);
            var station2A = CreateStation(new Position2f(2500, 7500), "South Wimbledon", tsd);
            var station3A = CreateStation(new Position2f(3500, 7000), "Colliers Wood", tsd);
            var station4A = CreateStation(new Position2f(4500, 6500), "Tooting Broadway", tsd);
            var station5A = CreateStation(new Position2f(5500, 5500), "Tooting Bec", tsd);
            var station6A = CreateStation(new Position2f(6000, 4750), "Balham", tsd);
            var station7A = CreateStation(new Position2f(6500, 4000), "Clapham South", tsd);
            var station8A = CreateStation(new Position2f(7000, 3000), "Clapham Common", tsd);
            var station9A = CreateStation(new Position2f(7000, 2000), "Clapham North", tsd);
            var station10A = CreateStation(new Position2f(8000, 1500), "Stockwell", tsd);
            var station11A = CreateStation(new Position2f(9000, 1500), "Oval", tsd);

            var station1B = CreateStation(new Position2f(9000, 1520), "Oval", tsd);
            var station2B = CreateStation(new Position2f(8000, 1520), "Stockwell", tsd);
            var station3B = CreateStation(new Position2f(7000, 2020), "Clapham North", tsd);
            var station4B = CreateStation(new Position2f(7000, 3020), "Clapham Common", tsd);
            var station5B = CreateStation(new Position2f(6500, 4020), "Clapham South", tsd);
            var station6B = CreateStation(new Position2f(6000, 4770), "Balham", tsd);
            var station7B = CreateStation(new Position2f(5500, 5520), "Tooting Bec", tsd);
            var station8B = CreateStation(new Position2f(4500, 6520), "Tooting Broadway", tsd);
            var station9B = CreateStation(new Position2f(3500, 7020), "Colliers Wood", tsd);
            var station10B = CreateStation(new Position2f(2500, 7520), "South Wimbledon", tsd);
            var station11B = CreateStation(new Position2f(1500, 8020), "Morden", tsd);

            const float frequency = 240f;
            var route1A = new Route<Position2f>(new[] { station1A, station2A, station3A, station4A, station5A, station6A, station7A, station8A, station9A, station10A, station11A }, frequency);
            var route1B = new Route<Position2f>(new[] { station1B, station2B, station3B, station4B, station5B, station6B, station7B, station8B, station9B, station10B, station11B }, frequency);
            return new Line<Position2f>("2", route1A, route1B);
        }
    }
}
