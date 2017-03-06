using System.Collections.Generic;
using Geometry;
using Table;
using Time;

namespace Transit.Timetable
{
    public class Timetable<P> where P : IPosition
    {
        private readonly Table<Entry<P>> _table = new Table<Entry<P>>();

        public void AddEntry(WeekTimePoint weekTimePoint, WeekTimePoint weekTimePointNextStation, Line<P> line, Route<P> route, TransferStation<P> transferStation, Station<P> station)
        {
            _table.AddEntry(new Entry<P>(weekTimePoint, weekTimePointNextStation, line, route, transferStation, station));
        }

        public IEnumerable<Entry<P>> Query(IQuery<Entry<P>> query)
        {
            return _table.Query(query);
        }
    }
}
