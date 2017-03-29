using System;

namespace Geometry.Shapes
{
    public interface IShape
    {
        Position2f CreateRandomPoint(Random rnd);

        float Area { get; }

        (Position2f, Position2f) Bounds { get; }
    }
}
