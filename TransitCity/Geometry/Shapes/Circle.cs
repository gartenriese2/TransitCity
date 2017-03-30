using System;

namespace Geometry.Shapes
{
    public class Circle : IShape
    {
        public Circle(Position2f center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public Position2f Center { get; }

        public float Radius { get; }

        public float Area => (float) (Radius * Radius * Math.PI);

        public (Position2f, Position2f) Bounds => (new Position2f(Center.X - Radius, Center.Y - Radius), new Position2f(Center.X + Radius, Center.Y + Radius));

        public Position2f CreateRandomPoint(Random rnd)
        {
            var t = 2 * Math.PI * rnd.NextDouble();
            var u = rnd.NextDouble() + rnd.NextDouble();
            var r = u > 1 ? 2 - u : u;
            var x = r * Radius * Math.Cos(t);
            var y = r * Radius * Math.Sin(t);
            return Center + new Position2f((float) x, (float) y);
        }

        public bool IsPointInside(Position2f point) => point.DistanceTo(Center) <= Radius;

        public override string ToString()
        {
            return $"Center: {Center}, Radius: {Radius}";
        }
    }
}
