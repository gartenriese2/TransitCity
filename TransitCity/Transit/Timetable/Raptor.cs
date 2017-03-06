using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Time;

namespace Transit.Timetable
{
    public class Raptor<TPos> where TPos : IPosition
    {
        private const int NumRounds = 5;

        private readonly TimetableManager<TPos> _timetableManager;

        public Raptor(TimetableManager<TPos> manager)
        {
            _timetableManager = manager ?? throw new ArgumentNullException();
        }

        public void Compute(TransferStation<TPos> startStation, WeekTimePoint startTime, TransferStation<TPos> targetStation, List<TransferStation<TPos>> transferStations)
        {
            var earliestKnownArrivalTimes = new Dictionary<TransferStation<TPos>, WeekTimePoint>
            {
                [startStation] = startTime
            };
            
            var markedStations = new Dictionary<TransferStation<TPos>, WeekTimePoint>
            {
                [startStation] = startTime
            };

            WeekTimePoint earliestKnownTargetArrivalTime = null;
            for (var k = 1; markedStations.Count > 0 && k <= NumRounds; ++k)
            {
                ComputeRound(targetStation, transferStations, earliestKnownArrivalTimes, markedStations, ref earliestKnownTargetArrivalTime);
            }
        }

        public WeekTimePoint Compute(Station<TPos> startStation, WeekTimePoint startTime, TransferStation<TPos> targetStation, List<TransferStation<TPos>> transferStations)
        {
            var earliestKnownArrivalTimes = new Dictionary<Station<TPos>, WeekTimePoint>
            {
                [startStation] = startTime
            };

            var markedStations = new Dictionary<Station<TPos>, WeekTimePoint>
            {
                [startStation] = startTime
            };

            WeekTimePoint earliestKnownTargetArrivalTime = null;
            for (var k = 1; markedStations.Count > 0 && k <= NumRounds; ++k)
            {
                ComputeRound(targetStation, transferStations, earliestKnownArrivalTimes, markedStations, ref earliestKnownTargetArrivalTime);
            }

            return earliestKnownTargetArrivalTime;
        }

        public void Compute(Station<TPos> startStation, WeekTimePoint startTime, TPos targetPos, List<TransferStation<TPos>> transferStations, ref WeekTimePoint earliestKnownTargetArrivalTime, List<Connection<TPos>> earliestKnownConnections)
        {
            var markedStations = new Dictionary<Station<TPos>, WeekTimePoint>
            {
                [startStation] = startTime
            };

            for (var k = 1; markedStations.Count > 0 && k <= NumRounds; ++k)
            {
                ComputeRound(targetPos, transferStations, earliestKnownConnections, markedStations, ref earliestKnownTargetArrivalTime);
            }
        }

        public WeekTimePoint Compute(TPos startPos, WeekTimePoint startTime, TransferStation<TPos> targetStation, List<TransferStation<TPos>> transferStations)
        {
            var maxWalkingTime = TimeSpan.FromMinutes(10);
            WeekTimePoint earliestKnownTargetArrivalTime = null;
            foreach (var station in transferStations.SelectMany(ts => ts.Stations))
            {
                var walkingTime = TimeSpan.FromSeconds(startPos.DistanceTo(station.Position) / 2.2f);
                if (walkingTime > maxWalkingTime)
                {
                    continue;
                }

                var arrivalTime = startTime + walkingTime;
                var earliest = Compute(station, arrivalTime, targetStation, transferStations);
                if (earliestKnownTargetArrivalTime == null || earliest < earliestKnownTargetArrivalTime)
                {
                    earliestKnownTargetArrivalTime = earliest;
                }
            }

            return earliestKnownTargetArrivalTime;
        }

        public List<Connection<TPos>> Compute(TPos startPos, WeekTimePoint startTime, TPos targetPos, List<TransferStation<TPos>> transferStations)
        {
            var maxWalkingTime = TimeSpan.FromMinutes(10);
            var earliestKnownTargetArrivalTime = startTime + TimeSpan.FromSeconds(startPos.DistanceTo(targetPos) / 2.2f);
            var earliestConnections = new List<Connection<TPos>>
            {
                Connection<TPos>.CreateWalk(startPos, startTime, targetPos, earliestKnownTargetArrivalTime)
            };

            foreach (var enterStation in transferStations.SelectMany(ts => ts.Stations))
            {
                var walkingTime = TimeSpan.FromSeconds(startPos.DistanceTo(enterStation.Position) / 2.2f);
                if (walkingTime > maxWalkingTime)
                {
                    continue;
                }

                var arrivalTime = startTime + walkingTime;
                earliestConnections.Add(Connection<TPos>.CreateWalkToStation(startPos, startTime, enterStation, arrivalTime));
                Compute(enterStation, arrivalTime, targetPos, transferStations, ref earliestKnownTargetArrivalTime, earliestConnections);
            }

            var con = earliestConnections.Find(c => c.TargetPos.DistanceTo(targetPos) < float.Epsilon);

            var connectionList = new List<Connection<TPos>> {con};

            while (con.Type == Connection<TPos>.TypeEnum.Ride || con.Type == Connection<TPos>.TypeEnum.Transfer || con.Type == Connection<TPos>.TypeEnum.WalkFromStation)
            {
                con = earliestConnections.Find(c => c.TargetStation == con.SourceStation);
                connectionList.Insert(0, con);
            }

            return connectionList;
        }

