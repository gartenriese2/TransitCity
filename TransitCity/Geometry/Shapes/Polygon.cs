using System;
using System.Collections.Generic;
using System.Linq;

namespace Geometry.Shapes
{
    public class Polygon : IShape
    {
        private readonly List<Triangle> _triangulation = new List<Triangle>();

        public Polygon(params float[] coords)
        {
            if (coords == null)
            {
                throw new ArgumentNullException();
            }

            if (coords.Length % 2 != 0)
            {
                throw new ArgumentException();
            }

            if (coords.Length < 6)
            {
                throw new ArgumentOutOfRangeException();
            }

            Vertices = new List<Position2f>(coords.Length / 2);
            for (var i = 0; i < coords.Length - 1; i += 2)
            {
                Vertices.Add(new Position2f(coords[i], coords[i + 1]));
            }

            Initialize();
        }

        public Polygon(List<Position2f> vertices)
        {
            Vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));
            if (Vertices.Count < 3)
            {
                throw new ArgumentException();
            }

            Initialize();
        }

        public List<Position2f> Vertices { get; }

        public float Area { get; private set; }

        public (Position2f, Position2f) Bounds { get; private set; }

        public Position2f Centroid { get; private set; }

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

        private void Initialize()
        {
            Triangulate();

            Area = _triangulation.Aggregate(0f, (f, tri) => f + tri.Area);

            var minX = Vertices.Min(p => p.X);
            var minY = Vertices.Min(p => p.Y);
            var maxX = Vertices.Max(p => p.X);
            var maxY = Vertices.Max(p => p.Y);
            Bounds = (new Position2f(minX, minY), new Position2f(maxX, maxY));

            var centroid = _triangulation.Aggregate(new Position2f(), (current, triangle) => current + triangle.Area * triangle.Centroid);
            Centroid = new Position2f(centroid.X / Area, centroid.Y / Area);
        }

        private void Triangulate()
        {
            _triangulation.Clear();

            if (Vertices.Count < 3)
            {
                return;
            }

            if (Vertices.Count == 3)
            {
                _triangulation.Add(new Triangle(Vertices[0], Vertices[1], Vertices[2]));
                return;
            }

            var untriangulatedVertices = new List<Position2f>(Vertices);

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
