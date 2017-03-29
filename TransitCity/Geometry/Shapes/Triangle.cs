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
        }

        public Position2f A { get; }

        public Position2f B { get; }

        public Position2f C { get; }

        public float Area { get; }

        public (Position2f, Position2f) Bounds { get; }

        public Position2f CreateRandomPoint(Random rnd)
        {
            var r1 = (float)Math.Sqrt(rnd.NextDouble());
            var r2 = (float)rnd.NextDouble();
            return (1 - r1) * A + r1 * (1 - r2) * B + r1 * r2 * C;
        }
    }
}
