using System;

namespace Time
{
    public class WeekTimeSpan
    {
        public WeekTimeSpan(WeekTimePoint begin, WeekTimePoint end)
        {
            Begin = begin;
            End = end;
        }

        public WeekTimePoint Begin { get; }

        public WeekTimePoint End { get; }

        public override string ToString()
        {
            return $"{Begin} - {End}";
        }

        public TimeSpan GetTimeSpan()
        {
            return WeekTimePoint.GetCorrectedDifference(Begin, End);
        }

        public bool IsInside(WeekTimePoint wtp)
        {
            if (Begin <= End)
            {
                return wtp >= Begin && wtp <= End;
            }

            return wtp >= Begin || wtp <= End;
        }

        public bool Overlaps(WeekTimeSpan other)
        {
            if (Begin <= End && other.Begin <= other.End) // none go into next week;
            {
                return Begin <= other.End && other.Begin <= End;
            }

            if (other.Begin <= other.End) // this goes into next week
            {
                var endOfWeek = new WeekTimeSpan(Begin, WeekTimePoint.CreateLastPossibleWeekTimePoint());
                var beginOfWeek = new WeekTimeSpan(new WeekTimePoint(DayOfWeek.Monday), End);
                return endOfWeek.Overlaps(other) || beginOfWeek.Overlaps(other);
            }

            if (Begin <= End) // other goes into next week
            {
                var endOfWeek = new WeekTimeSpan(other.Begin, WeekTimePoint.CreateLastPossibleWeekTimePoint());
                var beginOfWeek = new WeekTimeSpan(new WeekTimePoint(DayOfWeek.Monday), other.End);
                return Overlaps(endOfWeek) || Overlaps(beginOfWeek);
            }

            return true; // Both go into next week => overlap
        }
    }
}
