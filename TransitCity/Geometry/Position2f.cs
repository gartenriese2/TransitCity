﻿namespace Geometry
{
    public class Position2f : IPosition
    {
        public Position2f()
        {
            X = 0f;
            Y = 0f;
        }

        public Position2f(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X { get; }

        public float Y { get; }

        public static Position2f operator +(Position2f p1, Position2f p2)
        {
            return new Position2f(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Position2f operator +(Position2f p, Vector2f v)
        {
            return new Position2f(p.X + v.X, p.Y + v.Y);
        }

        public static Vector2f operator -(Position2f p1, Position2f p2)
        {
            return new Vector2f(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static Position2f operator -(Position2f p, Vector2f v)
        {
            return new Position2f(p.X - v.X, p.Y - v.Y);
        }

        public static Position2f operator *(Position2f p, float s)
        {
            return new Position2f(p.X * s, p.Y * s);
        }

        public static Position2f operator *(float s, Position2f p) => p * s;

        public static Position2f Lerp(float t, Position2f p1, Position2f p2)
        {
            return p1 * (1 - t) + p2 * t;
        }

        public override string ToString()
        {
            return $"({X}|{Y})";
        }

        public double DistanceTo(IPosition other)
        {
            return DistanceTo((Position2f) other);
        }

        public (Position2f, Position2f) GetOffsetPoints(Vector2f vec, float offset)
        {
            var vecRight = vec.RotateRight().Normalize() * offset;
            var vecLeft = vec.RotateLeft().Normalize() * offset;
            return (this + vecRight, this + vecLeft);
        }

        public bool EqualPosition(Position2f other)
        {
            return DistanceTo(other) <= float.Epsilon;
        }

        private float DistanceTo(Position2f other)
        {
            return (this - other).Length();
        }
    }
}
