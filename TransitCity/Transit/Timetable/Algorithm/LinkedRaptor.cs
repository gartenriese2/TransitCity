using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Time;
using Transit.Timetable.Managers;
using Utility.Extensions;

namespace Transit.Timetable.Algorithm
{
    public class LinkedRaptor<TPos> : SequentialRaptorBase<TPos> where TPos : IPosition
    {
        private readonly ITimetableManager<TPos, LinkedEntry<TPos>> _timetableManager;

        public LinkedRaptor(ITimetableManager<TPos, LinkedEntry<TPos>> manager, float walkingSpeed, TimeSpan maxWalkingTime, IReadOnlyCollection<TransferStation<TPos>> transferStations)
            : base(walkingSpeed, maxWalkingTime, transferStations)
        {
            _timetableManager = manager ?? throw new ArgumentNullException();
        }

        protected override void ComputeRound(TPos targetPos, List<Connection<TPos>> earliestKnownConnections, IDictionary<Station<TPos>, WeekTimePoint> markedStations, ref WeekTimePoint earliestKnownTargetArrivalTime)
        {
            var newlyMarkedStations = new Dictionary<Station<TPos>, WeekTimePoint>();

            foreach (var (station, startTime) in markedStations)
            {
                var firstDepartures = _timetableManager.GetDepartures(station, startTime, startTime + TimeSpan.FromHours(1)).GroupBy(e => e.Route).Select(g => g.First());
                foreach (var firstDepartureEntry in firstDepartures)
                {
                    var newStations = CheckRoute(firstDepartureEntry, ref earliestKnownTargetArrivalTime, earliestKnownConnections, targetPos);
                    foreach (var pair in newStations)
                    {
                        newlyMarkedStations.Add(pair.Key, pair.Value);
                    }
                }
            }

            markedStations.Clear();
            foreach (var newlyMarkedStation in newlyMarkedStations)
            {
                markedStations.Add(newlyMarkedStation.Key, newlyMarkedStation.Value);
            }
        }

        private Dictionary<Station<TPos>, WeekTimePoint> CheckRoute(LinkedEntry<TPos> entry, ref WeekTimePoint earliestKnownTargetArrivalTime, List<Connection<TPos>> earliestKnownConnections, TPos targetPos)
        {
            var newlyMarkedStations = new Dictionary<Station<TPos>, WeekTimePoint>();
            var nextEntries = _timetableManager.GetNextEntries(entry).ToList();

            foreach (var nextEntry in nextEntries)
            {
                var nextTime = nextEntry.WeekTimePoint;
                var nextStation = nextEntry.Station;
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
                        earliestKnownConnections[idx] = Connection<TPos>.CreateWalkFromStation(nextStation, nextTime, targetPos, arrivalAtTargetPos);
                    }
                }

                if (earliestKnownConnections.Any(c => c.TargetStation == nextStation && c.TargetTime <= nextTime))
                {
                    continue;
                }

                if (earliestKnownConnections.All(c => c.TargetStation != nextStation))
                {
                    var connection = Connection<TPos>.CreateRide(entry.Station, entry.WeekTimePoint, nextStation, nextTime, entry.Line);
                    earliestKnownConnections.Add(connection);
                    newlyMarkedStations.Add(nextStation, nextTime);
                }
                else if (earliestKnownConnections.Any(c => c.TargetStation == nextStation && c.TargetTime > nextTime))
                {
                    var connection = Connection<TPos>.CreateRide(entry.Station, entry.WeekTimePoint, nextStation, nextTime, entry.Line);
                    var idx = earliestKnownConnections.FindIndex(c => c.TargetStation == nextStation && c.TargetTime > nextTime);
                    earliestKnownConnections[idx] = connection;
                    newlyMarkedStations.Add(nextStation, nextTime);
                }

                var transferStation = GetTransferStation(nextStation);
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

                    var connection = Connection<TPos>.CreateTransfer(nextStation, nextTime, otherStation, arrivalTime);
                    if (earliestKnownConnections.All(c => c.TargetStation != otherStation))
                    {
                        earliestKnownConnections.Add(connection);
                        newlyMarkedStations.Add(otherStation, arrivalTime);
                    }
                    else if (earliestKnownConnections.Any(c => c.TargetStation == otherStation && c.TargetTime > arrivalTime))
                    {
                        var idx = earliestKnownConnections.FindIndex(c => c.TargetStation == otherStation && c.TargetTime > arrivalTime);
                        earliestKnownConnections[idx] = connection;
                        newlyMarkedStations.Add(otherStation, arrivalTime);
                    }
                }
            }

            return newlyMarkedStations;
        }
    }
}
