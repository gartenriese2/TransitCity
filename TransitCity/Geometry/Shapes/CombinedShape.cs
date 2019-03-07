namespace Geometry.Shapes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CombinedShape : IShape
    {
        private readonly List<IShape> _subShapes;

        public CombinedShape(IEnumerable<IShape> subShapes)
        {
            _subShapes = subShapes?.ToList() ?? throw new ArgumentNullException(nameof(subShapes));

            Area = _subShapes.Sum(s => s.Area);

            var minX = _subShapes.Min(s => s.Bounds.Item1.X);
            var minY = _subShapes.Min(s => s.Bounds.Item1.Y);
            var maxX = _subShapes.Max(s => s.Bounds.Item2.X);
            var maxY = _subShapes.Max(s => s.Bounds.Item2.Y);
            Bounds = (new Position2d(minX, minY), new Position2d(maxX, maxY));

            var centroid = _subShapes.Aggregate(new Position2d(), (current, shape) => current + shape.Area * shape.Centroid);
            Centroid = new Position2d(centroid.X / Area, centroid.Y / Area);
        }

        public double Area { get; }

        public (Position2d, Position2d) Bounds { get; }

        public Position2d Centroid { get; }

        public Position2d CreateRandomPoint(Random rnd)
        {
            var val = rnd.NextDouble() * Area;
            var areaSum = 0.0;
            foreach (var shape in _subShapes)
            {
                areaSum += shape.Area;
                if (val <= areaSum)
                {
                    return shape.CreateRandomPoint(rnd);
                }
            }

            throw new InvalidOperationException();
        }

        public bool IsPointInside(Position2d point)
        {
            return _subShapes.Any(s => s.IsPointInside(point));
        }
    }
}
