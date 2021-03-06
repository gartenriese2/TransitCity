﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Geometry
{
    public class Path : IReadOnlyList<Position2d>
    {
        private readonly List<Position2d> _path;

        public Path(List<Position2d> path)
        {
            _path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public int Count => _path.Count;

        public Position2d this[int index] => _path[index];

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Position2d> GetEnumerator()
        {
            return _path.GetEnumerator();
        }

        public int FindIndex(Predicate<Position2d> match)
        {
            return _path.FindIndex(match);
        }

        public int IndexOf(Position2d pos)
        {
            return _path.IndexOf(pos);
        }

        public double Length()
        {
            var length = 0.0;
            for (var i = 1; i < Count; ++i)
            {
                length += (_path[i] - _path[i - 1]).Length();
            }

            return length;
        }

        public (Path, Path) OffsetPaths(double offset)
        {
            var r1 = new List<Position2d>();
            var r2 = new List<Position2d>();
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

        public Position2d Lerp(double t)
        {
            if (t < 0.0 || t > 1.0)
            {
                throw new ArgumentOutOfRangeException();
            }

            var absoluteT = t * Length();
            var idx = 1;
            var lengthCounter = 0.0;
            while ((_path[idx] - _path[idx - 1]).Length() + lengthCounter < absoluteT)
            {
                lengthCounter += (_path[idx] - _path[idx - 1]).Length();
                ++idx;
            }

            return Position2d.Lerp((absoluteT - lengthCounter) / (_path[idx] - _path[idx - 1]).Length(), _path[idx - 1], _path[idx]);
        }

        public Vector2d DirectionLerp(double t)
        {
            if (t < 0.0 || t > 1.0)
            {
                throw new ArgumentOutOfRangeException();
            }

            var len = t * Length();
            var idx = 1;
            var lengthCounter = 0.0;
            while ((_path[idx] - _path[idx - 1]).Length() + lengthCounter < len)
            {
                lengthCounter += (_path[idx] - _path[idx - 1]).Length();
                ++idx;
            }

            return idx == 0 ? _path[idx + 1] - _path[idx] : _path[idx] - _path[idx - 1];
        }
    }
}