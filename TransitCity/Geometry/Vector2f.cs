using System;

namespace Geometry
{
    public class Vector2f
    {
        public Vector2f()
        {
            X = 0f;
            Y = 0f;
        }

        public Vector2f(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X { get; }

        public float Y { get; }

        public static Vector2f operator +(Vector2f v1, Vector2f v2)
        {
            return new Vector2f(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2f operator -(Vector2f v1, Vector2f v2)
        {
            return new Vector2f(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector2f operator *(Vector2f v1, float s)
        {
            return new Vector2f(v1.X * s, v1.Y * s);
        }

        public static Vector2f operator /(Vector2f v1, float s)
        {
            if (Math.Abs(s) < float.Epsilon)
            {
                throw new ArgumentException();
            }

            return new Vector2f(v1.X / s, v1.Y / s);
        }

        public static Vector2f operator -(Vector2f v)
        {
            return new Vector2f(-v.X, -v.Y);
        }

        public Vector2f Abs() => new Vector2f(Math.Abs(X), Math.Abs(Y));

        public float Dot(Vector2f other) => X * other.X + Y * other.Y;

        public float AbsDot(Vector2f other) => Math.Abs(Dot(other));

        public float LengthSquared() => X * X + Y * Y;

        public float Length() => (float) Math.Sqrt(LengthSquared());

        public Vector2f Normalize() => this / Length();

        public float MinComponent() => Math.Min(X, Y);

        public float MaxComponent() => Math.Max(X, Y);

        public Vector2f RotateRight() => new Vector2f(Y, -X);

        public Vector2f RotateLeft() => new Vector2f(-Y, X);

        public Vector2f Inverse() => new Vector2f(-X, -Y);
    }
}
