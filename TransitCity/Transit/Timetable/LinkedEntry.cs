using System.Collections.Generic;
using Geometry;
using Time;

namespace Transit.Timetable
{
    public class LinkedEntry<TPos> where TPos : IPosition
    {
        internal LinkedEntry(long id, WeekTimePoint weekTimePoint, Line<TPos> line, Route<TPos> route, TransferStation<TPos> transferStation, Station<TPos> station, List<long> nextEntries = null)
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

        internal Line<TPos> Line { get; }

        internal Route<TPos> Route { get; }

        internal TransferStation<TPos> TransferStation { get; }

        internal Station<TPos> Station { get; }

        internal List<long> NextEntries { get; } = new List<long>();
    }
}
