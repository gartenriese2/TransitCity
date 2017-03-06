using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;

namespace PathFinding.Network
{
    public class Network<P, C> where P : IPosition where C : IEdgeCost
    {
        private readonly Dictionary<Node<P>, List<DirectedEdge<C, P>>> _adjacencyList = new Dictionary<Node<P>, List<DirectedEdge<C, P>>>();

        private long _nodeCounter = 0;

        public IEnumerable<Node<P>> Nodes => _adjacencyList.Keys;

        public Node<P> CreateNode(P position)
        {
            return new Node<P>(_nodeCounter++, position);
        }

        public void AddNode(Node<P> node)
        {
            if (!_adjacencyList.ContainsKey(node))
            {
                _adjacencyList.Add(node, new List<DirectedEdge<C, P>>());
            }
        }

        public void AddDirectedEdge(DirectedEdge<C, P> directedEdge)
        {
            AddNodeConnection(directedEdge.NodeA, directedEdge);
        }

        public DirectedEdge<C, P> AddDirectedEdge(Node<P> nodeA, Node<P> nodeB, Func<Node<P>, Node<P>, C> costFunc)
        {
            var edge = new DirectedEdge<C, P>(nodeA, nodeB, costFunc(nodeA, nodeB));
            AddNodeConnection(nodeA, edge);
            return edge;
        }

        public void AddUndirectedEdge(DirectedEdge<C, P> directedEdge)
        {
            AddNodeConnection(directedEdge.NodeA, directedEdge);
            AddNodeConnection(directedEdge.NodeB, directedEdge.GetReverse());
        }

        public DirectedEdge<C, P> AddUndirectedEdge(Node<P> nodeA, Node<P> nodeB, Func<Node<P>, Node<P>, C> costFunc)
        {
            var edge = new DirectedEdge<C, P>(nodeA, nodeB, costFunc(nodeA, nodeB));
            AddNodeConnection(nodeA, edge);
            AddNodeConnection(nodeB, edge.GetReverse());
            return edge;
        }

        internal List<DirectedEdge<C, P>> GetOutgoingEdges(Node<P> node)
        {
            return _adjacencyList.ContainsKey(node) ? _adjacencyList[node] : new List<DirectedEdge<C, P>>();
        }

        internal List<Node<P>> GetNextNodes(Node<P> node)
        {
            return _adjacencyList.ContainsKey(node) ? _adjacencyList[node].Select(edge => edge.NodeB).ToList() : new List<Node<P>>();
        }

        private void AddNodeConnection(Node<P> nodeA, DirectedEdge<C, P> directedEdge)
        {
            if (!_adjacencyList.ContainsKey(nodeA))
            {
                _adjacencyList.Add(nodeA, new List<DirectedEdge<C, P>>());
            }

            if (!_adjacencyList[nodeA].Contains(directedEdge))
            {
                _adjacencyList[nodeA].Add(directedEdge);
            }
        }
    }
}
