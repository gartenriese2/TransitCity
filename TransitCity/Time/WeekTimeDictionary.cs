using System;
using System.Collections.Generic;
using System.Linq;
using Utility.Extensions;

namespace Time
{
    public class WeekTimeDictionary<T> where T : WeekTimeSpan
    {
        private readonly Dictionary<WeekTimeSpan, List<T>> _dictionary = new Dictionary<WeekTimeSpan, List<T>>();

        public WeekTimeDictionary(Granularity granularity, IEnumerable<T> collection = null)
        {
            var stepHours = TimeSpan.FromDays(7).TotalHours / (int) granularity;
            for (var i = 0; i < (int)granularity; ++i)
            {
                var begin = new WeekTimePoint(TimeSpan.FromHours(i * stepHours));
                var lastValue = i == (int) granularity - 1;
                var end = new WeekTimePoint(lastValue ? TimeSpan.Zero : TimeSpan.FromHours((i + 1) * stepHours));
                _dictionary.Add(new WeekTimeSpan(begin, end), new List<T>());
            }

            if (collection != null)
            {
                AddRange(collection);
            }
        }

        public enum Granularity
        {
            Day = 7,
            HalfDay = 14,
            QuarterDay = 28,
            TwoHour = 84,
            Hour = 168,
            HalfHour = 336,
            TwentyMinutes = 504,
            TenMinutes = 1008,
            FiveMinutes = 2016,
            Minute = 10080
        }

        public void Add(T entry)
        {
            foreach (var (wts, list) in _dictionary)
            {
                if (wts.Overlaps(entry))
                {
                    list.Add(entry);
                }
            }
        }

        public void AddRange(IEnumerable<T> range)
        {
            foreach (var entry in range)
            {
                foreach (var (wts, list) in _dictionary)
                {
                    if (wts.Overlaps(entry))
                    {
                        list.Add(entry);
                    }
                }
            }
        }

        public List<T> this[WeekTimePoint wtp] => _dictionary.First(pair => pair.Key.IsInside(wtp)).Value;
    }
}
