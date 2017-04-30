using System;
using System.Collections.Generic;
using Geometry;
using Time;

namespace Transit.Timetable.Algorithm
{
    [Obsolete("Use RaptorWithDataManagerBase instead")]
    public class SequentialRaptorBase<TPos> : RaptorBase<TPos> where TPos : IPosition
    {
        protected SequentialRaptorBase(float walkingSpeed, TimeSpan maxWalkingTime, IReadOnlyCollection<TransferStation<TPos>> transferStations)
            : base(walkingSpeed, maxWalkingTime, transferStations)
        {
        }

        public override List<Connection<TPos>> Compute(TPos sourcePos, WeekTimePoint startTime, TPos targetPos)
        {
            var earliestKnownTargetArrivalTime = startTime + TimeSpan.FromSeconds(sourcePos.DistanceTo(targetPos) / _walkingSpeed);
            var earliestConnections = new List<Connection<TPos>>
            {
                Connection<TPos>.CreateWalk(sourcePos, startTime, targetPos, earliestKnownTargetArrivalTime)
            };

            var (markedStations, connections) = GetInitialMarkedStations(sourcePos, startTime);
            earliestConnections.AddRange(connections);

            Compute(markedStations, targetPos, ref earliestKnownTargetArrivalTime, earliestConnections);

            return GetTravelPath(earliestConnections, targetPos);
        }

        protected virtual void ComputeRound(TPos targetPos, List<Connection<TPos>> earliestKnownConnections, IDictionary<Station<TPos>, WeekTimePoint> markedStations, ref WeekTimePoint earliestKnownTargetArrivalTime)
        {
            throw new NotImplementedException();
        }

        private void Compute(IDictionary<Station<TPos>, WeekTimePoint> markedStations, TPos targetPos, ref WeekTimePoint earliestKnownTargetArrivalTime, List<Connection<TPos>> earliestKnownConnections)
        {
            for (var k = 1; markedStations.Count > 0 && k <= NumRounds; ++k)
            {
                ComputeRound(targetPos, earliestKnownConnections, markedStations, ref earliestKnownTargetArrivalTime);
            }
        }
    }
}
