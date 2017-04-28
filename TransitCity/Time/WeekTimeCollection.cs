using System;
using System.Collections.Generic;
using System.Linq;

namespace Time
{
    public class WeekTimeCollection
    {
        private readonly List<WeekTimePoint> _weekTimePoints = new List<WeekTimePoint>();
        private readonly List<WeekTimePoint> _sortedWeekTimePoints = new List<WeekTimePoint>();

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

            _sortedWeekTimePoints = new List<WeekTimePoint>(_weekTimePoints.OrderBy(p => p));
        }

        public WeekTimeCollection(WeekTimePoint startWeekTimePoint, WeekTimePoint endWeekTimePoint, TimeSpan frequency)
        {
            var wtp = startWeekTimePoint;
            WeekTimeSpan wts;
            do
            {
                _weekTimePoints.Add(wtp);
                wts = new WeekTimeSpan(wtp, wtp + frequency);
                wtp += frequency;
            } while (!wts.IsInside(endWeekTimePoint) || wts.End == endWeekTimePoint);

            _sortedWeekTimePoints = new List<WeekTimePoint>(_weekTimePoints.OrderBy(p => p));
        }

        public WeekTimeCollection(IEnumerable<WeekTimePoint> weekTimePoints)
        {
            _weekTimePoints.AddRange(weekTimePoints);
            _sortedWeekTimePoints = new List<WeekTimePoint>(_weekTimePoints.OrderBy(p => p));
        }

        public IEnumerable<WeekTimePoint> UnsortedWeekTimePoints => _weekTimePoints;

        public IEnumerable<WeekTimePoint> SortedWeekTimePoints => _sortedWeekTimePoints;

        public int Count => _weekTimePoints.Count;

        public void Add(WeekTimePoint wtp)
        {
            _weekTimePoints.Add(wtp);
            _sortedWeekTimePoints.Add(wtp);
            _sortedWeekTimePoints.Sort();
        }

        public void AddRange(IEnumerable<WeekTimePoint> range)
        {
            _weekTimePoints.AddRange(range);
            _sortedWeekTimePoints.AddRange(range);
            _sortedWeekTimePoints.Sort();
        }

        public void AddCollection(WeekTimeCollection collection)
        {
            _weekTimePoints.AddRange(collection.UnsortedWeekTimePoints);
            _sortedWeekTimePoints.AddRange(collection.UnsortedWeekTimePoints);
            _sortedWeekTimePoints.Sort();
        }
    }
}
