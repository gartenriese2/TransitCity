using System;

namespace Geometry
{
    public class Position2f : IPosition
    {
        public Position2f(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X { get; }

        public float Y { get; }

        public override string ToString()
        {
            return $"({X}|{Y})";
        }

        public float DistanceTo(IPosition other)
        {
            return DistanceTo((Position2f) other);
        }

        private float DistanceTo(Position2f other)
        {
            return (float) Math.Sqrt(Math.Pow(other.X - X, 2) + Math.Pow(other.Y - Y, 2));
        }
    }
}
