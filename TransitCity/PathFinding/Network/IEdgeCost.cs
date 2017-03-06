using System;

namespace PathFinding.Network
{
    public interface IEdgeCost : IComparable, IEquatable<IEdgeCost>
    {
        IEdgeCost Add(IEdgeCost other);

        bool GreaterOrEquals(IEdgeCost other);
    }
}
