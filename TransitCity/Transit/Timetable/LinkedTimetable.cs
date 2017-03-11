using System.Collections.Generic;
using Geometry;
using Table;
using Time;

namespace Transit.Timetable
{
    public class LinkedTimetable<TPos> : ITimetable<KeyValuePair<long, LinkedEntry<TPos>>> where TPos : IPosition
    {
        private readonly Table<Dictionary<long, LinkedEntry<TPos>>, KeyValuePair<long, LinkedEntry<TPos>>> _table = new Table<Dictionary<long, LinkedEntry<TPos>>, KeyValuePair<long, LinkedEntry<TPos>>>();

        public void AddEntry(KeyValuePair<long, LinkedEntry<TPos>> entry)
        {
            _table.AddEntry(entry);
        }

        public void AddEntry(long id, WeekTimePoint weekTimePoint, Line<TPos> line, Route<TPos> route, TransferStation<TPos> transferStation, Station<TPos> station, List<long> nextEntries)
        {
            var pair = new KeyValuePair<long, LinkedEntry<TPos>>(id, new LinkedEntry<TPos>(id, weekTimePoint, line, route, transferStation, station, nextEntries));
            _table.AddEntry(pair);
        }

        public IEnumerable<KeyValuePair<long, LinkedEntry<TPos>>> Query(IQuery<KeyValuePair<long, LinkedEntry<TPos>>> query)
        {
            return _table.Query(query);
        }
    }
}
