namespace Utility.Units
{
    public class Acceleration
    {
        private readonly double _metersPerSecondsSquared;

        internal Acceleration(double metersPerSecondSquared)
        {
            _metersPerSecondsSquared = metersPerSecondSquared;
        }

        public double MetersPerSecondSquared => _metersPerSecondsSquared;

        public override string ToString()
        {
            return $"{_metersPerSecondsSquared}m/s²";
        }

        public static Acceleration operator +(Acceleration a1, Acceleration a2) => new Acceleration(a1.MetersPerSecondSquared + a2.MetersPerSecondSquared);

        public static Acceleration operator -(Acceleration a1, Acceleration a2) => new Acceleration(a1.MetersPerSecondSquared - a2.MetersPerSecondSquared);

        public static Acceleration operator *(Acceleration a, double f) => new Acceleration(a.MetersPerSecondSquared * f);

        public static Acceleration operator *(double f, Acceleration a) => a * f;

        public static Acceleration operator /(Acceleration a, double f) => new Acceleration(a.MetersPerSecondSquared / f);

        public static Speed operator *(Acceleration a, Duration t) => new Speed(a.MetersPerSecondSquared * t.Seconds);

        public static Distance operator *(Acceleration a, TimeSquared tt) => new Distance(a.MetersPerSecondSquared * tt.SecondsSquared);

        public static bool operator <(Acceleration a1, Acceleration a2) => a1.MetersPerSecondSquared < a2.MetersPerSecondSquared;

        public static bool operator >(Acceleration a1, Acceleration a2) => a1.MetersPerSecondSquared > a2.MetersPerSecondSquared;

        public static bool operator <=(Acceleration a1, Acceleration a2) => a1.MetersPerSecondSquared <= a2.MetersPerSecondSquared;

        public static bool operator >=(Acceleration a1, Acceleration a2) => a1.MetersPerSecondSquared >= a2.MetersPerSecondSquared;

        public static Acceleration FromMetersPerSecondSquared(double metersPerSecondSquared) => new Acceleration(metersPerSecondSquared);
    }
}
