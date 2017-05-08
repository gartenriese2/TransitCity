using System;
using System.Collections.Generic;
using System.Linq;
using PathFinding.Network;
using Time;

namespace Transit.Timetable.Managers
{
    public class DictionaryTimetableManager : ITimetableManager<LinkedEntry>
    {
        private readonly Dictionary<long, LinkedEntry> _timetable = new Dictionary<long, LinkedEntry>();

        private long _id = 0;

        public void AddRoute(Line line, Route route, WeekTimeCollection timeCollection, List<TransferStation> transferStations, Func<Station, Station, TimeEdgeCost> transitCostFunc)
        {
            var stations = route.Stations.ToList();
            foreach (var weekTime in timeCollection.SortedWeekTimePoints)
            {
                var currentTime = weekTime;
                var idQueue = new Queue<long>();
                foreach (var station in route.Stations)
                {
                    idQueue.Enqueue(++_id);
                }

                for (var i = 0; i < route.Stations.Count() - 1; ++i)
                {
                    var stationA = stations[i];
                    var stationB = stations[i + 1];
                    var cost = transitCostFunc(stationA, stationB);
                    var currentId = idQueue.Dequeue();
                    var nextEntries = idQueue.ToList();
                    var entry = new LinkedEntry(currentId, currentTime, line, route, GetTransferStation(stationA, transferStations), stationA, nextEntries);
                    _timetable.Add(currentId, entry);
                    currentTime += cost.TimeSpan;
                }

                var lastId = idQueue.Dequeue();
                var lastEntry = new LinkedEntry(lastId, currentTime, line, route, GetTransferStation(route.Stations.Last(), transferStations), route.Stations.Last());
                _timetable.Add(lastId, lastEntry);
            }
        }

        public IEnumerable<LinkedEntry> GetDepartures(Station station, WeekTimePoint from, WeekTimePoint to)
        {
            if (from == to)
            {
                return _timetable.Values.Where(entry => entry.Station == station && entry.WeekTimePoint == from);
            }

            if (from < to)
            {
                return _timetable.Values.Where(entry => entry.Station == station && entry.WeekTimePoint >= from && entry.WeekTimePoint <= to);
            }

            return _timetable.Values.Where(entry => entry.Station == station && (entry.WeekTimePoint >= from || entry.WeekTimePoint <= to));
        }

        public IEnumerable<LinkedEntry> GetDepartures(TransferStation station, WeekTimePoint from, WeekTimePoint to)
        {
            if (from == to)
            {
                return _timetable.Values.Where(entry => entry.TransferStation == station && entry.WeekTimePoint == from);
            }

            if (from < to)
            {
                return _timetable.Values.Where(entry => entry.TransferStation == station && entry.WeekTimePoint >= from && entry.WeekTimePoint <= to);
            }

            return _timetable.Values.Where(entry => entry.TransferStation == station && (entry.WeekTimePoint >= from || entry.WeekTimePoint <= to));
        }

        public IEnumerable<LinkedEntry> GetNextEntries(LinkedEntry entry)
        {
            return entry.NextEntries.Select(id => _timetable[id]);
        }

        private TransferStation GetTransferStation(Station station, IEnumerable<TransferStation> transferStations)
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
