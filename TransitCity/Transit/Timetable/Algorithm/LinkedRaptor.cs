using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Time;
using Transit.Timetable.Managers;
using Utility.Extensions;

namespace Transit.Timetable.Algorithm
{
    public class LinkedRaptor<TPos> : IRaptor<TPos> where TPos : IPosition
    {
        private const int NumRounds = 5;

        private readonly ITimetableManager<TPos, LinkedEntry<TPos>> _timetableManager;

        public LinkedRaptor(ITimetableManager<TPos, LinkedEntry<TPos>> manager)
        {
            _timetableManager = manager ?? throw new ArgumentNullException();
        }

        public List<Connection<TPos>> Compute(TPos startPos, WeekTimePoint startTime, TPos targetPos, IReadOnlyCollection<TransferStation<TPos>> transferStations)
        {
            var maxWalkingTime = TimeSpan.FromMinutes(10);
            var earliestKnownTargetArrivalTime = startTime + TimeSpan.FromSeconds(startPos.DistanceTo(targetPos) / 2.2f);
            var earliestConnections = new List<Connection<TPos>>
            {
                Connection<TPos>.CreateWalk(startPos, startTime, targetPos, earliestKnownTargetArrivalTime)
            };

            var markedStations = new Dictionary<Station<TPos>, WeekTimePoint>();
            foreach (var enterStation in transferStations.SelectMany(ts => ts.Stations))
            {
                var walkingTime = TimeSpan.FromSeconds(startPos.DistanceTo(enterStation.Position) / 2.2f);
                if (walkingTime > maxWalkingTime)
                {
                    continue;
                }

                var arrivalTime = startTime + walkingTime;
                earliestConnections.Add(Connection<TPos>.CreateWalkToStation(startPos, startTime, enterStation, arrivalTime));
                markedStations.Add(enterStation, arrivalTime);
            }

            Compute(markedStations, targetPos, transferStations, ref earliestKnownTargetArrivalTime, earliestConnections);

            var con = earliestConnections.Find(c => c.TargetPos.DistanceTo(targetPos) < float.Epsilon);

            var connectionList = new List<Connection<TPos>> { con };

            while (con.Type == Connection<TPos>.TypeEnum.Ride || con.Type == Connection<TPos>.TypeEnum.Transfer || con.Type == Connection<TPos>.TypeEnum.WalkFromStation)
            {
                con = earliestConnections.Find(c => c.TargetStation == con.SourceStation);
                connectionList.Insert(0, con);
            }

            return connectionList;
        }

        private void Compute(IDictionary<Station<TPos>, WeekTimePoint> markedStations, TPos targetPos, IReadOnlyCollection<TransferStation<TPos>> transferStations, ref WeekTimePoint earliestKnownTargetArrivalTime, List<Connection<TPos>> earliestKnownConnections)
        {
            for (var k = 1; markedStations.Count > 0 && k <= NumRounds; ++k)
            {
                ComputeRound(targetPos, transferStations, earliestKnownConnections, markedStations, ref earliestKnownTargetArrivalTime);
            }
        }

        private void ComputeRound(TPos targetPos, IReadOnlyCollection<TransferStation<TPos>> transferStations, List<Connection<TPos>> earliestKnownConnections, IDictionary<Station<TPos>, WeekTimePoint> markedStations, ref WeekTimePoint earliestKnownTargetArrivalTime)
        {
            var newlyMarkedStations = new Dictionary<Station<TPos>, WeekTimePoint>();

            foreach (var (station, startTime) in markedStations)
            {
                var firstDepartures = _timetableManager.GetDepartures(station, startTime, startTime + TimeSpan.FromHours(1)).GroupBy(e => e.Route).Select(g => g.First());
                foreach (var firstDepartureEntry in firstDepartures)
                {
                    var newStations = CheckRoute(firstDepartureEntry, ref earliestKnownTargetArrivalTime, earliestKnownConnections, targetPos, transferStations);
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

        private static TransferStation<TPos> GetTransferStation(Station<TPos> station, IEnumerable<TransferStation<TPos>> transferStations)
        {
            var collection = transferStations.Where(ts => ts.Stations.Any(s => s == station)).ToList();
            if (collection.Count != 1)
            {
                throw new InvalidOperationException();
            }

            return collection[0];
        }

        private Dictionary<Station<TPos>, WeekTimePoint> CheckRoute(LinkedEntry<TPos> entry, ref WeekTimePoint earliestKnownTargetArrivalTime, List<Connection<TPos>> earliestKnownConnections, TPos targetPos, IReadOnlyCollection<TransferStation<TPos>> transferStations)
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

                var exitWalkingTime = TimeSpan.FromSeconds(nextStation.ExitPosition.DistanceTo(targetPos) / 2.2f);
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

                var transferStation = GetTransferStation(nextStation, transferStations);
                var otherStations = transferStation.Stations.Where(s => s != nextStation);
                foreach (var otherStation in otherStations)
                {
                    const float exitTime = 10f;
                    var transferTime = nextStation.ExitPosition.DistanceTo(otherStation.EntryPosition) / 2.2f;
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
