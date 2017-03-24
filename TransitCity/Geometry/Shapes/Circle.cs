using System;

namespace Geometry.Shapes
{
    public class Circle : IShape
    {
        private readonly Position2f _center;

        private readonly float _radius;

        public Circle(Position2f center, float radius)
        {
            _center = center;
            _radius = radius;
        }

        public Position2f CreateRandomPoint(Random rnd)
        {
            var t = 2 * Math.PI * rnd.NextDouble();
            var u = rnd.NextDouble() + rnd.NextDouble();
            var r = u > 1 ? 2 - u : u;
            var x = r * Math.Cos(t);
            var y = r * Math.Sin(t);
            return new Position2f((float) x, (float) y);
        }
    }
}
