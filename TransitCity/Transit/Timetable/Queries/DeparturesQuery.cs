using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Table;
using Time;

namespace Transit.Timetable.Queries
{
    public class DeparturesQueryEntry : IQuery<Entry>
    {
        private readonly WeekTimePoint _startTimePoint;
        private readonly WeekTimePoint _endTimePoint;
        private readonly List<Station> _stations = new List<Station>();

        public DeparturesQueryEntry(Station station, WeekTimePoint startTimePoint, WeekTimePoint endTimePoint = null)
        {
            _startTimePoint = startTimePoint ?? throw new ArgumentNullException(nameof(startTimePoint));
            _endTimePoint = endTimePoint;
            _stations.Add(station);
        }

        public DeparturesQueryEntry(TransferStation station, WeekTimePoint startTimePoint, WeekTimePoint endTimePoint = null)
        {
            _startTimePoint = startTimePoint ?? throw new ArgumentNullException(nameof(startTimePoint));
            _endTimePoint = endTimePoint;
            _stations.AddRange(station.Stations);
        }

        public IEnumerable<Entry> Execute(IEnumerable<Entry> table)
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

    public class DeparturesQueryLinkedEntry : IQuery<KeyValuePair<long, LinkedEntry>>
    {
        private readonly WeekTimePoint _startTimePoint;
        private readonly WeekTimePoint _endTimePoint;
        private readonly List<Station> _stations = new List<Station>();

        public DeparturesQueryLinkedEntry(Station station, WeekTimePoint startTimePoint, WeekTimePoint endTimePoint = null)
        {
            _startTimePoint = startTimePoint ?? throw new ArgumentNullException(nameof(startTimePoint));
            _endTimePoint = endTimePoint;
            _stations.Add(station);
        }

        public DeparturesQueryLinkedEntry(TransferStation station, WeekTimePoint startTimePoint, WeekTimePoint endTimePoint = null)
        {
            _startTimePoint = startTimePoint ?? throw new ArgumentNullException(nameof(startTimePoint));
            _endTimePoint = endTimePoint;
            _stations.AddRange(station.Stations);
        }

        public IEnumerable<KeyValuePair<long, LinkedEntry>> Execute(IEnumerable<KeyValuePair<long, LinkedEntry>> table)
        {
            if (_endTimePoint == null || _startTimePoint <= _endTimePoint)
            {
                return
                    from entry in table
                    where entry.Value.WeekTimePoint >= _startTimePoint
                    where _endTimePoint == null || entry.Value.WeekTimePoint <= _endTimePoint
                    where _stations.Any(s => s == entry.Value.Station)
                    orderby entry.Value.WeekTimePoint ascending
                    select entry;
            }

            return
                from entry in table
                where entry.Value.WeekTimePoint >= _startTimePoint || entry.Value.WeekTimePoint <= _endTimePoint
                where _stations.Any(s => s == entry.Value.Station)
                orderby entry.Value.WeekTimePoint ascending
                select entry;
        }
    }
}
