using System;

namespace Geometry.Shapes
{
    public class Circle : IShape
    {
        public Circle(Position2d center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        public Position2d Center { get; }

        public double Radius { get; }

        public double Area => (Radius * Radius * Math.PI);

        public (Position2d, Position2d) Bounds => (new Position2d(Center.X - Radius, Center.Y - Radius), new Position2d(Center.X + Radius, Center.Y + Radius));

        public Position2d Centroid => Center;

        public Position2d CreateRandomPoint(Random rnd)
        {
            var t = 2 * Math.PI * rnd.NextDouble();
            var u = rnd.NextDouble() + rnd.NextDouble();
            var r = u > 1 ? 2 - u : u;
            var x = r * Radius * Math.Cos(t);
            var y = r * Radius * Math.Sin(t);
            return Center + new Position2d(x, y);
        }

        public bool IsPointInside(Position2d point) => point.DistanceTo(Center) <= Radius;

        public override string ToString()
        {
            return $"Center: {Center}, Radius: {Radius}";
        }
    }
}
