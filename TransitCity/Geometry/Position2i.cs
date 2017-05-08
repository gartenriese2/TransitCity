using System;

namespace Geometry
{
    public class Position2i : IPosition
    {
        public Position2i(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }

        public int Y { get; }

        public override string ToString()
        {
            return $"({X}|{Y})";
        }

        public double DistanceTo(IPosition other)
        {
            return DistanceTo((Position2i) other);
        }

        public float DistanceTo(Position2i other)
        {
            return (float) Math.Sqrt(Math.Pow(other.X - X, 2) + Math.Pow(other.Y - Y, 2));
        }
    }
}