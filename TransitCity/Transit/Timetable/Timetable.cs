using System.Collections.Generic;
using Geometry;
using Table;
using Time;

namespace Transit.Timetable
{
    public class Timetable<TPos> : ITimetable<Entry<TPos>> where TPos : IPosition
    {
        private readonly Table<List<Entry<TPos>>, Entry<TPos>> _table = new Table<List<Entry<TPos>>, Entry<TPos>>();

        public void AddEntry(Entry<TPos> entry)
        {
            _table.AddEntry(entry);
        }

        public void AddEntry(WeekTimePoint weekTimePoint, WeekTimePoint weekTimePointNextStation, Line<TPos> line, Route<TPos> route, TransferStation<TPos> transferStation, Station<TPos> station)
        {
            _table.AddEntry(new Entry<TPos>(weekTimePoint, weekTimePointNextStation, line, route, transferStation, station));
        }

        public IEnumerable<Entry<TPos>> Query(IQuery<Entry<TPos>> query)
        {
            return _table.Query(query);
        }
    }
}
