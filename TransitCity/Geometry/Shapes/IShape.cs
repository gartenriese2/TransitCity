using System;

namespace Geometry.Shapes
{
    public interface IShape
    {
        Position2f CreateRandomPoint(Random rnd);

        bool IsPointInside(Position2f point);

        float Area { get; }

        (Position2f, Position2f) Bounds { get; }

        Position2f Centroid { get; }
    }
}
