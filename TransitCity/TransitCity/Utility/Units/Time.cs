namespace TransitCity.Utility.Units
{
    using System;

    public class Time : IComparable
    {

        //---------------------------------------------------------------------
        // Constructors
        //---------------------------------------------------------------------
        public Time(Time other) : this(other.Ticks)
        {
        }

        private Time(long ticks)
        {
            TimeSpan = new TimeSpan(ticks);
        }

        //---------------------------------------------------------------------
        // Properties
        //---------------------------------------------------------------------
        public static Time Zero => new Time(0);

        public double Milliseconds => TimeSpan.TotalMilliseconds;

        public double Seconds => TimeSpan.TotalSeconds;

        public double Minutes => TimeSpan.TotalMinutes;

        public long Ticks => TimeSpan.Ticks;

        private TimeSpan TimeSpan { get; }

        //---------------------------------------------------------------------
        // Methods
        //---------------------------------------------------------------------
        public static Time FromTicks(long ticks)
        {
            return new Time(ticks);
        }

        public static Time FromMilliseconds(float ms)
        {
            long ticks = (long)(TimeSpan.TicksPerMillisecond * ms);
            return new Time(ticks);
        }

        public static Time FromSeconds(float s)
        {
            long ticks = (long)(TimeSpan.TicksPerSecond * s);
            return new Time(ticks);
        }

        public static Time FromMinutes(float m)
        {
            long ticks = (long)(TimeSpan.TicksPerMinute * m);
            return new Time(ticks);
        }

        public static Time FromHours(float h)
        {
            long ticks = (long)(TimeSpan.TicksPerHour * h);
            return new Time(ticks);
        }

        public static bool operator >=(Time t1, Time t2)
        {
            return t1.TimeSpan >= t2.TimeSpan;
        }

        public static bool operator <=(Time t1, Time t2)
        {
            return t1.TimeSpan <= t2.TimeSpan;
        }

        public static bool operator >(Time t1, Time t2)
        {
            return t1.TimeSpan > t2.TimeSpan;
        }

        public static bool operator <(Time t1, Time t2)
        {
            return t1.TimeSpan < t2.TimeSpan;
        }

        public static Time operator +(Time t1, Time t2)
        {
            return new Time(t1.Ticks + t2.Ticks);
        }

        public static Time operator -(Time t1, Time t2)
        {
            return new Time(t1.Ticks - t2.Ticks);
        }

        public static Time operator *(Time t1, float t2)
        {
            return new Time((long)(t1.Ticks * t2));
        }

        public static Distance operator *(Time t, Speed v)
        {
            return new Distance((float)t.Seconds * v.Ms);
        }

        public override string ToString()
        {
            return Seconds.ToString() + " seconds";
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            var otherTime = obj as Time;
            if (otherTime != null)
            {
                return Ticks.CompareTo(otherTime.Ticks);
            }

            throw new ArgumentException("Object is not a Time");
        }
    }
}