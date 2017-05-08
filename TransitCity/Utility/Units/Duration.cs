using System;

namespace Utility.Units
{
    public class Duration
    {
        private readonly double _seconds;

        internal Duration(double seconds)
        {
            _seconds = seconds;
        }

        public double Seconds => _seconds;

        public double Minutes => _seconds / 60.0;

        public double Hours => _seconds / 3600.0;

        public override string ToString()
        {
            return $"{_seconds}s";
        }

        public static implicit operator TimeSpan(Duration d) => TimeSpan.FromSeconds(d.Seconds);

        public static Duration operator +(Duration t1, Duration t2) => new Duration(t1.Seconds + t2.Seconds);

        public static Duration operator -(Duration t1, Duration t2) => new Duration(t1.Seconds - t2.Seconds);

        public static Duration operator *(Duration t, double f) => new Duration(t.Seconds * f);

        public static Duration operator *(double f, Duration t) => t * f;

        public static Duration operator /(Duration t, double f) => new Duration(t.Seconds / f);

        public static TimeSquared operator *(Duration t1, Duration t2) => new TimeSquared(t1.Seconds * t2.Seconds);

        public static Distance operator *(Duration t, Speed s) => s * t;

        public static Speed operator *(Duration t, Acceleration a) => a * t;

        public static bool operator <(Duration t1, Duration t2) => t1.Seconds < t2.Seconds;

        public static bool operator >(Duration t1, Duration t2) => t1.Seconds > t2.Seconds;

        public static bool operator <=(Duration t1, Duration t2) => t1.Seconds <= t2.Seconds;

        public static bool operator >=(Duration t1, Duration t2) => t1.Seconds >= t2.Seconds;

        public static Duration FromSeconds(double seconds) => new Duration(seconds);

        public static Duration FromMinutes(double minutes) => new Duration(minutes * 60.0);

        public static Duration FromHours(double hours) => new Duration(hours * 3600.0);
    }

    public class TimeSquared
    {
        private readonly double _secondsSquared;

        internal TimeSquared(double secondsSquared)
        {
            _secondsSquared = secondsSquared;
        }

        public double SecondsSquared => _secondsSquared;

        public Duration SquareRoot => Duration.FromSeconds(Math.Sqrt(SecondsSquared));

        public static TimeSquared operator +(TimeSquared tt1, TimeSquared tt2) => new TimeSquared(tt1.SecondsSquared + tt2.SecondsSquared);

        public static TimeSquared operator -(TimeSquared tt1, TimeSquared tt2) => new TimeSquared(tt1.SecondsSquared - tt2.SecondsSquared);

        public static TimeSquared operator *(TimeSquared tt, double f) => new TimeSquared(tt.SecondsSquared * f);

        public static TimeSquared operator *(double f, TimeSquared tt) => tt * f;

        public static TimeSquared operator /(TimeSquared tt, double f) => new TimeSquared(tt.SecondsSquared / f);

        public static bool operator <(TimeSquared tt1, TimeSquared tt2) => tt1.SecondsSquared < tt2.SecondsSquared;

        public static bool operator >(TimeSquared tt1, TimeSquared tt2) => tt1.SecondsSquared > tt2.SecondsSquared;

        public static bool operator <=(TimeSquared tt1, TimeSquared tt2) => tt1.SecondsSquared <= tt2.SecondsSquared;

        public static bool operator >=(TimeSquared tt1, TimeSquared tt2) => tt1.SecondsSquared >= tt2.SecondsSquared;

        public static TimeSquared FromSecondsSquared(double secondsSquared) => new TimeSquared(secondsSquared);
    }
}
