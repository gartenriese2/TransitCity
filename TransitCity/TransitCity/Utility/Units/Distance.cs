namespace TransitCity.Utility.Units
{
    using System;

    public class Distance : IComparable
    {
        //---------------------------------------------------------------------
        // Constructors
        //---------------------------------------------------------------------
        public Distance(float meters)
        {
            Meters = meters;
        }

        //---------------------------------------------------------------------
        // Properties
        //---------------------------------------------------------------------
        public float Meters { get; set; }

        public float KiloMeters
        {
            get { return Meters / 1000f; }
            set { Meters = value * 1000f; }
        }

        //---------------------------------------------------------------------
        // Methods
        //---------------------------------------------------------------------
        public static explicit operator float(Distance d)
        {
            return d.Meters;
        }

        public static bool operator >=(Distance d1, Distance d2)
        {
            return (float)d1 >= (float)d2;
        }

        public static bool operator <=(Distance d1, Distance d2)
        {
            return (float)d1 <= (float)d2;
        }

        public static bool operator >(Distance d1, Distance d2)
        {
            return (float)d1 > (float)d2;
        }

        public static bool operator <(Distance d1, Distance d2)
        {
            return (float)d1 < (float)d2;
        }

        public static Distance operator +(Distance d1, Distance d2)
        {
            return new Distance(d1.Meters + d2.Meters);
        }

        public static Distance operator -(Distance d1, Distance d2)
        {
            return new Distance(d1.Meters - d2.Meters);
        }

        public static Speed operator /(Distance d, Time t)
        {
            return new Speed(d.Meters / (float)t.Seconds);
        }

        public static Time operator /(Distance d, Speed v)
        {
            return Time.FromSeconds(d.Meters / v.Ms);
        }

        public override string ToString()
        {
            return Meters + " meters";
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            var otherDistance = obj as Distance;
            if (otherDistance != null)
            {
                return Meters.CompareTo(otherDistance.Meters);
            }

            throw new ArgumentException("Object is not a Distance");
        }
    }
}