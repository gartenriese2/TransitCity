using System;

namespace Geometry
{
    public class Vector2d : IVector
    {
        public Vector2d()
        {
            X = 0.0;
            Y = 0.0;
        }

        public Vector2d(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }

        public double Y { get; }

        public static Vector2d operator +(Vector2d v1, Vector2d v2)
        {
            return new Vector2d(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2d operator -(Vector2d v1, Vector2d v2)
        {
            return new Vector2d(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector2d operator *(Vector2d v1, double s)
        {
            return new Vector2d(v1.X * s, v1.Y * s);
        }

        public static Vector2d operator /(Vector2d v1, double s)
        {
            if (Math.Abs(s) < double.Epsilon)
            {
                throw new ArgumentException();
            }

            return new Vector2d(v1.X / s, v1.Y / s);
        }

        public static Vector2d operator -(Vector2d v)
        {
            return new Vector2d(-v.X, -v.Y);
        }

        public Vector2d Abs() => new Vector2d(Math.Abs(X), Math.Abs(Y));

        public double Dot(Vector2d other) => X * other.X + Y * other.Y;

        public double AbsDot(Vector2d other) => Math.Abs(Dot(other));

        public double LengthSquared() => X * X + Y * Y;

        public double Length() => Math.Sqrt(LengthSquared());

        public Vector2d Normalize() => this / Length();

        public double MinComponent() => Math.Min(X, Y);

        public double MaxComponent() => Math.Max(X, Y);

        public Vector2d RotateRight() => new Vector2d(Y, -X);

        public Vector2d RotateLeft() => new Vector2d(-Y, X);

        public Vector2d Inverse() => new Vector2d(-X, -Y);
    }
}
