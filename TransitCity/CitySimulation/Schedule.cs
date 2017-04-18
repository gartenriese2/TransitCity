using System;
using System.Collections.Generic;
using Time;

namespace CitySimulation
{
    public class Schedule
    {
        private readonly List<WeekTimeSpan> _weekTimeSpans;

        public Schedule(List<WeekTimeSpan> weekTimeSpans)
        {
            _weekTimeSpans = weekTimeSpans ?? throw new ArgumentNullException(nameof(weekTimeSpans));
        }

        public IEnumerable<WeekTimeSpan> WeekTimeSpans => _weekTimeSpans;
    }
}
