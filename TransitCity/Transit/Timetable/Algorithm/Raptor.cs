using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Time;
using Transit.Data;
using Utility.Extensions;
using Utility.Units;

namespace Transit.Timetable.Algorithm
{
    public class Raptor : RaptorBase
    {
        public Raptor(TimeSpan maxWalkingTime, TimeSpan maxWaitingTime, DataManager dataManager)
            : base(maxWalkingTime, maxWaitingTime, dataManager)
        {
        }

        public override List<Connection> Compute(Position2d sourcePos, WeekTimePoint startTime, Position2d targetPos, Speed walkingSpeed)
        {
            var earliestKnownTargetArrivalTime = startTime + TimeSpan.FromSeconds(sourcePos.DistanceTo(targetPos) / walkingSpeed.MetersPerSecond);
            var earliestConnections = new List<Connection>
            {
                Connection.CreateWalk(sourcePos, startTime, targetPos, earliestKnownTargetArrivalTime)
            };

            var (markedStations, connections) = GetInitialMarkedStations(sourcePos, startTime, walkingSpeed);
            earliestConnections.AddRange(connections);

            for (var k = 1; markedStations.Count > 0 && k <= NumRounds; ++k)
            {
                ComputeRound(targetPos, earliestConnections, markedStations, ref earliestKnownTargetArrivalTime, walkingSpeed);
            }

            return GetTravelPath(earliestConnections, targetPos);
        }

        public override List<Connection> ComputeReverse(Position2d sourcePos, WeekTimePoint latestArrivalTime, Position2d targetPos, Speed walkingSpeed)
        {
            var latestKnownSourceDepartureTime = latestArrivalTime - TimeSpan.FromSeconds(sourcePos.DistanceTo(targetPos) / walkingSpeed.MetersPerSecond);
            var latestConnections = new List<Connection>
            {
                Connection.CreateWalk(sourcePos, latestKnownSourceDepartureTime, targetPos, latestArrivalTime)
            };

            var (markedStations, connections) = GetInitialMarkedStationsReverse(targetPos, latestArrivalTime, walkingSpeed);
            latestConnections.AddRange(connections);

            for (var k = 1; markedStations.Count > 0 && k <= NumRounds; ++k)
            {
                ComputeRoundReverse(sourcePos, latestConnections, markedStations, ref latestKnownSourceDepartureTime, walkingSpeed);
            }

            return GetTravelPathReverse(latestConnections, sourcePos);
        }

        private void ComputeRound(Position2d targetPos, List<Connection> earliestKnownConnections, IDictionary<StationInfo, WeekTimePoint> markedStations, ref WeekTimePoint earliestKnownTargetArrivalTime, Speed walkingSpeed)
        {
            var newlyMarkedStations = new Dictionary<StationInfo, WeekTimePoint>();

            foreach (var (station, startTime) in markedStations)
            {
                ComputeRoundForStation(station, startTime, ref earliestKnownTargetArrivalTime, earliestKnownConnections, targetPos, newlyMarkedStations, walkingSpeed);
            }

            markedStations.Clear();
            foreach (var newlyMarkedStation in newlyMarkedStations)
            {
                markedStations.Add(newlyMarkedStation.Key, newlyMarkedStation.Value);
            }
        }

        private void ComputeRoundReverse(Position2d sourcePos, List<Connection> latestKnownConnections, IDictionary<StationInfo, WeekTimePoint> markedStations, ref WeekTimePoint latestKnownSourceDepartureTime, Speed walkingSpeed)
        {
            var newlyMarkedStations = new Dictionary<StationInfo, WeekTimePoint>();

            foreach (var (station, departureTime) in markedStations)
            {
                ComputeRoundForStationReverse(station, departureTime, ref latestKnownSourceDepartureTime, latestKnownConnections, sourcePos, newlyMarkedStations, walkingSpeed);
            }

            markedStations.Clear();
            foreach (var newlyMarkedStation in newlyMarkedStations)
            {
                markedStations.Add(newlyMarkedStation.Key, newlyMarkedStation.Value);
            }
        }

        private void ComputeRoundForStation(StationInfo station, WeekTimePoint startTime, ref WeekTimePoint earliestKnownTargetArrivalTime, List<Connection> earliestKnownConnections, Position2d targetPos, IDictionary<StationInfo, WeekTimePoint> newlyMarkedStations, Speed walkingSpeed)
        {
            var (nextDeparture, trip) = station.GetNextDepartureAndTripArrayBinarySearch(startTime);
            if (nextDeparture == null)
            {
                return;
            }

            if (WeekTimePoint.GetCorrectedDifference(startTime, nextDeparture) > _maxWaitingTime)
            {
                return;
            }

            var newStations = CheckRoute(station, nextDeparture, trip, ref earliestKnownTargetArrivalTime, earliestKnownConnections, targetPos, walkingSpeed);
            foreach (var pair in newStations)
            {
                if (!newlyMarkedStations.ContainsKey(pair.Key) || newlyMarkedStations[pair.Key] > pair.Value)
                {
                    newlyMarkedStations[pair.Key] = pair.Value;
                }
            }
        }

