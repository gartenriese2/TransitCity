namespace Geometry.Shapes
{
    using System;

    public interface IShape
    {
        double Area { get; }

        (Position2d, Position2d) Bounds { get; }

        Position2d Centroid { get; }

        Position2d CreateRandomPoint(Random rnd);

        bool IsPointInside(Position2d point);
    }
}
