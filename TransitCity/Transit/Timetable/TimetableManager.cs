using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using PathFinding.Network;
using Time;

namespace Transit.Timetable
{
    public class TimetableManager<TPos> where TPos : IPosition
    {
        private readonly Timetable<TPos> _timetable = new Timetable<TPos>();

        public void AddRoute(Line<TPos> line, Route<TPos> route, WeekTimeCollection timeCollection, List<TransferStation<TPos>> transferStations, Func<Station<TPos>, Station<TPos>, TimeEdgeCost> transitCostFunc)
        {
            var stations = route.Stations.ToList();
            foreach (var weekTime in timeCollection)
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
            return _timetable.Query(new DeparturesQuery<TPos>(station, from, to));
        }

        public IEnumerable<Entry<TPos>> GetDepartures(TransferStation<TPos> station, WeekTimePoint from, WeekTimePoint to)
        {
            return _timetable.Query(new DeparturesQuery<TPos>(station, from, to));
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
