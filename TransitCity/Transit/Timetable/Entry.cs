using Geometry;
using Time;

namespace Transit.Timetable
{
    public class Entry<P> where P : IPosition
    {
        internal Entry(WeekTimePoint weekTimePoint, WeekTimePoint weekTimePointNextStation, Line<P> line, Route<P> route, TransferStation<P> transferStation, Station<P> station)
        {
            WeekTimePoint = weekTimePoint;
            WeekTimePointNextStation = weekTimePointNextStation;
            Line = line;
            Route = route;
            TransferStation = transferStation;
            Station = station;
        }

        internal WeekTimePoint WeekTimePoint { get; }

        internal WeekTimePoint WeekTimePointNextStation { get; }

        internal Line<P> Line { get; }

        internal Route<P> Route { get; }

        internal TransferStation<P> TransferStation { get; }

        internal Station<P> Station { get; }
    }
}
