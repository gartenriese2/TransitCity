using Geometry;
using Time;

namespace Transit.Timetable
{
    public class Entry<TPos> where TPos : IPosition
    {
        internal Entry(WeekTimePoint weekTimePoint, WeekTimePoint weekTimePointNextStation, Line<TPos> line, Route<TPos> route, TransferStation<TPos> transferStation, Station<TPos> station)
        {
            WeekTimePoint = weekTimePoint;
            WeekTimePointNextStation = weekTimePointNextStation;
            Line = line;
            Route = route;
            TransferStation = transferStation;
            Station = station;
        }

        public WeekTimePoint WeekTimePoint { get; }

        internal WeekTimePoint WeekTimePointNextStation { get; }

        internal Line<TPos> Line { get; }

        internal Route<TPos> Route { get; }

        internal TransferStation<TPos> TransferStation { get; }

        internal Station<TPos> Station { get; }
    }
}
