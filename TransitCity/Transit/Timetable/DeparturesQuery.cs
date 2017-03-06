using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Table;
using Time;

namespace Transit.Timetable
{
    public class DeparturesQuery<TPos> : IQuery<Entry<TPos>> where TPos : IPosition
    {
        private readonly WeekTimePoint _startTimePoint;
        private readonly WeekTimePoint _endTimePoint;
        private readonly List<Station<TPos>> _stations = new List<Station<TPos>>();

        public DeparturesQuery(Station<TPos> station, WeekTimePoint startTimePoint, WeekTimePoint endTimePoint = null)
        {
            _startTimePoint = startTimePoint ?? throw new ArgumentNullException(nameof(startTimePoint));
            _endTimePoint = endTimePoint;
            _stations.Add(station);
        }

        public DeparturesQuery(TransferStation<TPos> station, WeekTimePoint startTimePoint, WeekTimePoint endTimePoint = null)
        {
            _startTimePoint = startTimePoint ?? throw new ArgumentNullException(nameof(startTimePoint));
            _endTimePoint = endTimePoint;
            _stations.AddRange(station.Stations);
        }

        public IEnumerable<Entry<TPos>> Execute(IEnumerable<Entry<TPos>> table)
        {
            if (_endTimePoint == null || _startTimePoint <= _endTimePoint)
            {
                return
                    from entry in table
                    where entry.WeekTimePoint >= _startTimePoint
                    where _endTimePoint == null || entry.WeekTimePoint <= _endTimePoint
                    where _stations.Any(s => s == entry.Station)
                    orderby entry.WeekTimePoint ascending
                    select entry;
            }

            return
                from entry in table
                where entry.WeekTimePoint >= _startTimePoint || entry.WeekTimePoint <= _endTimePoint
                where _stations.Any(s => s == entry.Station)
                orderby entry.WeekTimePoint ascending
                select entry;
        }
    }
}
