using System;
using System.Collections;
using System.Collections.Generic;

namespace Geometry
{
    public class Path : IReadOnlyList<Position2f>
    {
        private readonly List<Position2f> _path;

        public Path(List<Position2f> path)
        {
            _path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public int Count => _path.Count;

        public Position2f this[int index] => _path[index];

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Position2f> GetEnumerator()
        {
            return _path.GetEnumerator();
        }

        public int FindIndex(Predicate<Position2f> match)
        {
            return _path.FindIndex(match);
        }

        public int IndexOf(Position2f pos)
        {
            return _path.IndexOf(pos);
        }

        public float Length()
        {
            var length = 0f;
            for (var i = 1; i < Count; ++i)
            {
                length += (_path[i] - _path[i - 1]).Length();
            }

            return length;
        }

        public (Path, Path) OffsetPaths(float offset)
        {
            var r1 = new List<Position2f>();
            var r2 = new List<Position2f>();
            for (var i = 0; i < _path.Count; ++i)
            {
                var b = _path[i];
                var a = i == 0 ? b : _path[i - 1];
                var c = i == _path.Count - 1 ? b : _path[i + 1];
                var (p1, p2) = b.GetOffsetPoints(c - a, offset);
                r1.Add(p1);
                r2.Add(p2);
            }

            r2.Reverse();
            return (new Path(r1), new Path(r2));
        }

        public Path Subpath(int startIndex, int length)
        {
            if (startIndex + length > Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (startIndex < 0 || length < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            return new Path(_path.GetRange(startIndex, length));
        }
    }
}