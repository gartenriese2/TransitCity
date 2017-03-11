using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using PathFinding.Network;
using Time;
using Transit.Timetable.Queries;

namespace Transit.Timetable.Managers
{
    public class LinkedTimetableManager<TPos> : ITimetableManager<TPos, LinkedEntry<TPos>> where TPos : IPosition
    {
        private readonly LinkedTimetable<TPos> _timetable = new LinkedTimetable<TPos>();

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
                    _timetable.AddEntry(currentId, currentTime, line, route, GetTransferStation(stationA, transferStations), stationA, nextEntries);
                    currentTime += cost.TimeSpan;
                }

                _timetable.AddEntry(idQueue.Dequeue(), currentTime, line, route, GetTransferStation(route.Stations.Last(), transferStations), route.Stations.Last(), null);
            }
        }

        public IEnumerable<LinkedEntry<TPos>> GetDepartures(Station<TPos> station, WeekTimePoint from, WeekTimePoint to)
        {
            var query = _timetable.Query(new DeparturesQueryLinkedEntry<TPos>(station, from, to));
            return query.Select(p => p.Value);
        }

        public IEnumerable<LinkedEntry<TPos>> GetDepartures(TransferStation<TPos> station, WeekTimePoint from, WeekTimePoint to)
        {
            var query = _timetable.Query(new DeparturesQueryLinkedEntry<TPos>(station, from, to));
            return query.Select(p => p.Value);
        }

        public IEnumerable<LinkedEntry<TPos>> GetNextEntries(LinkedEntry<TPos> entry)
        {
            var query = _timetable.Query(new LinkedEntryQuery<TPos>(entry));
            return query.Select(p => p.Value);
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
