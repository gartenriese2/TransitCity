using System;

namespace Geometry.Shapes
{
    public interface IShape
    {
        Position2f CreateRandomPoint(Random rnd);
    }
}
