using Geometry;

namespace PathFinding.Network
{
    public class Node<P> where P : IPosition
    {
        private readonly long _id;

        internal Node(long id, P position)
        {
            _id = id;
            Position = position;
        }

        public P Position { get; }

        public override string ToString()
        {
            return $"Node: {Position}";
        }
    }

    public class Node2f : Node<Position2f>
    {
        internal Node2f(long id, float x, float y) : base(id, new Position2f(x, y))
        {
        }
    }

    internal class Node2i : Node<Position2i>
    {
        internal Node2i(long id, int x, int y) : base(id, new Position2i(x, y))
        {
        }
    }
}
