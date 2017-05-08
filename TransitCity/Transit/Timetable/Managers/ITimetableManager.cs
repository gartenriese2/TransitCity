using System;
using System.Collections.Generic;
using PathFinding.Network;
using Time;

namespace Transit.Timetable.Managers
{
    public interface ITimetableManager<TEntry>
    {
        void AddRoute(Line line, Route route, WeekTimeCollection timeCollection, List<TransferStation> transferStations, Func<Station, Station, TimeEdgeCost> transitCostFunc);

        IEnumerable<TEntry> GetDepartures(Station station, WeekTimePoint from, WeekTimePoint to);

        IEnumerable<TEntry> GetDepartures(TransferStation station, WeekTimePoint from, WeekTimePoint to);

        IEnumerable<TEntry> GetNextEntries(TEntry entry);
    }
}
