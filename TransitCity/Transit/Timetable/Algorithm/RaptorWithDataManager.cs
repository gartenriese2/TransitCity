using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Time;
using Transit.Data;
using Utility.Extensions;

namespace Transit.Timetable.Algorithm
{
    using Connection2f = Connection<Position2f>;

    public class RaptorWithDataManager : RaptorWithDataManagerBase
    {
        public RaptorWithDataManager(float walkingSpeed, TimeSpan maxWalkingTime, TimeSpan maxWaitingTime, DataManager dataManager)
            : base(walkingSpeed, maxWalkingTime, maxWaitingTime, dataManager)
        {
        }

        public override List<Connection2f> Compute(Position2f sourcePos, WeekTimePoint startTime, Position2f targetPos)
        {
            var earliestKnownTargetArrivalTime = startTime + TimeSpan.FromSeconds(sourcePos.DistanceTo(targetPos) / _walkingSpeed);
            var earliestConnections = new List<Connection<Position2f>>
            {
                Connection2f.CreateWalk(sourcePos, startTime, targetPos, earliestKnownTargetArrivalTime)
            };

            var (markedStations, connections) = GetInitialMarkedStations(sourcePos, startTime);
            earliestConnections.AddRange(connections);

            for (var k = 1; markedStations.Count > 0 && k <= NumRounds; ++k)
            {
                ComputeRound(targetPos, earliestConnections, markedStations, ref earliestKnownTargetArrivalTime);
            }

            return GetTravelPath(earliestConnections, targetPos);
        }

        private void ComputeRound(Position2f targetPos, List<Connection2f> earliestKnownConnections, IDictionary<StationInfo, WeekTimePoint> markedStations, ref WeekTimePoint earliestKnownTargetArrivalTime)
        {
            var newlyMarkedStations = new Dictionary<StationInfo, WeekTimePoint>();

            foreach (var (station, startTime) in markedStations)
            {
                ComputeRoundForStation(station, startTime, ref earliestKnownTargetArrivalTime, earliestKnownConnections, targetPos, newlyMarkedStations);
            }

            markedStations.Clear();
            foreach (var newlyMarkedStation in newlyMarkedStations)
            {
                markedStations.Add(newlyMarkedStation.Key, newlyMarkedStation.Value);
            }
        }

        private void ComputeRoundForStation(StationInfo station, WeekTimePoint startTime, ref WeekTimePoint earliestKnownTargetArrivalTime, List<Connection2f> earliestKnownConnections, Position2f targetPos, Dictionary<StationInfo, WeekTimePoint> newlyMarkedStations)
        {
            var nextDeparture = station.GetNextDeparture(startTime);
            if (nextDeparture == null)
            {
                return;
            }

            if (WeekTimePoint.GetCorrectedDifference(startTime, nextDeparture) > _maxWaitingTime)
            {
                return;
            }

            var newStations = CheckRoute(station, nextDeparture, ref earliestKnownTargetArrivalTime, earliestKnownConnections, targetPos);
            foreach (var pair in newStations)
            {
                newlyMarkedStations.Add(pair.Key, pair.Value);
            }
        }

        private Dictionary<StationInfo, WeekTimePoint> CheckRoute(StationInfo currentStationInfo, WeekTimePoint currentDeparture, ref WeekTimePoint earliestKnownTargetArrivalTime, List<Connection2f> earliestKnownConnections, Position2f targetPos)
        {
            var newlyMarkedStations = new Dictionary<StationInfo, WeekTimePoint>();
            var (lineInfo, routeInfo, stationInfo) = _dataManager.GetInfos(currentStationInfo.Station);
            var nextStationInfos = routeInfo.GetNextStationInfos(stationInfo).ToList();
            var nextArrivals = routeInfo.GetNextArrivalsOnTrip(stationInfo.Station, currentDeparture).ToList();
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

                var exitWalkingTime = TimeSpan.FromSeconds(nextStation.ExitPosition.DistanceTo(targetPos) / _walkingSpeed);
                if (exitWalkingTime < TimeSpan.FromMinutes(10))
                {
                    var arrivalAtTargetPos = nextTime + exitWalkingTime;
                    if (arrivalAtTargetPos < earliestKnownTargetArrivalTime)
                    {
                        earliestKnownTargetArrivalTime = arrivalAtTargetPos;
                        var idx = earliestKnownConnections.FindIndex(c => c.TargetPos.DistanceTo(targetPos) < float.Epsilon);
                        earliestKnownConnections[idx] = Connection2f.CreateWalkFromStation(nextStation, nextTime, targetPos, arrivalAtTargetPos);
                    }
                }

                if (earliestKnownConnections.Any(c => c.TargetStation == nextStation && c.TargetTime <= nextTime))
                {
                    continue;
                }

                if (earliestKnownConnections.All(c => c.TargetStation != nextStation))
                {
                    var connection = Connection2f.CreateRide(stationInfo.Station, currentDeparture, nextStation, nextTime, lineInfo.Line);
                    earliestKnownConnections.Add(connection);
                    newlyMarkedStations.Add(nextStationInfo, nextTime);
                }
                else if (earliestKnownConnections.Any(c => c.TargetStation == nextStation && c.TargetTime > nextTime))
                {
                    var connection = Connection2f.CreateRide(stationInfo.Station, currentDeparture, nextStation, nextTime, lineInfo.Line);
                    var idx = earliestKnownConnections.FindIndex(c => c.TargetStation == nextStation && c.TargetTime > nextTime);
                    earliestKnownConnections[idx] = connection;
                    newlyMarkedStations.Add(nextStationInfo, nextTime);
                }

                var transferStation = _dataManager.GetTransferStation(nextStation);
                var otherStations = transferStation.Stations.Where(s => s != nextStation);
                foreach (var otherStation in otherStations)
                {
                    const float exitTime = 10f;
                    var transferTime = nextStation.ExitPosition.DistanceTo(otherStation.EntryPosition) / _walkingSpeed;
                    var timespan = TimeSpan.FromSeconds(transferTime + exitTime);
                    var arrivalTime = nextTime + timespan;
                    if (arrivalTime >= earliestKnownTargetArrivalTime)
                    {
                        continue;
                    }

                    var connection = Connection2f.CreateTransfer(nextStation, nextTime, otherStation, arrivalTime);
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
    }
}
