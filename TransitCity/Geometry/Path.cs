using System;
using System.Collections;
using System.Collections.Generic;

namespace Geometry
{
    public class Path<TPos> : IReadOnlyList<TPos> where TPos : IPosition
    {
        private readonly List<TPos> _path;

        public Path(List<TPos> path)
        {
            _path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public int Count => _path.Count;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public TPos this[int index] => _path[index];

        public IEnumerator<TPos> GetEnumerator()
        {
            return _path.GetEnumerator();
        }

        public int FindIndex(Predicate<TPos> match)
        {
            return _path.FindIndex(match);
        }

        public static (Path<Position2f>, Path<Position2f>) GetOffsetPaths(Path<Position2f> path, float offset)
        {
            var r1 = new List<Position2f>();
            var r2 = new List<Position2f>();
            for (var i = 0; i < path.Count; ++i)
            {
                var b = path[i];
                var a = i == 0 ? b : path[i - 1];
                var c = i == path.Count - 1 ? b : path[i + 1];
                var (p1, p2) = b.GetOffsetPoints(c - a, offset);
                r1.Add(p1);
                r2.Add(p2);
            }

            r2.Reverse();
            return (new Path<Position2f>(r1), new Path<Position2f>(r2));
        }
    }
}