using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Time
{
    public class WeekTimeCollection : IEnumerable
    {
        private readonly List<WeekTimePoint> _weekTimePoints = new List<WeekTimePoint>();

        public WeekTimeCollection() { }

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

            _weekTimePoints.Sort();
        }

        public WeekTimeCollection(WeekTimePoint startWeekTimePoint, WeekTimePoint endWeekTimePoint, TimeSpan frequency)
        {
            var wtp = startWeekTimePoint;
            while (wtp <= endWeekTimePoint)
            {
                _weekTimePoints.Add(wtp);
                wtp += frequency;
            }

            _weekTimePoints.Sort();
        }

        public WeekTimeCollection(IEnumerable<WeekTimePoint> weekTimePoints)
        {
            _weekTimePoints.AddRange(weekTimePoints.OrderBy(wtp => wtp));
        }

        public WeekTimePoint this[int key] => _weekTimePoints[key];

        public IEnumerable<WeekTimePoint> SortedWeekTimePoints => _weekTimePoints;

        public int Count => _weekTimePoints.Count;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public WeekTimeEnumerator GetEnumerator() => new WeekTimeEnumerator(_weekTimePoints);

        public int IndexOf(WeekTimePoint wtp)
        {
            return _weekTimePoints.IndexOf(wtp);
        }

        public void Add(WeekTimePoint wtp)
        {
            _weekTimePoints.Add(wtp);
            _weekTimePoints.Sort();
        }

        public void AddRange(IEnumerable<WeekTimePoint> range)
        {
            _weekTimePoints.AddRange(range);
            _weekTimePoints.Sort();
        }

        public void AddCollection(WeekTimeCollection collection)
        {
            _weekTimePoints.AddRange(collection.SortedWeekTimePoints);
            _weekTimePoints.Sort();
        }
    }
}
