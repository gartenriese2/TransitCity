using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Time;

namespace Transit.Data
{
    public class RouteInfo
    {
        public RouteInfo(Route<Position2f> route, Path path, List<StationInfo> stationInfos, List<Trip<Position2f>> trips)
        {
            Route = route ?? throw new ArgumentNullException(nameof(route));
            Path = path ?? throw new ArgumentNullException(nameof(path));
            StationInfos = stationInfos ?? throw new ArgumentNullException(nameof(stationInfos));
            Trips = trips ?? throw new ArgumentNullException(nameof(trips));
        }

        public Route<Position2f> Route { get; }

        public Path Path { get; }

        public List<StationInfo> StationInfos { get; }

        public List<Trip<Position2f>> Trips { get; }

        public IEnumerable<StationInfo> GetNextStationInfos(StationInfo stationInfo)
        {
            var idx = StationInfos.IndexOf(stationInfo);
            if (idx == -1)
            {
                throw new ArgumentException();
            }

            return StationInfos.Skip(idx + 1);
        }

        public IEnumerable<StationInfo> GetLastStationInfos(StationInfo stationInfo)
        {
            var idx = StationInfos.IndexOf(stationInfo);
            if (idx == -1)
            {
                throw new ArgumentException();
            }

            return StationInfos.Take(idx).Reverse();
        }

        public IEnumerable<WeekTimePoint> GetNextArrivalsOnTrip(Station<Position2f> station, WeekTimePoint departure)
        {
            foreach (var trip in Trips)
            {
                if (!(trip.DepartureAtStation(station) == departure))
                {
                    continue;
                }

                return trip.GetNextArrivals(station);
            }

            throw new ArgumentException("Could not find a trip with the specified departure time.");
        }

        public IEnumerable<WeekTimePoint> GetNextDeparturesOnTrip(Station<Position2f> station, WeekTimePoint departure)
        {
            foreach (var trip in Trips)
            {
                if (!(trip.DepartureAtStation(station) == departure))
                {
                    continue;
                }

                return trip.GetNextDepartures(station);
            }

            throw new ArgumentException("Could not find a trip with the specified departure time.");
        }
    }
}
