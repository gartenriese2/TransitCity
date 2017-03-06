using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Time
{
    public class WeekTimeCollection : IEnumerable
    {
        private readonly List<WeekTimePoint> _weekTimePoints = new List<WeekTimePoint>();

        public WeekTimeCollection(TimeSpan startTimePoint, TimeSpan endTimePoint, TimeSpan frequency, IEnumerable<DayOfWeek> days)
        {
            var wtp = startTimePoint;
            while (wtp <= endTimePoint)
            {
                foreach (var dayOfWeek in days)
                {
                    _weekTimePoints.Add(new WeekTimePoint(dayOfWeek, (byte) wtp.Hours, (byte) wtp.Minutes, (byte) wtp.Seconds));
                }

                wtp += frequency;
            }
        }

        public IEnumerable<WeekTimePoint> WeekTimePoints => _weekTimePoints;

        public IEnumerable<WeekTimePoint> SortedWeekTimePoints => _weekTimePoints.OrderBy(point => point);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public WeekTimeEnumerator GetEnumerator() => new WeekTimeEnumerator(_weekTimePoints);
    }
}
