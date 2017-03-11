using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using PathFinding.Network;
using Time;

namespace Transit.Timetable.Managers
{
    public class DictionaryTimetableManager<TPos> : ITimetableManager<TPos, LinkedEntry<TPos>> where TPos : IPosition
    {
        private readonly Dictionary<long, LinkedEntry<TPos>> _timetable = new Dictionary<long, LinkedEntry<TPos>>();

        private long _id = 0;

        public void AddRoute(Line<TPos> line, Route<TPos> route, WeekTimeCollection timeCollection, List<TransferStation<TPos>> transferStations, Func<Station<TPos>, Station<TPos>, TimeEdgeCost> transitCostFunc)
        {
            var stations = route.Stations.ToList();
            foreach (var weekTime in timeCollection)
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
                    var entry = new LinkedEntry<TPos>(currentId, currentTime, line, route, GetTransferStation(stationA, transferStations), stationA, nextEntries);
                    _timetable.Add(currentId, entry);
                    currentTime += cost.TimeSpan;
                }

                var lastId = idQueue.Dequeue();
                var lastEntry = new LinkedEntry<TPos>(lastId, currentTime, line, route, GetTransferStation(route.Stations.Last(), transferStations), route.Stations.Last(), null);
                _timetable.Add(lastId, lastEntry);
            }
        }

        public IEnumerable<LinkedEntry<TPos>> GetDepartures(Station<TPos> station, WeekTimePoint from, WeekTimePoint to)
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

        public IEnumerable<LinkedEntry<TPos>> GetDepartures(TransferStation<TPos> station, WeekTimePoint from, WeekTimePoint to)
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

        public IEnumerable<LinkedEntry<TPos>> GetNextEntries(LinkedEntry<TPos> entry)
        {
            return entry.NextEntries.Select(id => _timetable[id]);
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
