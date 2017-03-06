using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using PathFinding.Network;

namespace PathFinding.Algorithm
{
    internal class AStarAlgorithm<P, C> : IGraphAlgorithm<P, C> where P : IPosition where C : IEstimateableEdgeCost, new()
    {
        private readonly Func<Node<P>, Node<P>, C> _heuristicCostEstimateFunc;

        internal AStarAlgorithm(Func<Node<P>, Node<P>, C> heuristicCostEstimateFunc)
        {
            _heuristicCostEstimateFunc = heuristicCostEstimateFunc;
        }

        public C ComputeCost(Network<P, C> network, Node<P> from, Node<P> to)
        {
            throw new NotImplementedException();
        }

        public List<DirectedEdge<C, P>> ComputePath(Network<P, C> network, Node<P> from, Node<P> to)
        {
            throw new NotImplementedException();
        }

        public Tuple<C, List<DirectedEdge<C, P>>> Compute(Network<P, C> network, Node<P> from, Node<P> to)
        {
            var closedSet = new List<Node<P>>();
            var openSet = new List<Node<P>> { from };
            var cameFrom = new Dictionary<Node<P>, DirectedEdge<C, P>>();
            var gScore = new Dictionary<Node<P>, C>
            {
                [from] = new C()
            };
            var fScore = new Dictionary<Node<P>, C>
            {
                [from] = _heuristicCostEstimateFunc(from, to)
            };

            while (openSet.Count > 0)
            {
                openSet.Sort((nodeA, nodeB) => fScore[nodeA].CompareTo(fScore[nodeB]));
                var current = openSet.First();
                if (current == to)
                {
                    var path = ReconstructPath(cameFrom, current);
                    return new Tuple<C, List<DirectedEdge<C, P>>>(path.Select(edge => edge.Cost).Aggregate((c1, c2) => (C)c1.Add(c2)), path);
                }

                openSet.Remove(current);
                closedSet.Add(current);
                foreach (var edge in network.GetOutgoingEdges(current))
                {
                    var neighbor = edge.NodeB;
                    if (closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    var tentativeGScore = (C)gScore[current].Add(edge.Cost);
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                    else if (tentativeGScore.GreaterOrEquals(gScore[neighbor]))
                    {
                        continue;
                    }

                    cameFrom[neighbor] = edge;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = (C)gScore[neighbor].Add(_heuristicCostEstimateFunc(neighbor, to));
                }
            }

            return null;
        }

        private List<DirectedEdge<C, P>> ReconstructPath(Dictionary<Node<P>, DirectedEdge<C, P>> cameFrom, Node<P> current)
        {
            var totalPath = new List<DirectedEdge<C, P>>();
            var tmp = current;
            while (cameFrom.ContainsKey(tmp))
            {
                var edge = cameFrom[tmp];
                tmp = edge.NodeA;
                totalPath.Add(edge);
            }

            totalPath.Reverse();
            return totalPath;
        }
    }
}
