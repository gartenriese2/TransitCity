using System;

namespace Utility.Units
{
    public class Area
    {
        private readonly float _squareMeters;

        internal Area(float squareMeters)
        {
            if (squareMeters < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(squareMeters));
            }

            _squareMeters = squareMeters;
        }

        public float SquareMeters => _squareMeters;

        public float SquareKilometers => _squareMeters / 1000000f;

        public static Area operator +(Area a1, Area a2) => new Area(a1.SquareMeters + a2.SquareMeters);

        public static Area operator -(Area a1, Area a2) => new Area(a1.SquareMeters - a2.SquareMeters);

        public static Area operator *(Area a, float f) => new Area(a.SquareMeters * f);

        public static Area operator *(float f, Area a) => a * f;

        public static Area operator /(Area a, float f) => new Area(a.SquareMeters / f);

        public static bool operator <(Area a1, Area a2) => a1.SquareMeters < a2.SquareMeters;

        public static bool operator >(Area a1, Area a2) => a1.SquareMeters > a2.SquareMeters;

        public static bool operator <=(Area a1, Area a2) => a1.SquareMeters <= a2.SquareMeters;

        public static bool operator >=(Area a1, Area a2) => a1.SquareMeters >= a2.SquareMeters;

        public static Area FromSqaureMeters(float squareMeters) => new Area(squareMeters);

        public static Area FromSquareKilometers(float squareKilometers) => new Area(squareKilometers * 1000000);
    }
}
