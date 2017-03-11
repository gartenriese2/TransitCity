using System;
using System.Collections.Generic;
using Geometry;
using PathFinding.Network;
using Time;

namespace Transit.Timetable.Managers
{
    public interface ITimetableManager<TPos, TEntry> where TPos : IPosition
    {
        void AddRoute(Line<TPos> line, Route<TPos> route, WeekTimeCollection timeCollection, List<TransferStation<TPos>> transferStations, Func<Station<TPos>, Station<TPos>, TimeEdgeCost> transitCostFunc);

        IEnumerable<TEntry> GetDepartures(Station<TPos> station, WeekTimePoint from, WeekTimePoint to);

        IEnumerable<TEntry> GetDepartures(TransferStation<TPos> station, WeekTimePoint from, WeekTimePoint to);

        IEnumerable<TEntry> GetNextEntries(TEntry entry);
    }
}
