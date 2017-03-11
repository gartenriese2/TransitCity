﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Time;
using Transit.Timetable.Managers;
using Utility.Extensions;
using Utility.Threading;

namespace Transit.Timetable.Algorithm
{
    public class ParallelRaptor<TPos> : ParallelRaptorBase<TPos> where TPos : IPosition
    {
        private readonly ITimetableManager<TPos, Entry<TPos>> _timetableManager;

        public ParallelRaptor(ITimetableManager<TPos, Entry<TPos>> manager, float walkingSpeed)
            : base(walkingSpeed)
        {
            _timetableManager = manager ?? throw new ArgumentNullException();
        }

        protected override void ComputeRoundForStation(Station<TPos> station, WeekTimePoint startTime, Atomic<AtomicWeekTimePoint> earliestKnownTargetArrivalTime, ConcurrentBag<Connection<TPos>> earliestKnownConnections, TPos targetPos, IReadOnlyCollection<TransferStation<TPos>> transferStations, ConcurrentDictionary<Station<TPos>, WeekTimePoint> newlyMarkedStations)
        {
            var firstDepartures = _timetableManager.GetDepartures(station, startTime, startTime + TimeSpan.FromHours(1)).GroupBy(e => e.Route).Select(g => g.First());
            foreach (var firstDepartureEntry in firstDepartures)
            {
                var newStations = CheckRoute(firstDepartureEntry, earliestKnownTargetArrivalTime, earliestKnownConnections, targetPos, transferStations);
                foreach (var pair in newStations)
                {
                    newlyMarkedStations.TryAdd(pair.Key, pair.Value);
                }
            }
        }

        private Dictionary<Station<TPos>, WeekTimePoint> CheckRoute(Entry<TPos> entry, Atomic<AtomicWeekTimePoint> earliestKnownTargetArrivalTime, ConcurrentBag<Connection<TPos>> earliestKnownConnections, TPos targetPos, IReadOnlyCollection<TransferStation<TPos>> transferStations)
        {
            var newlyMarkedStations = new Dictionary<Station<TPos>, WeekTimePoint>();
            var nextEntries = _timetableManager.GetNextEntries(entry).ToList();

            foreach (var nextEntry in nextEntries)
            {
                var nextTime = nextEntry.WeekTimePoint;
                var nextStation = nextEntry.Station;
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
                        var con = Connection<TPos>.CreateWalkFromStation(nextStation, nextTime, targetPos, arrivalAtTargetPos);
                        earliestKnownConnections.Replace(c => c.TargetPos != null && c.TargetPos.DistanceTo(targetPos) < float.Epsilon, con, _lockObjConnections);
                    });
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
                    earliestKnownConnections.Replace(c => c.TargetStation == nextStation && c.TargetTime > nextTime, connection, _lockObjConnections);
                    newlyMarkedStations.Add(nextStation, nextTime);
                }

                var transferStation = GetTransferStation(nextStation, transferStations);
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

                    var connection = Connection<TPos>.CreateTransfer(nextStation, nextTime, otherStation, arrivalTime);
                    if (earliestKnownConnections.All(c => c.TargetStation != otherStation))
                    {
                        earliestKnownConnections.Add(connection);
                        newlyMarkedStations.Add(otherStation, arrivalTime);
                    }
                    else if (earliestKnownConnections.Any(c => c.TargetStation == otherStation && c.TargetTime > arrivalTime))
                    {
                        earliestKnownConnections.Replace(c => c.TargetStation == otherStation && c.TargetTime > arrivalTime, connection, _lockObjConnections);
                        newlyMarkedStations.Add(otherStation, arrivalTime);
                    }
                }
            }

            return newlyMarkedStations;
        }
    }
    
}
