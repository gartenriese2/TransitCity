using System;

namespace Geometry.Shapes
{
    public class Triangle : IShape
    {
        public Triangle(Position2d a, Position2d b, Position2d c)
        {
            A = a;
            B = b;
            C = c;

            var ab = A.DistanceTo(B);
            var bc = B.DistanceTo(C);
            var ca = C.DistanceTo(A);
            var s = (ab + bc + ca) / 2;
            Area = Math.Sqrt(s * (s - ab) * (s - bc) * (s - ca));

            var minX = Math.Min(a.X, Math.Min(b.X, c.X));
            var minY = Math.Min(a.Y, Math.Min(b.Y, c.Y));
            var maxX = Math.Max(a.X, Math.Max(b.X, c.X));
            var maxY = Math.Max(a.Y, Math.Max(b.Y, c.Y));
            Bounds = (new Position2d(minX, minY), new Position2d(maxX, maxY));

            var centroid = A + B + C;
            Centroid = new Position2d(centroid.X / 3.0, centroid.Y / 3.0);
        }

        public Position2d A { get; }

        public Position2d B { get; }

        public Position2d C { get; }

        public double Area { get; }

        public (Position2d, Position2d) Bounds { get; }

        public Position2d Centroid { get; }

        public Position2d CreateRandomPoint(Random rnd)
        {
            var r1 = Math.Sqrt(rnd.NextDouble());
            var r2 = rnd.NextDouble();
            return (1 - r1) * A + r1 * (1 - r2) * B + r1 * r2 * C;
        }

        public bool IsPointInside(Position2d point)
        {
            var b1 = Sign(point, A, B) < 0.0;
            var b2 = Sign(point, B, C) < 0.0;
            var b3 = Sign(point, C, A) < 0.0;

            return b1 == b2 && b2 == b3;

            double Sign(Position2d p1, Position2d p2, Position2d p3)
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
