using System;
using System.Collections.Generic;
using System.Linq;

namespace Geometry.Shapes
{
    public class Polygon : IShape
    {
        private readonly List<Position2f> _vertices;

        private readonly List<Triangle> _triangulation = new List<Triangle>();

        public Polygon(List<Position2f> vertices)
        {
            _vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));
            if (_vertices.Count < 3)
            {
                throw new ArgumentException();
            }

            Triangulate();

            Area = _triangulation.Aggregate(0f, (f, tri) => f + tri.Area);

            var minX = _vertices.Min(p => p.X);
            var minY = _vertices.Min(p => p.Y);
            var maxX = _vertices.Max(p => p.X);
            var maxY = _vertices.Max(p => p.Y);
            Bounds = (new Position2f(minX, minY), new Position2f(maxX, maxY));
        }

        public float Area { get; }

        public (Position2f, Position2f) Bounds { get; }

        public Position2f CreateRandomPoint(Random rnd)
        {
            var val = rnd.NextDouble() * Area;
            var areaSum = 0f;
            foreach (var tri in _triangulation)
            {
                areaSum += tri.Area;
                if (val <= areaSum)
                {
                    return tri.CreateRandomPoint(rnd);
                }
            }

            throw new InvalidOperationException();
        }

        public bool IsPointInside(Position2f point) => _triangulation.Any(t => t.IsPointInside(point));

        private void Triangulate()
        {
            _triangulation.Clear();

            if (_vertices.Count < 3)
            {
                return;
            }

            if (_vertices.Count == 3)
            {
                _triangulation.Add(new Triangle(_vertices[0], _vertices[1], _vertices[2]));
                return;
            }

            var untriangulatedVertices = new List<Position2f>(_vertices);

            while (untriangulatedVertices.Count > 3)
            {
                var num = untriangulatedVertices.Count;
                for (var i = 0; i < num; ++i)
                {
                    var iBefore = i == 0 ? num - 1 : i - 1;
                    var iAfter = i == num - 1 ? 0 : i + 1;
                    var tri = new Triangle(untriangulatedVertices[iBefore], untriangulatedVertices[i], untriangulatedVertices[iAfter]);
                    var anyInside = untriangulatedVertices.Where((t, j) => j != iBefore && j != i && j != iAfter).Any(vertex => tri.IsPointInside(vertex));
                    if (anyInside)
                    {
                        continue;
                    }

                    _triangulation.Add(tri);
                    untriangulatedVertices.RemoveAt(i);
                    break;
                }
            }

            _triangulation.Add(new Triangle(untriangulatedVertices[0], untriangulatedVertices[1], untriangulatedVertices[2]));
        }
    }
}
