using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Time;
using Utility.Units;

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

        public IEnumerable<Trip<Position2f>> GetActiveTrips(WeekTimePoint wtp)
        {
            return Trips.Where(trip => new WeekTimeSpan(trip.DepartureAtStation(trip.Stations.First()), trip.ArrivalAtStation(trip.Stations.Last())).IsInside(wtp));
        }

        public IEnumerable<Position2f> GetActiveVehiclePositions(WeekTimePoint wtp)
        {
            var trips = GetActiveTrips(wtp).ToList();
            var positions = new List<Position2f>(trips.Count);
            foreach (var trip in trips)
            {
                var lastArrival = trip.DepartureAtStation(trip.Stations.First()) - TimeSpan.FromSeconds(30); // TODO
                var lastDeparture = trip.DepartureAtStation(trip.Stations.First());
                var stationIdx = 0;
                var lastIsArrival = true;
                while (lastArrival < wtp && lastDeparture < wtp)
                {
                    if (lastIsArrival)
                    {
                        lastArrival = trip.ArrivalAtStation(trip.Stations.ElementAt(stationIdx + 1));
                    }
                    else
                    {
                        lastDeparture = trip.DepartureAtStation(trip.Stations.ElementAt(stationIdx + 1));
                        ++stationIdx;
                    }

                    lastIsArrival = !lastIsArrival;
                }

                if (lastArrival < lastDeparture)
                {
                    positions.Add(trip.Stations.ElementAt(stationIdx).Position);
                }
                else
                {
                    var dist = SubwayTravelDistanceFunc(Duration.FromSeconds((float) (wtp - lastDeparture).TotalSeconds),  Duration.FromSeconds((float) (lastArrival - lastDeparture).TotalSeconds));
                    var idx = Path.FindIndex(f => f.EqualPosition(trip.Stations.ElementAt(stationIdx).Position));
                    var idx2 = Path.FindIndex(f => f.EqualPosition(trip.Stations.ElementAt(stationIdx + 1).Position));
                    var subPath = Path.Subpath(idx, idx2 - idx + 1);
                    if (!subPath[0].EqualPosition(trip.Stations.ElementAt(stationIdx).Position))
                    {
                        throw new InvalidOperationException();
                    }

                    if (!subPath.Last().EqualPosition(trip.Stations.ElementAt(stationIdx + 1).Position))
                    {
                        throw new InvalidOperationException();
                    }

                    var rel = dist / subPath.Length();
                    positions.Add(subPath.Lerp(rel.Meters));
                }
            }

            return positions;

            Distance SubwayTravelDistanceFunc(Duration duration, Duration partDuration)
            {
                if (duration > partDuration)
                {
                    throw new ArgumentOutOfRangeException();
                }

                var meanAcceleration = Acceleration.FromMetersPerSecondSquared(0.6f);
                var maximalSpeed = Speed.FromKilometersPerHour(70f);
                var timeToReachMaximalSpeed = maximalSpeed / meanAcceleration;

                if (duration <= timeToReachMaximalSpeed)
                {
                    return meanAcceleration / 2 * duration * duration;
                }

                var neededDistanceToReachMaximalSpeed = meanAcceleration / 2 * timeToReachMaximalSpeed * timeToReachMaximalSpeed;

                if (duration <= partDuration - timeToReachMaximalSpeed)
                {
                    var remainingDuration = duration - timeToReachMaximalSpeed;
                    return neededDistanceToReachMaximalSpeed + maximalSpeed * remainingDuration;
                }

                var durationAtMaxSpeed = partDuration - 2 * timeToReachMaximalSpeed;
                var remainingDuration2 = duration - timeToReachMaximalSpeed - durationAtMaxSpeed;
                return neededDistanceToReachMaximalSpeed + maximalSpeed * durationAtMaxSpeed + meanAcceleration / 2 * remainingDuration2 * remainingDuration2;
            }
        }
    } 
}
