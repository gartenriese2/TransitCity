using System;
using System.Collections.Generic;
using System.Linq;

namespace Geometry
{
    public class CentripetalCatmullRomSpline
    {
        private readonly List<Position2d> _controlPoints;

        public CentripetalCatmullRomSpline(List<Position2d> controlPoints)
        {
            if (controlPoints == null || controlPoints.Count < 2)
            {
                throw new ArgumentException();
            }

            _controlPoints = controlPoints;
        }

        public Path CalculateSegments(int subsegments)
        {
            if (subsegments < 1)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (subsegments == 1)
            {
                return new Path(_controlPoints);
            }

            var pts = new List<Position2d>
            {
                _controlPoints[0]
            };
            var dt = 1.0 / subsegments;

            if (_controlPoints.Count == 2)
            {
                for (var i = 1; i < subsegments; ++i)
                {
                    pts.Add(PointOnCurve(_controlPoints[0], _controlPoints[1], dt * i));
                }

                pts.Add(_controlPoints.Last());
                return new Path(pts);
            }

            for (var i = 1; i < subsegments; ++i)
            {
                pts.Add(PointOnCurve(_controlPoints[0], _controlPoints[0], _controlPoints[1], _controlPoints[2], dt * i));
            }

            for (var idx = 1; idx < _controlPoints.Count - 2; ++idx)
            {
                pts.Add(_controlPoints[idx]);
                for (var i = 1; i < subsegments; ++i)
                {
                    pts.Add(PointOnCurve(_controlPoints[idx - 1], _controlPoints[idx], _controlPoints[idx + 1], _controlPoints[idx + 2], dt * i));
                }
            }

            pts.Add(_controlPoints[_controlPoints.Count - 2]);
            for (var i = 1; i < subsegments; ++i)
            {
                pts.Add(PointOnCurve(_controlPoints[_controlPoints.Count - 3], _controlPoints[_controlPoints.Count - 2], _controlPoints[_controlPoints.Count - 1], _controlPoints[_controlPoints.Count - 1], dt * i));
            }

            pts.Add(_controlPoints.Last());

            return new Path(pts);
        }

        private static Position2d PointOnCurve(Position2d p0, Position2d p1, Position2d p2, Position2d p3, double t)
        {
            var t2 = t * t;
            var t3 = t2 * t;

            var x = 0.5 * (2.0 * p1.X +
                           (-p0.X + p2.X) * t +
                           (2.0 * p0.X - 5.0 * p1.X + 4 * p2.X - p3.X) * t2 +
                           (-p0.X + 3.0 * p1.X - 3.0 * p2.X + p3.X) * t3);

            var y = 0.5 * (2.0 * p1.Y +
                           (-p0.Y + p2.Y) * t +
                           (2.0 * p0.Y - 5.0 * p1.Y + 4 * p2.Y - p3.Y) * t2 +
                           (-p0.Y + 3.0 * p1.Y - 3.0 * p2.Y + p3.Y) * t3);

            return new Position2d(x, y);
        }

        private static Position2d PointOnCurve(Position2d p0, Position2d p1, double t)
        {
            return p0 + (p1 - p0) * t;
        }
    }
}
