using System;
using System.Collections.Generic;
using System.Linq;
using Table;
using Time;

namespace Transit.Timetable.Queries
{
    public class TimePointQuery : IQuery<Entry>
    {
        private readonly WeekTimePoint _startTimePoint;
        private readonly WeekTimePoint _endTimePoint;

        public TimePointQuery(WeekTimePoint startTimePoint, WeekTimePoint endTimePoint = null)
        {
            _startTimePoint = startTimePoint ?? throw new ArgumentNullException(nameof(startTimePoint));
            _endTimePoint = endTimePoint;
        }

        public IEnumerable<Entry> Execute(IEnumerable<Entry> table)
        {
            return
                from entry in table
                where entry.WeekTimePoint >= _startTimePoint
                where _endTimePoint == null || entry.WeekTimePoint <= _endTimePoint
                orderby entry.WeekTimePoint ascending
                select entry;
        }
    }
}
