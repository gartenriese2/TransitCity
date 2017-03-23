namespace Utility.Units
{
    public class Speed
    {
        private readonly float _metersPerSecond;

        internal Speed(float metersPerSecond)
        {
            _metersPerSecond = metersPerSecond;
        }

        public float MetersPerSecond => _metersPerSecond;

        public float KilometersPerHour => _metersPerSecond * 3.6f;

        public static Speed operator +(Speed s1, Speed s2) => new Speed(s1.MetersPerSecond + s2.MetersPerSecond);

        public static Speed operator -(Speed s1, Speed s2) => new Speed(s1.MetersPerSecond - s2.MetersPerSecond);

        public static Speed operator *(Speed s, float f) => new Speed(s.MetersPerSecond * f);

        public static Speed operator *(float f, Speed s) => s * f;

        public static Speed operator /(Speed s, float f) => new Speed(s.MetersPerSecond / f);

        public static Distance operator *(Speed s, Duration t) => new Distance(s.MetersPerSecond * t.Seconds);

        public static Duration operator /(Speed s, Acceleration a) => new Duration(s.MetersPerSecond / a.MetersPerSecondSquared);

        public static bool operator <(Speed s1, Speed s2) => s1.MetersPerSecond < s2.MetersPerSecond;

        public static bool operator >(Speed s1, Speed s2) => s1.MetersPerSecond > s2.MetersPerSecond;

        public static bool operator <=(Speed s1, Speed s2) => s1.MetersPerSecond <= s2.MetersPerSecond;

        public static bool operator >=(Speed s1, Speed s2) => s1.MetersPerSecond >= s2.MetersPerSecond;

        public static Speed FromMetersPerSecond(float metersPerSecond) => new Speed(metersPerSecond);

        public static Speed FromKilometersPerHour(float kilometersPerHour) => new Speed(kilometersPerHour / 3.6f);
    }
}