        private void ComputeRoundForStationReverse(StationInfo station, WeekTimePoint departureTime, ref WeekTimePoint latestKnownSourceDepartureTime, List<Connection> latestKnownConnections, Position2d sourcePos, IDictionary<StationInfo, WeekTimePoint> newlyMarkedStations, Speed walkingSpeed)
        {
            var (lastArrival, trip) = station.GetLastArrivalAndTripArrayBinarySearch(departureTime);
            if (lastArrival == null)
            {
                return;
            }

            if (WeekTimePoint.GetCorrectedDifference(lastArrival, departureTime) > _maxWaitingTime)
            {
                return;
            }

            var newStations = CheckRouteReverse(station, lastArrival, trip, ref latestKnownSourceDepartureTime, latestKnownConnections, sourcePos, walkingSpeed);
            foreach (var pair in newStations)
            {
                if (!newlyMarkedStations.ContainsKey(pair.Key) || newlyMarkedStations[pair.Key] < pair.Value)
                {
                    newlyMarkedStations[pair.Key] = pair.Value;
                }
            }
        }

        private Dictionary<StationInfo, WeekTimePoint> CheckRoute(StationInfo currentStationInfo, WeekTimePoint currentDeparture, Trip trip, ref WeekTimePoint earliestKnownTargetArrivalTime, List<Connection> earliestKnownConnections, Position2d targetPos, Speed walkingSpeed)
        {
            var newlyMarkedStations = new Dictionary<StationInfo, WeekTimePoint>();
            var (lineInfo, routeInfo, stationInfo) = _dataManager.GetInfos(currentStationInfo.Station);
            var nextStationInfos = routeInfo.GetNextStationInfos(stationInfo).ToList();
            var nextArrivals = trip.GetNextArrivals(stationInfo.Station).ToList();
            if (nextStationInfos.Count != nextArrivals.Count)
            {
                throw new InvalidOperationException();
            }

            for (var i = 0; i < nextArrivals.Count; ++i)
            {
                var nextTime = nextArrivals[i];
                var nextStationInfo = nextStationInfos[i];
                var nextStation = nextStationInfo.Station;
                if (nextTime >= earliestKnownTargetArrivalTime)
                {
                    break;
                }

                var exitWalkingTime = TimeSpan.FromSeconds(nextStation.ExitPosition.DistanceTo(targetPos) / walkingSpeed.MetersPerSecond);
                if (exitWalkingTime < TimeSpan.FromMinutes(10))
                {
                    var arrivalAtTargetPos = nextTime + exitWalkingTime;
                    if (arrivalAtTargetPos < earliestKnownTargetArrivalTime)
                    {
                        earliestKnownTargetArrivalTime = arrivalAtTargetPos;
                        var idx = earliestKnownConnections.FindIndex(c => c.TargetPos.DistanceTo(targetPos) < float.Epsilon);
                        earliestKnownConnections[idx] = Connection.CreateWalkFromStation(nextStation, nextTime, targetPos, arrivalAtTargetPos);
                    }
                }

                if (earliestKnownConnections.Any(c => c.TargetStation == nextStation && c.TargetTime <= nextTime))
                {
                    continue;
                }

                if (earliestKnownConnections.All(c => c.TargetStation != nextStation))
                {
                    var connection = Connection.CreateRide(stationInfo.Station, currentDeparture, nextStation, nextTime, lineInfo.Line);
                    earliestKnownConnections.Add(connection);
                    newlyMarkedStations.Add(nextStationInfo, nextTime);
                }
                else if (earliestKnownConnections.Any(c => c.TargetStation == nextStation && c.TargetTime > nextTime))
                {
                    var connection = Connection.CreateRide(stationInfo.Station, currentDeparture, nextStation, nextTime, lineInfo.Line);
                    var idx = earliestKnownConnections.FindIndex(c => c.TargetStation == nextStation && c.TargetTime > nextTime);
                    earliestKnownConnections[idx] = connection;
                    newlyMarkedStations.Add(nextStationInfo, nextTime);
                }

                var transferStation = _dataManager.GetTransferStation(nextStation);
                var otherStations = transferStation.Stations.Where(s => s != nextStation);
                foreach (var otherStation in otherStations)
                {
                    const float exitTime = 10f;
                    var transferTime = nextStation.ExitPosition.DistanceTo(otherStation.EntryPosition) / walkingSpeed.MetersPerSecond;
                    var timespan = TimeSpan.FromSeconds(transferTime + exitTime);
                    var arrivalTime = nextTime + timespan;
                    if (arrivalTime >= earliestKnownTargetArrivalTime)
                    {
                        continue;
                    }

                    var connection = Connection.CreateTransfer(nextStation, nextTime, otherStation, arrivalTime);
                    var otherStationInfo = _dataManager.GetInfos(otherStation).stationInfo;
                    if (earliestKnownConnections.All(c => c.TargetStation != otherStation))
                    {
                        earliestKnownConnections.Add(connection);
                        newlyMarkedStations.Add(otherStationInfo, arrivalTime);
                    }
                    else if (earliestKnownConnections.Any(c => c.TargetStation == otherStation && c.TargetTime > arrivalTime))
                    {
                        var idx = earliestKnownConnections.FindIndex(c => c.TargetStation == otherStation && c.TargetTime > arrivalTime);
                        earliestKnownConnections[idx] = connection;
                        newlyMarkedStations.Add(otherStationInfo, arrivalTime);
                    }
                }
            }

            return newlyMarkedStations;
        }

