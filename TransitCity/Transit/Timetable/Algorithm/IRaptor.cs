using System.Collections.Generic;
using Geometry;
using Time;

namespace Transit.Timetable.Algorithm
{
    public interface IRaptor
    {
        List<Connection> Compute(Position2d sourcePos, WeekTimePoint startTime, Position2d targetPos);

        List<Connection> ComputeReverse(Position2d sourcePos, WeekTimePoint latestArrivalTime, Position2d targetPos);
    }
}
