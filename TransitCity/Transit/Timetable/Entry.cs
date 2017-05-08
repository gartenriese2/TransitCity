using Time;

namespace Transit.Timetable
{
    public class Entry
    {
        internal Entry(WeekTimePoint weekTimePoint, WeekTimePoint weekTimePointNextStation, Line line, Route route, TransferStation transferStation, Station station)
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

        internal Line Line { get; }

        internal Route Route { get; }

        internal TransferStation TransferStation { get; }

        internal Station Station { get; }
    }
}
