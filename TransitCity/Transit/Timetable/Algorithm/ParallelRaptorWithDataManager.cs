using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geometry;
using Time;
using Transit.Data;
using Utility.Extensions;
using Utility.Threading;

namespace Transit.Timetable.Algorithm
{
    using Connection2f = Connection<Position2f>;

    public class ParallelRaptorWithDataManager : RaptorWithDataManagerBase
    {
        private readonly object _lockObjConnections = new object();

        public ParallelRaptorWithDataManager(float walkingSpeed, TimeSpan maxWalkingTime, TimeSpan maxWaitingTime, DataManager dataManager)
            : base(walkingSpeed, maxWalkingTime, maxWaitingTime, dataManager)
        {
        }

        public override List<Connection2f> Compute(Position2f startPos, WeekTimePoint startTime, Position2f targetPos)
        {
            var earliestKnownTargetArrivalTime = new Atomic<AtomicWeekTimePoint>(new AtomicWeekTimePoint(startTime + TimeSpan.FromSeconds(startPos.DistanceTo(targetPos) / _walkingSpeed)));
            var earliestConnections = new ConcurrentBag<Connection2f>
            {
                Connection2f.CreateWalk(startPos, startTime, targetPos, earliestKnownTargetArrivalTime.Data())
            };

            var (markedStations, connections) = GetInitialMarkedStations(startPos, startTime);
            connections.ForEach(c => earliestConnections.Add(c));

            for (var k = 1; markedStations.Count > 0 && k <= NumRounds; ++k)
            {
                ComputeRound(targetPos, earliestConnections, markedStations, earliestKnownTargetArrivalTime);
            }

            return GetTravelPath(earliestConnections, targetPos);
        }

        private void ComputeRound(Position2f targetPos, ConcurrentBag<Connection2f> earliestKnownConnections, IDictionary<StationInfo, WeekTimePoint> markedStations, Atomic<AtomicWeekTimePoint> earliestKnownTargetArrivalTime)
        {
            var newlyMarkedStations = new ConcurrentDictionary<StationInfo, WeekTimePoint>();

            var taskList = new List<Task>();
            foreach (var (station, startTime) in markedStations)
            {
                taskList.Add(Task.Factory.StartNew(() => ComputeRoundForStation(station, startTime, earliestKnownTargetArrivalTime, earliestKnownConnections, targetPos, newlyMarkedStations)));
            }

            Task.WaitAll(taskList.ToArray());

            markedStations.Clear();
            foreach (var newlyMarkedStation in newlyMarkedStations)
            {
                markedStations.Add(newlyMarkedStation.Key, newlyMarkedStation.Value);
            }
        }

        private void ComputeRoundForStation(StationInfo station, WeekTimePoint startTime, Atomic<AtomicWeekTimePoint> earliestKnownTargetArrivalTime, ConcurrentBag<Connection2f> earliestKnownConnections, Position2f targetPos, ConcurrentDictionary<StationInfo, WeekTimePoint> newlyMarkedStations)
        {
            var nextDeparture = station.GetNextDepartureArrayBinarySearch(startTime);
            if (nextDeparture == null)
            {
                return;
            }

            if (WeekTimePoint.GetCorrectedDifference(startTime, nextDeparture) > _maxWaitingTime)
            {
                return;
            }

            var newStations = CheckRoute(station, nextDeparture, earliestKnownTargetArrivalTime, earliestKnownConnections, targetPos);
            foreach (var pair in newStations)
            {
                newlyMarkedStations.TryAdd(pair.Key, pair.Value);
            }
        }

        private Dictionary<StationInfo, WeekTimePoint> CheckRoute(StationInfo currentStationInfo, WeekTimePoint currentDeparture, Atomic<AtomicWeekTimePoint> earliestKnownTargetArrivalTime, ConcurrentBag<Connection2f> earliestKnownConnections, Position2f targetPos)
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
                if (nextTime >= earliestKnownTargetArrivalTime.Data())
                {
                    break;
                }

                var exitWalkingTime = TimeSpan.FromSeconds(nextStation.ExitPosition.DistanceTo(targetPos) / _walkingSpeed);
                if (exitWalkingTime < TimeSpan.FromMinutes(10))
                {
                    var arrivalAtTargetPos = nextTime + exitWalkingTime;
                    earliestKnownTargetArrivalTime.ReplaceOnCondition(a => arrivalAtTargetPos < a, new AtomicWeekTimePoint(arrivalAtTargetPos), () =>
                    {
                        var con = Connection2f.CreateWalkFromStation(nextStation, nextTime, targetPos, arrivalAtTargetPos);
                        earliestKnownConnections.Replace(c => c.TargetPos != null && c.TargetPos.DistanceTo(targetPos) < float.Epsilon, con, _lockObjConnections);
                    });
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
                    earliestKnownConnections.Replace(c => c.TargetStation == nextStation && c.TargetTime > nextTime, connection, _lockObjConnections);
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
                    if (arrivalTime >= earliestKnownTargetArrivalTime.Data())
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
                        earliestKnownConnections.Replace(c => c.TargetStation == otherStation && c.TargetTime > arrivalTime, connection, _lockObjConnections);
                        newlyMarkedStations.Add(otherStationInfo, arrivalTime);
                    }
                }
            }

            return newlyMarkedStations;
        }
    }
}