        private void ComputeRound(TransferStation<TPos> targetStation, IReadOnlyCollection<TransferStation<TPos>> transferStations, IDictionary<TransferStation<TPos>, WeekTimePoint> earliestKnownArrivalTimes, IDictionary<TransferStation<TPos>, WeekTimePoint> markedStations, ref WeekTimePoint earliestKnownTargetArrivalTime)
        {
            var newlyMarkedStations = new Dictionary<TransferStation<TPos>, WeekTimePoint>();

            foreach (var (stations, startTime) in markedStations.Select(p => new KeyValuePair<IEnumerable<Station<TPos>>, WeekTimePoint>(p.Key.Stations, p.Value)))
            {
                foreach (var station in stations)
                {
                    var firstDepartures = _timetableManager.GetDepartures(station, startTime, startTime + TimeSpan.FromHours(1)).GroupBy(e => e.Route).Select(g => g.First());
                    foreach (var firstDepartureEntry in firstDepartures)
                    {
                        var newStations = CheckRoute(firstDepartureEntry, ref earliestKnownTargetArrivalTime, earliestKnownArrivalTimes, targetStation, transferStations);
                        foreach (var pair in newStations)
                        {
                            newlyMarkedStations.Add(pair.Key, pair.Value);
                        }
                    }
                }
            }

            markedStations.Clear();
            foreach (var newlyMarkedStation in newlyMarkedStations)
            {
                markedStations.Add(newlyMarkedStation.Key, newlyMarkedStation.Value);
            }
        }

