using Geometry;

namespace PathFinding.Network
{
    public class DirectedEdge<C, P> where C : IEdgeCost where P : IPosition
    {
        internal DirectedEdge(Node<P> nodeA, Node<P> nodeB, C cost)
        {
            NodeA = nodeA;
            NodeB = nodeB;
            Cost = cost;
        }

        internal Node<P> NodeA { get; }

        internal Node<P> NodeB { get; }

        internal C Cost { get; }

        public override string ToString()
        {
            return $"DirectedEdge: {NodeA} <-> {NodeB} (Cost: {Cost})";
        }

        internal DirectedEdge<C, P> GetReverse()
        {
            return new DirectedEdge<C, P>(NodeB, NodeA, Cost);
        }
    }
}
