using System.Collections.Generic;

namespace WpfDrawing.Timetable
{
    public class DepartureData
    {
        public int Hour { get; set; }

        public List<int> MinutesWeekday { get; } = new List<int>();

        public List<int> MinutesFriday { get; } = new List<int>();

        public List<int> MinutesSaturday { get; } = new List<int>();

        public List<int> MinutesSunday { get; } = new List<int>();
    }
}
