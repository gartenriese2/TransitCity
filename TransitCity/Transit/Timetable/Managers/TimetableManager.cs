using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using PathFinding.Network;
using Time;
using Transit.Timetable.Queries;

namespace Transit.Timetable.Managers
{
    public class TimetableManager<TPos> : ITimetableManager<TPos, Entry<TPos>> where TPos : IPosition
    {
        private readonly Timetable<TPos> _timetable = new Timetable<TPos>();

        public void AddRoute(Line<TPos> line, Route<TPos> route, WeekTimeCollection timeCollection, List<TransferStation<TPos>> transferStations, Func<Station<TPos>, Station<TPos>, TimeEdgeCost> transitCostFunc)
        {
            var stations = route.Stations.ToList();
            foreach (var weekTime in timeCollection.SortedWeekTimePoints)
            {
                var currentTime = weekTime;
                for (var i = 0; i < route.Stations.Count() - 1; ++i)
                {
                    var stationA = stations[i];
                    var stationB = stations[i + 1];
                    var cost = transitCostFunc(stationA, stationB);
                    var nextTime = currentTime + cost.TimeSpan;
                    _timetable.AddEntry(currentTime, nextTime, line, route, GetTransferStation(stationA, transferStations), stationA);
                    currentTime = nextTime;
                }

                _timetable.AddEntry(currentTime, null, line, route, GetTransferStation(route.Stations.Last(), transferStations), route.Stations.Last());
            }
        }

        public IEnumerable<Entry<TPos>> GetEntriesInRange(WeekTimePoint from, WeekTimePoint to)
        {
            return _timetable.Query(new TimePointQuery<TPos>(from, to));
        }

        public IEnumerable<Entry<TPos>> GetDepartures(Station<TPos> station, WeekTimePoint from, WeekTimePoint to)
        {
            return _timetable.Query(new DeparturesQueryEntry<TPos>(station, from, to));
        }

        public IEnumerable<Entry<TPos>> GetDepartures(TransferStation<TPos> station, WeekTimePoint from, WeekTimePoint to)
        {
            return _timetable.Query(new DeparturesQueryEntry<TPos>(station, from, to));
        }

        public IEnumerable<Entry<TPos>> GetNextEntries(Entry<TPos> entry)
        {
            var nextEntries = new List<Entry<TPos>>();
            var nextTime = entry.WeekTimePointNextStation;
            var nextStation = entry.Route.GetNextStation(entry.Station);
            while (nextTime != null)
            {
                var nextEntry = GetEntriesInRange(nextTime, nextTime).Where(e => e.Station == nextStation && e.Route == entry.Route).ElementAt(0);
                nextEntries.Add(nextEntry);
                nextTime = nextEntry.WeekTimePointNextStation;
                nextStation = nextEntry.Route.GetNextStation(nextEntry.Station);
            }
            
            return nextEntries;
        }

        private TransferStation<TPos> GetTransferStation(Station<TPos> station, IEnumerable<TransferStation<TPos>> transferStations)
        {
            var collection = transferStations.Where(ts => ts.Stations.Any(s => s == station)).ToList();
            if (collection.Count != 1)
            {
                throw new InvalidOperationException();
            }

            return collection[0];
        }
    }
}
