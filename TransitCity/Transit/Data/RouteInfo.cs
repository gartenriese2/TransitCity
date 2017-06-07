using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Time;
using Utility;
using Utility.Units;

namespace Transit.Data
{
    public class RouteInfo
    {
        private readonly WeekTimeDictionary<Trip> _weekTimeTripsDictionary;

        public RouteInfo(Route route, Path path, List<StationInfo> stationInfos, List<Trip> trips)
        {
            Route = route ?? throw new ArgumentNullException(nameof(route));
            Path = path ?? throw new ArgumentNullException(nameof(path));
            StationInfos = stationInfos ?? throw new ArgumentNullException(nameof(stationInfos));
            Trips = trips ?? throw new ArgumentNullException(nameof(trips));
            _weekTimeTripsDictionary = new WeekTimeDictionary<Trip>(WeekTimeDictionary<Trip>.Granularity.TwentyMinutes, Trips);
        }

        public Route Route { get; }

        public Path Path { get; }

        public List<StationInfo> StationInfos { get; }

        public List<Trip> Trips { get; }

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

        public IEnumerable<WeekTimePoint> GetNextArrivalsOnTrip(Station station, WeekTimePoint departure)
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

        public IEnumerable<WeekTimePoint> GetNextDeparturesOnTrip(Station station, WeekTimePoint departure)
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

        public IEnumerable<Trip> GetActiveTrips(WeekTimePoint wtp)
        {
            var hourlyTrips = _weekTimeTripsDictionary[wtp];
            return hourlyTrips.Where(trip => new WeekTimeSpan(trip.DepartureAtStation(trip.Stations.First()), trip.ArrivalAtStation(trip.Stations.Last())).IsInside(wtp));
        }

        public IEnumerable<Position2d> GetActiveVehiclePositions(WeekTimePoint wtp)
        {
            var trips = GetActiveTrips(wtp).ToList();
            var positions = new List<Position2d>(trips.Count);
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

        public IEnumerable<(Trip, Position2d, Vector2d)> GetActiveVehiclePositionsAndDirections(WeekTimePoint wtp)
        {
            var trips = GetActiveTrips(wtp).ToList();
            var posAndDirs = new List<(Trip, Position2d, Vector2d)>(trips.Count);
            foreach (var trip in trips)
            {
                var (lastArrival, lastDeparture, stationIdx) = GetArrivalDepartureInfoFromTrip(trip, wtp);

                if (lastArrival < lastDeparture)
                {
                    var pos = trip.Stations.ElementAt(stationIdx).Position;
                    var idx = Path.FindIndex(f => f.EqualPosition(pos));
                    var vec = idx == Path.Count - 1 ? Path[idx] - Path[idx - 1] : Path[idx + 1] - Path[idx];
                    posAndDirs.Add((trip, pos, vec));
                }
                else
                {
                    var departurePosition = trip.Stations.ElementAt(stationIdx).Position;
                    var arrivalPosition = trip.Stations.ElementAt(stationIdx + 1).Position;
                    var (pos, vec) = GetPositionAndDirectionOnPath(wtp.TimePoint, lastDeparture.TimePoint, lastArrival.TimePoint, departurePosition, arrivalPosition);
                    posAndDirs.Add((trip, pos, vec));
                }
            }

            return posAndDirs;
        }

        private static (WeekTimePoint, WeekTimePoint, int) GetArrivalDepartureInfoFromTrip(Trip trip, WeekTimePoint wtp)
        {
            var lastArrival = trip.DepartureAtStation(trip.Stations.First()) - TimeSpan.FromSeconds(30); // TODO
            var lastDeparture = trip.DepartureAtStation(trip.Stations.First());
            var stationIdx = 0;
            var lastIsArrival = true; // arrival must be less than departure if true
            while ((lastIsArrival && lastDeparture < wtp) || (!lastIsArrival && lastArrival < wtp))
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

        private Position2d GetPositionOnPath(
            WeekTimePoint wtp,
            WeekTimePoint departureFromLastStation,
            WeekTimePoint arrivalAtNextStation,
            Position2d departurePosition,
            Position2d arrivalPosition)
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

        private (Position2d, Vector2d) GetPositionAndDirectionOnPath(
            TimeSpan wtp,
            TimeSpan departureFromLastStation,
            TimeSpan arrivalAtNextStation,
            Position2d departurePosition,
            Position2d arrivalPosition)
        {
            if (wtp < departureFromLastStation || wtp > arrivalAtNextStation)
            {
                throw new ArgumentOutOfRangeException();
            }

            var traveledDistance = SubwayTravelDistanceFunc(Duration.FromSeconds((wtp - departureFromLastStation).TotalSeconds), Duration.FromSeconds((arrivalAtNextStation - departureFromLastStation).TotalSeconds));
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

            var pathBetweenStationsLength = pathBetweenStations.Length();
            if (traveledDistance.Meters - pathBetweenStationsLength > 1)
            {
                throw new InvalidOperationException("More than 1m difference");
            }

            var relativeDistance = Math.Min(1.0, traveledDistance.Meters / pathBetweenStationsLength);
            var pos = pathBetweenStations.Lerp(relativeDistance);
            var vec = pathBetweenStations.DirectionLerp(relativeDistance);
            return (pos, vec);
        }

        private static Distance SubwayTravelDistanceFunc(Duration duration, Duration partDuration)
        {
            return Movement.GetDistanceFromDuration(duration, partDuration, Acceleration.FromMetersPerSecondSquared(0.6), Speed.FromKilometersPerHour(70));
        }
    }
}