        private void ComputeRound(TransferStation<TPos> targetStation, IReadOnlyCollection<TransferStation<TPos>> transferStations, IDictionary<Station<TPos>, WeekTimePoint> earliestKnownArrivalTimes, IDictionary<Station<TPos>, WeekTimePoint> markedStations, ref WeekTimePoint earliestKnownTargetArrivalTime)
        {
            var newlyMarkedStations = new Dictionary<Station<TPos>, WeekTimePoint>();

            foreach (var (station, startTime) in markedStations)
            {
                var firstDepartures = _timetableManager.GetDepartures(station, startTime, startTime + TimeSpan.FromHours(1)).GroupBy(e => e.Route).Select(g => g.First());
                foreach (var firstDepartureEntry in firstDepartures)
                {
                    var newStations = CheckRoute(firstDepartureEntry, ref earliestKnownTargetArrivalTime, earliestKnownArrivalTimes, targetStation, transferStations);
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

        private Dictionary<TransferStation<TPos>, WeekTimePoint> CheckRoute(Entry<TPos> entry, ref WeekTimePoint earliestKnownTargetArrivalTime, IDictionary<TransferStation<TPos>, WeekTimePoint> earliestKnownArrivalTimes, TransferStation<TPos> targetStation, IReadOnlyCollection<TransferStation<TPos>> transferStations)
        {
            var newlyMarkedStations = new Dictionary<TransferStation<TPos>, WeekTimePoint>();
            var nextTime = entry.WeekTimePointNextStation;
            var nextStation = entry.Route.GetNextStation(entry.Station);
            while (nextTime != null && (earliestKnownTargetArrivalTime == null || nextTime < earliestKnownTargetArrivalTime))
            {
                var nextEntries = _timetableManager.GetEntriesInRange(nextTime, nextTime).Where(e => e.Station == nextStation && e.Route == entry.Route).ToList();
                if (nextEntries.Count != 1)
                {
                    throw new InvalidOperationException();
                }

                var nextEntry = nextEntries[0];
                var transferStation = GetTransferStation(nextStation, transferStations);

                if (transferStation == targetStation)
                {
                    if (earliestKnownTargetArrivalTime == null || nextTime < earliestKnownTargetArrivalTime)
                    {
                        earliestKnownTargetArrivalTime = nextTime;
                    }
                }

                if (!earliestKnownArrivalTimes.ContainsKey(transferStation))
                {
                    earliestKnownArrivalTimes.Add(transferStation, nextTime);
                    newlyMarkedStations.Add(transferStation, nextTime);
                }
                else if (earliestKnownArrivalTimes[transferStation] > nextTime)
                {
                    earliestKnownArrivalTimes[transferStation] = nextTime;
                    newlyMarkedStations.Add(transferStation, nextTime);
                }

                nextTime = nextEntry.WeekTimePointNextStation;
                nextStation = nextEntry.Route.GetNextStation(nextEntry.Station);
            }

            return newlyMarkedStations;
        }

        private Dictionary<Station<TPos>, WeekTimePoint> CheckRoute(Entry<TPos> entry, ref WeekTimePoint earliestKnownTargetArrivalTime, IDictionary<Station<TPos>, WeekTimePoint> earliestKnownArrivalTimes, TransferStation<TPos> targetStation, IReadOnlyCollection<TransferStation<TPos>> transferStations)
        {
            var newlyMarkedStations = new Dictionary<Station<TPos>, WeekTimePoint>();
            var nextTime = entry.WeekTimePointNextStation;
            var nextStation = entry.Route.GetNextStation(entry.Station);
            while (nextTime != null && (earliestKnownTargetArrivalTime == null || nextTime < earliestKnownTargetArrivalTime))
            {
                var nextEntries = _timetableManager.GetEntriesInRange(nextTime, nextTime).Where(e => e.Station == nextStation && e.Route == entry.Route).ToList();
                if (nextEntries.Count != 1)
                {
                    throw new InvalidOperationException();
                }

                var nextEntry = nextEntries[0];
                var transferStation = GetTransferStation(nextStation, transferStations);

                if (transferStation == targetStation)
                {
                    if (earliestKnownTargetArrivalTime == null || nextTime < earliestKnownTargetArrivalTime)
                    {
                        earliestKnownTargetArrivalTime = nextTime;
                    }
                }

                if (!earliestKnownArrivalTimes.ContainsKey(nextStation))
                {
                    earliestKnownArrivalTimes.Add(nextStation, nextTime);
                    newlyMarkedStations.Add(nextStation, nextTime);
                }
                else if (earliestKnownArrivalTimes[nextStation] > nextTime)
                {
                    earliestKnownArrivalTimes[nextStation] = nextTime;
                    newlyMarkedStations.Add(nextStation, nextTime);
                }

                var otherStations = transferStation.Stations.Where(s => s != nextStation);
                foreach (var otherStation in otherStations)
                {
                    const float exitTime = 10f;
                    var transferTime = nextStation.ExitPosition.DistanceTo(otherStation.EntryPosition) / 2.2f;
                    var timespan = TimeSpan.FromSeconds(transferTime + exitTime);
                    var arrivalTime = nextTime + timespan;
                    if (!earliestKnownArrivalTimes.ContainsKey(otherStation))
                    {
                        earliestKnownArrivalTimes.Add(otherStation, arrivalTime);
                        newlyMarkedStations.Add(otherStation, arrivalTime);
                    }
                    else if (earliestKnownArrivalTimes[otherStation] > arrivalTime)
                    {
                        earliestKnownArrivalTimes[otherStation] = arrivalTime;
                        newlyMarkedStations.Add(otherStation, arrivalTime);
                    }
                }

                nextTime = nextEntry.WeekTimePointNextStation;
                nextStation = nextEntry.Route.GetNextStation(nextEntry.Station);
            }

            return newlyMarkedStations;
        }

        private Dictionary<Station<TPos>, WeekTimePoint> CheckRoute(Entry<TPos> entry, ref WeekTimePoint earliestKnownTargetArrivalTime, List<Connection<TPos>> earliestKnownConnections, TPos targetPos, IReadOnlyCollection<TransferStation<TPos>> transferStations)
        {
            var newlyMarkedStations = new Dictionary<Station<TPos>, WeekTimePoint>();
            var nextTime = entry.WeekTimePointNextStation;
            var nextStation = entry.Route.GetNextStation(entry.Station);
            
            while (nextTime != null && nextTime < earliestKnownTargetArrivalTime)
            {
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

                var nextEntries = _timetableManager.GetEntriesInRange(nextTime, nextTime).Where(e => e.Station == nextStation && e.Route == entry.Route).ToList();
                if (nextEntries.Count != 1)
                {
                    throw new InvalidOperationException();
                }

                var nextEntry = nextEntries[0];

                if (earliestKnownConnections.Any(c => c.TargetStation == nextStation && c.TargetTime <= nextTime))
                {
                    nextTime = nextEntry.WeekTimePointNextStation;
                    nextStation = nextEntry.Route.GetNextStation(nextEntry.Station);
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

                nextTime = nextEntry.WeekTimePointNextStation;
                nextStation = nextEntry.Route.GetNextStation(nextEntry.Station);
            }

            return newlyMarkedStations;
        }
    }

    internal static class DeconstructExtensions
    {
        public static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> tuple, out T1 key, out T2 value)
        {
            key = tuple.Key;
            value = tuple.Value;
        }
    }
}
