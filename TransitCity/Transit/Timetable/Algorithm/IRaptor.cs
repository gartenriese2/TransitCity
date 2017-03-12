using System.Collections.Generic;
using Geometry;
using Time;

namespace Transit.Timetable.Algorithm
{
    public interface IRaptor<TPos> where TPos : IPosition
    {
        List<Connection<TPos>> Compute(TPos startPos, WeekTimePoint startTime, TPos targetPos);
    }
}
