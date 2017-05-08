namespace Utility.Units
{
    public class Speed
    {
        private readonly double _metersPerSecond;

        internal Speed(double metersPerSecond)
        {
            _metersPerSecond = metersPerSecond;
        }

        public double MetersPerSecond => _metersPerSecond;

        public double KilometersPerHour => _metersPerSecond * 3.6;

        public override string ToString()
        {
            return $"{_metersPerSecond}m/s";
        }

        public static Speed operator +(Speed s1, Speed s2) => new Speed(s1.MetersPerSecond + s2.MetersPerSecond);

        public static Speed operator -(Speed s1, Speed s2) => new Speed(s1.MetersPerSecond - s2.MetersPerSecond);

        public static Speed operator *(Speed s, double f) => new Speed(s.MetersPerSecond * f);

        public static Speed operator *(double f, Speed s) => s * f;

        public static Speed operator /(Speed s, double f) => new Speed(s.MetersPerSecond / f);

        public static Distance operator *(Speed s, Duration t) => new Distance(s.MetersPerSecond * t.Seconds);

        public static Duration operator /(Speed s, Acceleration a) => new Duration(s.MetersPerSecond / a.MetersPerSecondSquared);

        public static bool operator <(Speed s1, Speed s2) => s1.MetersPerSecond < s2.MetersPerSecond;

        public static bool operator >(Speed s1, Speed s2) => s1.MetersPerSecond > s2.MetersPerSecond;

        public static bool operator <=(Speed s1, Speed s2) => s1.MetersPerSecond <= s2.MetersPerSecond;

        public static bool operator >=(Speed s1, Speed s2) => s1.MetersPerSecond >= s2.MetersPerSecond;

        public static Speed FromMetersPerSecond(double metersPerSecond) => new Speed(metersPerSecond);

        public static Speed FromKilometersPerHour(double kilometersPerHour) => new Speed(kilometersPerHour / 3.6);
    }
}
