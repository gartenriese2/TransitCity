namespace Geometry
{
    public class Position2d : IPosition
    {
        public Position2d()
        {
            X = 0.0;
            Y = 0.0;
        }

        public Position2d(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }

        public double Y { get; }

        public static Position2d operator +(Position2d p1, Position2d p2)
        {
            return new Position2d(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Position2d operator +(Position2d p, Vector2d v)
        {
            return new Position2d(p.X + v.X, p.Y + v.Y);
        }

        public static Vector2d operator -(Position2d p1, Position2d p2)
        {
            return new Vector2d(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static Position2d operator -(Position2d p, Vector2d v)
        {
            return new Position2d(p.X - v.X, p.Y - v.Y);
        }

        public static Position2d operator *(Position2d p, double s)
        {
            return new Position2d(p.X * s, p.Y * s);
        }

        public static Position2d operator *(double s, Position2d p) => p * s;

        public static Position2d operator /(Position2d p, double s)
        {
            return new Position2d(p.X / s, p.Y / s);
        }

        public static Position2d Lerp(double t, Position2d p1, Position2d p2)
        {
            return p1 * (1 - t) + p2 * t;
        }

        public override string ToString()
        {
            return $"({X:F2}|{Y:F2})";
        }

        public double DistanceTo(IPosition other)
        {
            return DistanceTo((Position2d)other);
        }

        public (Position2d, Position2d) GetOffsetPoints(Vector2d vec, double offset)
        {
            var vecRight = vec.RotateRight().Normalize() * offset;
            var vecLeft = vec.RotateLeft().Normalize() * offset;
            return (this + vecRight, this + vecLeft);
        }

        public bool EqualPosition(Position2d other)
        {
            return DistanceTo(other) <= double.Epsilon;
        }

        private double DistanceTo(Position2d other)
        {
            return (this - other).Length();
        }
    }
}
