using System;

namespace Utility.Units
{
    public class Distance
    {
        private readonly float _meters;

        internal Distance(float meters)
        {
            if (meters < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(meters));
            }

            _meters = meters;
        } 

        public float Meters => _meters;

        public float Kilometers => _meters / 1000;

        public override string ToString()
        {
            return $"{_meters}m";
        }

        public static Distance operator +(Distance d1, Distance d2) => new Distance(d1.Meters + d2.Meters);

        public static Distance operator -(Distance d1, Distance d2) => new Distance(d1.Meters - d2.Meters);

        public static Distance operator *(Distance d, float f) => new Distance(d.Meters * f);

        public static Distance operator *(float f, Distance d) => d * f;

        public static Area operator *(Distance d1, Distance d2) => new Area(d1.Meters * d2.Meters);

        public static Distance operator /(Distance d, float f) => new Distance(d.Meters / f);

        public static Speed operator /(Distance d, Duration t) => new Speed(d.Meters / t.Seconds);

        public static Duration operator /(Distance d, Speed s) => new Duration(d.Meters / s.MetersPerSecond);

        public static TimeSquared operator /(Distance d, Acceleration a) => new TimeSquared(d.Meters / a.MetersPerSecondSquared);

        public static bool operator <(Distance d1, Distance d2) => d1.Meters < d2.Meters;

        public static bool operator >(Distance d1, Distance d2) => d1.Meters > d2.Meters;

        public static bool operator <=(Distance d1, Distance d2) => d1.Meters <= d2.Meters;

        public static bool operator >=(Distance d1, Distance d2) => d1.Meters >= d2.Meters;

        public static Distance FromMeters(float meters) => new Distance(meters);

        public static Distance FromKilometers(float kilometers) => new Distance(kilometers * 1000);
    }
}
