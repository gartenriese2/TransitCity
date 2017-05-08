using System;

namespace Utility.Units
{
    public class Area
    {
        private readonly double _squareMeters;

        internal Area(double squareMeters)
        {
            if (squareMeters < 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(squareMeters));
            }

            _squareMeters = squareMeters;
        }

        public double SquareMeters => _squareMeters;

        public double SquareKilometers => _squareMeters / 1000000.0;

        public override string ToString()
        {
            return $"{_squareMeters}m²";
        }

        public static Area operator +(Area a1, Area a2) => new Area(a1.SquareMeters + a2.SquareMeters);

        public static Area operator -(Area a1, Area a2) => new Area(a1.SquareMeters - a2.SquareMeters);

        public static Area operator *(Area a, double f) => new Area(a.SquareMeters * f);

        public static Area operator *(double f, Area a) => a * f;

        public static Area operator /(Area a, double f) => new Area(a.SquareMeters / f);

        public static bool operator <(Area a1, Area a2) => a1.SquareMeters < a2.SquareMeters;

        public static bool operator >(Area a1, Area a2) => a1.SquareMeters > a2.SquareMeters;

        public static bool operator <=(Area a1, Area a2) => a1.SquareMeters <= a2.SquareMeters;

        public static bool operator >=(Area a1, Area a2) => a1.SquareMeters >= a2.SquareMeters;

        public static Area FromSqaureMeters(double squareMeters) => new Area(squareMeters);

        public static Area FromSquareKilometers(double squareKilometers) => new Area(squareKilometers * 1000000.0);
    }
}