        private Dictionary<StationInfo, WeekTimePoint> CheckRouteReverse(StationInfo currentStationInfo, WeekTimePoint currentArrival, Trip trip, ref WeekTimePoint latestKnownSourceDepartureTime, List<Connection> latestKnownConnections, Position2d sourcePos, Speed walkingSpeed)
        {
            var newlyMarkedStations = new Dictionary<StationInfo, WeekTimePoint>();
            var (lineInfo, routeInfo, stationInfo) = _dataManager.GetInfos(currentStationInfo.Station);
            var lastStationInfos = routeInfo.GetLastStationInfos(stationInfo).ToList();
            var lastDepartures = trip.GetLastDepartures(stationInfo.Station).ToList();
            if (lastStationInfos.Count != lastDepartures.Count)
            {
                throw new InvalidOperationException();
            }

            for (var i = 0; i < lastDepartures.Count; ++i)
            {
                var lastTime = lastDepartures[i];
                var lastStationInfo = lastStationInfos[i];
                var lastStation = lastStationInfo.Station;
                if (lastTime <= latestKnownSourceDepartureTime)
                {
                    break;
                }

                var entryWalkingTime = TimeSpan.FromSeconds(lastStation.EntryPosition.DistanceTo(sourcePos) / walkingSpeed.MetersPerSecond);
                if (entryWalkingTime < TimeSpan.FromMinutes(10))
                {
                    var departureAtSorucePos = lastTime - entryWalkingTime;
                    if (departureAtSorucePos > latestKnownSourceDepartureTime)
                    {
                        latestKnownSourceDepartureTime = departureAtSorucePos;
                        var idx = latestKnownConnections.FindIndex(c => c.SourcePos.DistanceTo(sourcePos) < float.Epsilon);
                        latestKnownConnections[idx] = Connection.CreateWalkToStation(sourcePos, departureAtSorucePos, lastStation, lastTime);
                    }
                }

                if (latestKnownConnections.Any(c => c.SourceStation == lastStation && c.SourceTime >= lastTime))
                {
                    continue;
                }

                if (latestKnownConnections.All(c => c.SourceStation != lastStation))
                {
                    var connection = Connection.CreateRide(lastStation, lastTime, stationInfo.Station, currentArrival, lineInfo.Line);
                    latestKnownConnections.Add(connection);
                    newlyMarkedStations.Add(lastStationInfo, lastTime);
                }
                else if (latestKnownConnections.Any(c => c.SourceStation == lastStation && c.SourceTime < lastTime))
                {
                    var connection = Connection.CreateRide(lastStation, lastTime, stationInfo.Station, currentArrival, lineInfo.Line);
                    var idx = latestKnownConnections.FindIndex(c => c.SourceStation == lastStation && c.SourceTime < lastTime);
                    latestKnownConnections[idx] = connection;
                    newlyMarkedStations.Add(lastStationInfo, lastTime);
                }

                var transferStation = _dataManager.GetTransferStation(lastStation);
                var otherStations = transferStation.Stations.Where(s => s != lastStation);
                foreach (var otherStation in otherStations)
                {
                    const float exitTime = 10f;
                    var transferTime = lastStation.EntryPosition.DistanceTo(otherStation.ExitPosition) / walkingSpeed.MetersPerSecond;
                    var timespan = TimeSpan.FromSeconds(transferTime + exitTime);
                    var departureTime = lastTime - timespan;
                    if (departureTime <= latestKnownSourceDepartureTime)
                    {
                        continue;
                    }

                    var connection = Connection.CreateTransfer(otherStation, departureTime, lastStation, lastTime);
                    var otherStationInfo = _dataManager.GetInfos(otherStation).stationInfo;
                    if (latestKnownConnections.All(c => c.SourceStation != otherStation))
                    {
                        latestKnownConnections.Add(connection);
                        newlyMarkedStations.Add(otherStationInfo, departureTime);
                    }
                    else if (latestKnownConnections.Any(c => c.SourceStation == otherStation && c.SourceTime < departureTime))
                    {
                        var idx = latestKnownConnections.FindIndex(c => c.SourceStation == otherStation && c.SourceTime < departureTime);
                        latestKnownConnections[idx] = connection;
                        newlyMarkedStations.Add(otherStationInfo, departureTime);
                    }
                }
            }

            return newlyMarkedStations;
        }
    }
}
