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
                var (lastArrival, lastDeparture, stationIdx) = GetArrivalDepartureInfoFromTrip(trip, wtp);

                if (lastArrival < lastDeparture)
                {
                    positions.Add(trip.Stations.ElementAt(stationIdx).Position);
                }
                else
                {
                    var departurePosition = trip.Stations.ElementAt(stationIdx).Position;
                    var arrivalPosition = trip.Stations.ElementAt(stationIdx + 1).Position;
                    var positionOnPath = GetPositionOnPath(wtp, lastDeparture, lastArrival, departurePosition, arrivalPosition);
                    positions.Add(positionOnPath);
                }
            }

            return positions;
        }

        public IEnumerable<(Position2f, Vector2f)> GetActiveVehiclePositionsAndDirections(WeekTimePoint wtp)
        {
            var trips = GetActiveTrips(wtp).ToList();
            var positions = new List<(Position2f, Vector2f)>(trips.Count);
            foreach (var trip in trips)
            {
                var (lastArrival, lastDeparture, stationIdx) = GetArrivalDepartureInfoFromTrip(trip, wtp);

                if (lastArrival < lastDeparture)
                {
                    var pos = trip.Stations.ElementAt(stationIdx).Position;
                    var idx = Path.FindIndex(f => f.EqualPosition(pos));
                    var vec = idx == Path.Count - 1 ? Path[idx] - Path[idx - 1] : Path[idx + 1] - Path[idx];
                    positions.Add((pos, vec));
                }
                else
                {
                    var departurePosition = trip.Stations.ElementAt(stationIdx).Position;
                    var arrivalPosition = trip.Stations.ElementAt(stationIdx + 1).Position;
                    var (pos, vec) = GetPositionAndDirectionOnPath(wtp, lastDeparture, lastArrival, departurePosition, arrivalPosition);
                    positions.Add((pos, vec));
                }
            }

            return positions;
        }

        private static (WeekTimePoint, WeekTimePoint, int) GetArrivalDepartureInfoFromTrip(Trip<Position2f> trip, WeekTimePoint wtp)
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

            return (lastArrival, lastDeparture, stationIdx);
        }

        private Position2f GetPositionOnPath(
            WeekTimePoint wtp,
            WeekTimePoint departureFromLastStation,
            WeekTimePoint arrivalAtNextStation,
            Position2f departurePosition,
            Position2f arrivalPosition)
        {
            var traveledDistance = SubwayTravelDistanceFunc(Duration.FromSeconds((float)(wtp - departureFromLastStation).TotalSeconds), Duration.FromSeconds((float)(arrivalAtNextStation - departureFromLastStation).TotalSeconds));
            var departureStationIndex = Path.FindIndex(f => f.EqualPosition(departurePosition));
            var arrivalStationIndex = Path.FindIndex(f => f.EqualPosition(arrivalPosition));
            var pathBetweenStations = Path.Subpath(departureStationIndex, arrivalStationIndex - departureStationIndex + 1);
            if (!pathBetweenStations.First().EqualPosition(departurePosition))
            {
                throw new InvalidOperationException();
            }

            if (!pathBetweenStations.Last().EqualPosition(arrivalPosition))
            {
                throw new InvalidOperationException();
            }

            var relativeDistance = traveledDistance.Meters / pathBetweenStations.Length();
            return pathBetweenStations.Lerp(relativeDistance);
        }

        private (Position2f, Vector2f) GetPositionAndDirectionOnPath(
            WeekTimePoint wtp,
            WeekTimePoint departureFromLastStation,
            WeekTimePoint arrivalAtNextStation,
            Position2f departurePosition,
            Position2f arrivalPosition)
        {
            var traveledDistance = SubwayTravelDistanceFunc(Duration.FromSeconds((float)(wtp - departureFromLastStation).TotalSeconds), Duration.FromSeconds((float)(arrivalAtNextStation - departureFromLastStation).TotalSeconds));
            var departureStationIndex = Path.FindIndex(f => f.EqualPosition(departurePosition));
            var arrivalStationIndex = Path.FindIndex(f => f.EqualPosition(arrivalPosition));
            var pathBetweenStations = Path.Subpath(departureStationIndex, arrivalStationIndex - departureStationIndex + 1);
            if (!pathBetweenStations.First().EqualPosition(departurePosition))
            {
                throw new InvalidOperationException();
            }

            if (!pathBetweenStations.Last().EqualPosition(arrivalPosition))
            {
                throw new InvalidOperationException();
            }

            var relativeDistance = traveledDistance.Meters / pathBetweenStations.Length();
            var pos = pathBetweenStations.Lerp(relativeDistance);
            var vec = pathBetweenStations.DirectionLerp(relativeDistance);
            return (pos, vec);
        }

        private static Distance SubwayTravelDistanceFunc(Duration duration, Duration partDuration)
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
