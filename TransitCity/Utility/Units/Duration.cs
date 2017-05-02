using System;

namespace Utility.Units
{
    public class Duration
    {
        private readonly float _seconds;

        internal Duration(float seconds)
        {
            _seconds = seconds;
        }

        public float Seconds => _seconds;

        public float Minutes => _seconds / 60f;

        public float Hours => _seconds / 3600f;

        public override string ToString()
        {
            return $"{_seconds}s";
        }

        public static implicit operator TimeSpan(Duration d) => TimeSpan.FromSeconds(d.Seconds);

        public static Duration operator +(Duration t1, Duration t2) => new Duration(t1.Seconds + t2.Seconds);

        public static Duration operator -(Duration t1, Duration t2) => new Duration(t1.Seconds - t2.Seconds);

        public static Duration operator *(Duration t, float f) => new Duration(t.Seconds * f);

        public static Duration operator *(float f, Duration t) => t * f;

        public static Duration operator /(Duration t, float f) => new Duration(t.Seconds / f);

        public static TimeSquared operator *(Duration t1, Duration t2) => new TimeSquared(t1.Seconds * t2.Seconds);

        public static Distance operator *(Duration t, Speed s) => s * t;

        public static Speed operator *(Duration t, Acceleration a) => a * t;

        public static bool operator <(Duration t1, Duration t2) => t1.Seconds < t2.Seconds;

        public static bool operator >(Duration t1, Duration t2) => t1.Seconds > t2.Seconds;

        public static bool operator <=(Duration t1, Duration t2) => t1.Seconds <= t2.Seconds;

        public static bool operator >=(Duration t1, Duration t2) => t1.Seconds >= t2.Seconds;

        public static Duration FromSeconds(float seconds) => new Duration(seconds);

        public static Duration FromMinutes(float minutes) => new Duration(minutes * 60f);

        public static Duration FromHours(float hours) => new Duration(hours * 3600f);
    }

    public class TimeSquared
    {
        private readonly float _secondsSquared;

        internal TimeSquared(float secondsSquared)
        {
            _secondsSquared = secondsSquared;
        }

        public float SecondsSquared => _secondsSquared;

        public Duration SquareRoot => Duration.FromSeconds((float) Math.Sqrt(SecondsSquared));

        public static TimeSquared operator +(TimeSquared tt1, TimeSquared tt2) => new TimeSquared(tt1.SecondsSquared + tt2.SecondsSquared);

        public static TimeSquared operator -(TimeSquared tt1, TimeSquared tt2) => new TimeSquared(tt1.SecondsSquared - tt2.SecondsSquared);

        public static TimeSquared operator *(TimeSquared tt, float f) => new TimeSquared(tt.SecondsSquared * f);

        public static TimeSquared operator *(float f, TimeSquared tt) => tt * f;

        public static TimeSquared operator /(TimeSquared tt, float f) => new TimeSquared(tt.SecondsSquared / f);

        public static bool operator <(TimeSquared tt1, TimeSquared tt2) => tt1.SecondsSquared < tt2.SecondsSquared;

        public static bool operator >(TimeSquared tt1, TimeSquared tt2) => tt1.SecondsSquared > tt2.SecondsSquared;

        public static bool operator <=(TimeSquared tt1, TimeSquared tt2) => tt1.SecondsSquared <= tt2.SecondsSquared;

        public static bool operator >=(TimeSquared tt1, TimeSquared tt2) => tt1.SecondsSquared >= tt2.SecondsSquared;

        public static TimeSquared FromSecondsSquared(float secondsSquared) => new TimeSquared(secondsSquared);
    }
}
