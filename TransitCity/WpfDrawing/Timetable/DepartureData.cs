using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfDrawing.Timetable
{
    public class DepartureData
    {
        public DepartureData(int hour, List<int> weekday, List<int> friday, List<int> saturday, List<int> sunday)
        {
            if (hour < 0 || hour > 23)
            {
                throw new ArgumentOutOfRangeException(nameof(hour));
            }

            if (weekday.Any(t => t < 0 || t > 59))
            {
                throw new ArgumentOutOfRangeException(nameof(weekday));
            }

            if (friday.Any(t => t < 0 || t > 59))
            {
                throw new ArgumentOutOfRangeException(nameof(friday));
            }

            if (saturday.Any(t => t < 0 || t > 59))
            {
                throw new ArgumentOutOfRangeException(nameof(saturday));
            }

            if (sunday.Any(t => t < 0 || t > 59))
            {
                throw new ArgumentOutOfRangeException(nameof(sunday));
            }

            Hour = hour;
            MinutesWeekday = weekday.OrderBy(t => t);
            MinutesFriday = friday.OrderBy(t => t);
            MinutesSaturday = saturday.OrderBy(t => t);
            MinutesSunday = sunday.OrderBy(t => t);
        }

        public int Hour { get; }

        public IEnumerable<int> MinutesWeekday { get; }

        public IEnumerable<int> MinutesFriday { get; }

        public IEnumerable<int> MinutesSaturday { get; }

        public IEnumerable<int> MinutesSunday { get; }
    }
}
