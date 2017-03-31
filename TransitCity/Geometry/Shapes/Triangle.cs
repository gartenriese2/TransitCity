using System;

namespace Geometry.Shapes
{
    public class Triangle : IShape
    {
        public Triangle(Position2f a, Position2f b, Position2f c)
        {
            A = a;
            B = b;
            C = c;

            var ab = A.DistanceTo(B);
            var bc = B.DistanceTo(C);
            var ca = C.DistanceTo(A);
            var s = (ab + bc + ca) / 2;
            Area = (float)Math.Sqrt(s * (s - ab) * (s - bc) * (s - ca));

            var minX = Math.Min(a.X, Math.Min(b.X, c.X));
            var minY = Math.Min(a.Y, Math.Min(b.Y, c.Y));
            var maxX = Math.Max(a.X, Math.Max(b.X, c.X));
            var maxY = Math.Max(a.Y, Math.Max(b.Y, c.Y));
            Bounds = (new Position2f(minX, minY), new Position2f(maxX, maxY));

            var centroid = A + B + C;
            Centroid = new Position2f(centroid.X / 3f, centroid.Y / 3f);
        }

        public Position2f A { get; }

        public Position2f B { get; }

        public Position2f C { get; }

        public float Area { get; }

        public (Position2f, Position2f) Bounds { get; }

        public Position2f Centroid { get; }

        public Position2f CreateRandomPoint(Random rnd)
        {
            var r1 = (float)Math.Sqrt(rnd.NextDouble());
            var r2 = (float)rnd.NextDouble();
            return (1 - r1) * A + r1 * (1 - r2) * B + r1 * r2 * C;
        }

        public bool IsPointInside(Position2f point)
        {
            var b1 = Sign(point, A, B) < 0.0f;
            var b2 = Sign(point, B, C) < 0.0f;
            var b3 = Sign(point, C, A) < 0.0f;

            return b1 == b2 && b2 == b3;

            float Sign(Position2f p1, Position2f p2, Position2f p3)
            {
                return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
            }
        }

        public override string ToString()
        {
            return $"{A}, {B}, {C}";
        }
    }
}
