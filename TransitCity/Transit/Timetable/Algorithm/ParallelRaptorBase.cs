using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geometry;
using Time;
using Utility.Extensions;
using Utility.Threading;

namespace Transit.Timetable.Algorithm
{
    public class ParallelRaptorBase<TPos> : IRaptor<TPos> where TPos : IPosition
    {
        private const int NumRounds = 5;
        protected readonly object _lockObjConnections = new object();
        protected readonly float _walkingSpeed;

        protected ParallelRaptorBase(float walkingSpeed)
        {
            _walkingSpeed = walkingSpeed;
        }

        public List<Connection<TPos>> Compute(TPos startPos, WeekTimePoint startTime, TPos targetPos, IReadOnlyCollection<TransferStation<TPos>> transferStations)
        {
            var maxWalkingTime = TimeSpan.FromMinutes(10);
            var earliestKnownTargetArrivalTime = new Atomic<AtomicWeekTimePoint>(new AtomicWeekTimePoint(startTime + TimeSpan.FromSeconds(startPos.DistanceTo(targetPos) / _walkingSpeed)));
            var earliestConnections = new ConcurrentBag<Connection<TPos>>
            {
                Connection<TPos>.CreateWalk(startPos, startTime, targetPos, earliestKnownTargetArrivalTime.Data())
            };

            var markedStations = new ConcurrentDictionary<Station<TPos>, WeekTimePoint>();
            foreach (var enterStation in transferStations.SelectMany(ts => ts.Stations))
            {
                var walkingTime = TimeSpan.FromSeconds(startPos.DistanceTo(enterStation.Position) / _walkingSpeed);
                if (walkingTime > maxWalkingTime)
                {
                    continue;
                }

                var arrivalTime = startTime + walkingTime;
                earliestConnections.Add(Connection<TPos>.CreateWalkToStation(startPos, startTime, enterStation, arrivalTime));
                markedStations.TryAdd(enterStation, arrivalTime);
            }

            Compute(markedStations, targetPos, transferStations, earliestKnownTargetArrivalTime, earliestConnections);

            var con = earliestConnections.Find(c => c.TargetPos != null && c.TargetPos.DistanceTo(targetPos) < float.Epsilon, _lockObjConnections);

            var connectionList = new List<Connection<TPos>> { con };

            while (con.Type == Connection<TPos>.TypeEnum.Ride || con.Type == Connection<TPos>.TypeEnum.Transfer || con.Type == Connection<TPos>.TypeEnum.WalkFromStation)
            {
                con = earliestConnections.Find(c => c.TargetStation == con.SourceStation, _lockObjConnections);
                connectionList.Insert(0, con);
            }

            return connectionList;
        }

        protected static TransferStation<TPos> GetTransferStation(Station<TPos> station, IEnumerable<TransferStation<TPos>> transferStations)
        {
            var collection = transferStations.Where(ts => ts.Stations.Any(s => s == station)).ToList();
            if (collection.Count != 1)
            {
                throw new InvalidOperationException();
            }

            return collection[0];
        }

        protected virtual void ComputeRoundForStation(Station<TPos> station, WeekTimePoint startTime, Atomic<AtomicWeekTimePoint> earliestKnownTargetArrivalTime, ConcurrentBag<Connection<TPos>> earliestKnownConnections, TPos targetPos, IReadOnlyCollection<TransferStation<TPos>> transferStations, ConcurrentDictionary<Station<TPos>, WeekTimePoint> newlyMarkedStations) { }

        private void Compute(IDictionary<Station<TPos>, WeekTimePoint> markedStations, TPos targetPos, IReadOnlyCollection<TransferStation<TPos>> transferStations, Atomic<AtomicWeekTimePoint> earliestKnownTargetArrivalTime, ConcurrentBag<Connection<TPos>> earliestKnownConnections)
        {
            for (var k = 1; markedStations.Count > 0 && k <= NumRounds; ++k)
            {
                ComputeRound(targetPos, transferStations, earliestKnownConnections, markedStations, earliestKnownTargetArrivalTime);
            }
        }

        private void ComputeRound(TPos targetPos, IReadOnlyCollection<TransferStation<TPos>> transferStations, ConcurrentBag<Connection<TPos>> earliestKnownConnections, IDictionary<Station<TPos>, WeekTimePoint> markedStations, Atomic<AtomicWeekTimePoint> earliestKnownTargetArrivalTime)
        {
            var newlyMarkedStations = new ConcurrentDictionary<Station<TPos>, WeekTimePoint>();

            var taskList = new List<Task>();
            foreach (var (station, startTime) in markedStations)
            {
                taskList.Add(Task.Factory.StartNew(() => ComputeRoundForStation(station, startTime, earliestKnownTargetArrivalTime, earliestKnownConnections, targetPos, transferStations, newlyMarkedStations)));
            }

            Task.WaitAll(taskList.ToArray());

            markedStations.Clear();
            foreach (var newlyMarkedStation in newlyMarkedStations)
            {
                markedStations.Add(newlyMarkedStation.Key, newlyMarkedStation.Value);
            }
        }
    }
}
