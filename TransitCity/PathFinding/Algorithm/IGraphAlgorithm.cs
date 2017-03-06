using System;
using System.Collections.Generic;
using Geometry;
using PathFinding.Network;

namespace PathFinding.Algorithm
{
    internal interface IGraphAlgorithm<P, C> where P : IPosition where C : IEdgeCost
    {
        C ComputeCost(Network<P, C> network, Node<P> from, Node<P> to);

        List<DirectedEdge<C, P>> ComputePath(Network<P, C> network, Node<P> from, Node<P> to);

        Tuple<C, List<DirectedEdge<C, P>>> Compute(Network<P, C> network, Node<P> from, Node<P> to);
    }
}
