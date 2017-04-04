using System.Collections.Generic;
using Geometry;
using Time;

namespace Transit.Timetable.Algorithm
{
    public interface IRaptor<TPos> where TPos : IPosition
    {
        List<Connection<TPos>> Compute(TPos sourcePos, WeekTimePoint startTime, TPos targetPos);

        List<Connection<TPos>> ComputeReverse(TPos sourcePos, WeekTimePoint latestArrivalTime, TPos targetPos);
    }
}
