using System.Collections.Generic;
using Time;

namespace Transit.Timetable
{
    public class LinkedEntry
    {
        internal LinkedEntry(long id, WeekTimePoint weekTimePoint, Line line, Route route, TransferStation transferStation, Station station, List<long> nextEntries = null)
        {
            Id = id;
            WeekTimePoint = weekTimePoint;
            Line = line;
            Route = route;
            TransferStation = transferStation;
            Station = station;
            if (nextEntries != null)
            {
                NextEntries = nextEntries;
            }
        }

        internal long Id { get; }

        public WeekTimePoint WeekTimePoint { get; }

        internal Line Line { get; }

        internal Route Route { get; }

        internal TransferStation TransferStation { get; }

        internal Station Station { get; }

        internal List<long> NextEntries { get; } = new List<long>();
    }
}
