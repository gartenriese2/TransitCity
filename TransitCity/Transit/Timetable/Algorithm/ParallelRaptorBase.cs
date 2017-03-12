using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Geometry;
using Time;
using Utility.Extensions;
using Utility.Threading;

namespace Transit.Timetable.Algorithm
{
    public class ParallelRaptorBase<TPos> : RaptorBase<TPos> where TPos : IPosition
    {
        protected readonly object _lockObjConnections = new object();

        protected ParallelRaptorBase(float walkingSpeed, TimeSpan maxWalkingTime, IReadOnlyCollection<TransferStation<TPos>> transferStations)
            : base(walkingSpeed, maxWalkingTime, transferStations)
        {
        }

        public override List<Connection<TPos>> Compute(TPos startPos, WeekTimePoint startTime, TPos targetPos)
        {
            var earliestKnownTargetArrivalTime = new Atomic<AtomicWeekTimePoint>(new AtomicWeekTimePoint(startTime + TimeSpan.FromSeconds(startPos.DistanceTo(targetPos) / _walkingSpeed)));
            var earliestConnections = new ConcurrentBag<Connection<TPos>>
            {
                Connection<TPos>.CreateWalk(startPos, startTime, targetPos, earliestKnownTargetArrivalTime.Data())
            };

            var (markedStations, connections) = GetInitialMarkedStations(startPos, startTime);
            connections.ForEach(c => earliestConnections.Add(c));

            Compute(markedStations, targetPos, earliestKnownTargetArrivalTime, earliestConnections);

            return GetTravelPath(earliestConnections, targetPos);
        }

        protected virtual void ComputeRoundForStation(Station<TPos> station, WeekTimePoint startTime, Atomic<AtomicWeekTimePoint> earliestKnownTargetArrivalTime, ConcurrentBag<Connection<TPos>> earliestKnownConnections, TPos targetPos, ConcurrentDictionary<Station<TPos>, WeekTimePoint> newlyMarkedStations)
        {
            throw new NotImplementedException();
        }

        private void Compute(IDictionary<Station<TPos>, WeekTimePoint> markedStations, TPos targetPos, Atomic<AtomicWeekTimePoint> earliestKnownTargetArrivalTime, ConcurrentBag<Connection<TPos>> earliestKnownConnections)
        {
            for (var k = 1; markedStations.Count > 0 && k <= NumRounds; ++k)
            {
                ComputeRound(targetPos, earliestKnownConnections, markedStations, earliestKnownTargetArrivalTime);
            }
        }

        private void ComputeRound(TPos targetPos, ConcurrentBag<Connection<TPos>> earliestKnownConnections, IDictionary<Station<TPos>, WeekTimePoint> markedStations, Atomic<AtomicWeekTimePoint> earliestKnownTargetArrivalTime)
        {
            var newlyMarkedStations = new ConcurrentDictionary<Station<TPos>, WeekTimePoint>();

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
    }
}
