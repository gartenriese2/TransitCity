using System;

namespace Time
{
    public class WeekTimePoint : IComparable, IComparable<WeekTimePoint>, IEquatable<WeekTimePoint>
    {
        public WeekTimePoint(TimeSpan timeSpan)
        {
            if (timeSpan.Days < 0 || timeSpan.Days > 6)
            {
                throw new ArgumentOutOfRangeException(nameof(timeSpan), timeSpan, "Days need to be between 0 and 6");
            }

            TimePoint = timeSpan;
        }

        public WeekTimePoint(byte day, byte hours = 0, byte minutes = 0, byte seconds = 0)
        {
            if (day > 6)
            {
                throw new ArgumentOutOfRangeException(nameof(day), day, "Days need to be between 0 and 6");
            }

            if (hours > 23)
            {
                throw new ArgumentOutOfRangeException(nameof(hours), hours, "Hours need to be between 0 and 23");
            }

            if (minutes > 59)
            {
                throw new ArgumentOutOfRangeException(nameof(minutes), minutes, "Minutes need to be between 0 and 59");
            }

            if (seconds > 59)
            {
                throw new ArgumentOutOfRangeException(nameof(seconds), seconds, "Seconds need to be between 0 and 59");
            }

            TimePoint = new TimeSpan(day, hours, minutes, seconds);
        }

        public WeekTimePoint(DayOfWeek day, byte hours = 0, byte minutes = 0, byte seconds = 0) : this((byte)(day == DayOfWeek.Sunday ? 6 : (int)day - 1), hours, minutes, seconds)
        {
        }

        public TimeSpan TimePoint { get; set; }

        public static bool operator ==(WeekTimePoint a, WeekTimePoint b) => a?.TimePoint == b?.TimePoint;

        public static bool operator !=(WeekTimePoint a, WeekTimePoint b) => a?.TimePoint != b?.TimePoint;

        public static bool operator <(WeekTimePoint a, WeekTimePoint b) => a?.TimePoint < b?.TimePoint;

        public static bool operator >(WeekTimePoint a, WeekTimePoint b) => a?.TimePoint > b?.TimePoint;

        public static bool operator <=(WeekTimePoint a, WeekTimePoint b) => a?.TimePoint <= b?.TimePoint;

        public static bool operator >=(WeekTimePoint a, WeekTimePoint b) => a?.TimePoint >= b?.TimePoint;

        public static WeekTimePoint operator +(WeekTimePoint wtp1, TimeSpan timespan)
        {
            var ts = wtp1.TimePoint + timespan;
            if (ts.Days > 6)
            {
                ts = new TimeSpan(ts.Days % 7, ts.Hours, ts.Minutes, ts.Seconds);
            }

            return new WeekTimePoint(ts);
        }

        public static WeekTimePoint operator -(WeekTimePoint wtp1, TimeSpan timespan)
        {
            var ts = wtp1.TimePoint - timespan;
            while (ts.TotalDays < 0)
            {
                ts = new TimeSpan(ts.Days + 7, ts.Hours, ts.Minutes, ts.Seconds);
            }

            return new WeekTimePoint(ts);
        }

        public static TimeSpan operator -(WeekTimePoint wtp1, WeekTimePoint wtp2)
        {
            return wtp1.TimePoint - wtp2.TimePoint;
        }

        public static TimeSpan GetCorrectedDifference(WeekTimePoint wtp1, WeekTimePoint wtp2)
        {
            if (wtp2 >= wtp1)
            {
                return wtp2 - wtp1;
            }

            return TimeSpan.FromDays(7) - (wtp1 - wtp2);
        }

        public static WeekTimePoint CreateLastPossibleWeekTimePoint() => new WeekTimePoint(TimeSpan.FromTicks(TimeSpan.FromDays(7).Ticks - 1));

        public override int GetHashCode()
        {
            return TimePoint.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((WeekTimePoint) obj);
        }

        public override string ToString()
        {
            return $"{GetDayOfWeek()} {TimePoint.ToString(@"hh\:mm\:ss")}";
        }

        public DayOfWeek GetDayOfWeek()
        {
            switch (TimePoint.Days)
            {
                case 0:
                    return DayOfWeek.Monday;
                case 1:
                    return DayOfWeek.Tuesday;
                case 2:
                    return DayOfWeek.Wednesday;
                case 3:
                    return DayOfWeek.Thursday;
                case 4:
                    return DayOfWeek.Friday;
                case 5:
                    return DayOfWeek.Saturday;
                case 6:
                    return DayOfWeek.Sunday;
                default:
                    throw new InvalidOperationException("Days must be between 0 and 6");
            }
        }

        public int CompareTo(WeekTimePoint other)
        {
            return TimePoint.CompareTo(other.TimePoint);
        }

        public bool Equals(WeekTimePoint other)
        {
            return TimePoint.Equals(other.TimePoint);
        }

        public int CompareTo(object obj)
        {
            if (!(obj is WeekTimePoint))
            {
                throw new ArgumentException();
            }

            return CompareTo((WeekTimePoint)obj);
        }
    }
}
