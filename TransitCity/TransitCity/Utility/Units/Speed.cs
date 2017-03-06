namespace TransitCity.Utility.Units
{
    using System;

    public class Speed : IComparable
    {
        //---------------------------------------------------------------------
        // Constructors
        //---------------------------------------------------------------------
        public Speed(float ms = 0)
        {
            Ms = ms;
        }

        //---------------------------------------------------------------------
        // Properties
        //---------------------------------------------------------------------
        public float Ms { get; set; }

        public float Kmh
        {
            get { return Ms * 3.6f; }
            set { Ms = value / 3.6f; }
        }

        //---------------------------------------------------------------------
        // Methods
        //---------------------------------------------------------------------
        public static explicit operator float(Speed v)
        {
            return v.Ms;
        }

        public static bool operator >=(Speed v1, Speed v2)
        {
            return (float)v1 >= (float)v2;
        }

        public static bool operator <=(Speed v1, Speed v2)
        {
            return (float)v1 <= (float)v2;
        }

        public static bool operator >(Speed v1, Speed v2)
        {
            return (float)v1 > (float)v2;
        }

        public static bool operator <(Speed v1, Speed v2)
        {
            return (float)v1 < (float)v2;
        }

        public static Speed operator +(Speed v1, Speed v2)
        {
            return new Speed(v1.Ms + v2.Ms);
        }

        public static Speed operator -(Speed v1, Speed v2)
        {
            return new Speed(v1.Ms - v2.Ms);
        }

        public static Distance operator *(Speed v, Time t)
        {
            return new Distance((float)t.Seconds * v.Ms);
        }

        public override string ToString()
        {
            return Ms + " meters per second";
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            var otherSpeed = obj as Speed;
            if (otherSpeed != null)
            {
                return Ms.CompareTo(otherSpeed.Ms);
            }

            throw new ArgumentException("Object is not a Speed");
        }
    }
}