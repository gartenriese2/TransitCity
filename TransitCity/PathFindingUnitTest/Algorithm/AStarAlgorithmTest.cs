using System;
using System.Collections.Generic;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PathFinding.Algorithm;
using PathFinding.Network;

namespace PathFindingUnitTest.Algorithm
{
    [TestClass]
    public class AStarAlgorithmTest
    {
        [TestMethod]
        public void ComputeTest()
        {
            Func<Node<Position2f>, Node<Position2f>, BasicEdgeCost> heuristicCostEstimateFunc = (a, b) => new BasicEdgeCost(a.Position.DistanceTo(b.Position));
            IGraphAlgorithm<Position2f, BasicEdgeCost> algorithm = new AStarAlgorithm<Position2f, BasicEdgeCost>(heuristicCostEstimateFunc);

            var network = new Network<Position2f, BasicEdgeCost>();

            var node00 = network.CreateNode(new Position2f(0, 0));
            var node10 = network.CreateNode(new Position2f(1, 0));
            var node11 = network.CreateNode(new Position2f(1, 1));
            network.AddDirectedEdge(new DirectedEdge<BasicEdgeCost, Position2f>(node00, node10, new BasicEdgeCost(node00.Position.DistanceTo(node10.Position))));
            network.AddDirectedEdge(new DirectedEdge<BasicEdgeCost, Position2f>(node00, node11, new BasicEdgeCost(node00.Position.DistanceTo(node11.Position))));
            network.AddDirectedEdge(new DirectedEdge<BasicEdgeCost, Position2f>(node10, node11, new BasicEdgeCost(node10.Position.DistanceTo(node11.Position))));

            var tuple = algorithm.Compute(network, node00, node11);
            Assert.IsTrue(tuple.Item1.GreaterOrEquals(new BasicEdgeCost(1.41421359f)));
        }

        [TestMethod]
        public void ComputeTest2()
        {
            Func<Node<Position2i>, Node<Position2i>, BasicEdgeCost> heuristicCostEstimateFunc = (a, b) => new BasicEdgeCost(a.Position.DistanceTo(b.Position) / a.Position.X);
            IGraphAlgorithm<Position2i, BasicEdgeCost> algorithm = new AStarAlgorithm<Position2i, BasicEdgeCost>(heuristicCostEstimateFunc);

            var network = new Network<Position2i, BasicEdgeCost>();

            var node00 = network.CreateNode(new Position2i(0, 0));
            var node10 = network.CreateNode(new Position2i(1, 0));
            var node20 = network.CreateNode(new Position2i(2, 0));
            var node30 = network.CreateNode(new Position2i(3, 0));
            var node01 = network.CreateNode(new Position2i(0, 1));
            var node11 = network.CreateNode(new Position2i(1, 1));
            var node21 = network.CreateNode(new Position2i(2, 1));
            var node31 = network.CreateNode(new Position2i(3, 1));
            var node02 = network.CreateNode(new Position2i(0, 2));
            var node12 = network.CreateNode(new Position2i(1, 2));
            var node22 = network.CreateNode(new Position2i(2, 2));
            var node32 = network.CreateNode(new Position2i(3, 2));
            var node03 = network.CreateNode(new Position2i(0, 3));
            var node13 = network.CreateNode(new Position2i(1, 3));
            var node23 = network.CreateNode(new Position2i(2, 3));
            var node33 = network.CreateNode(new Position2i(3, 3));

            Func<Node<Position2i>, Node<Position2i>, BasicEdgeCost> costFunc = (a, b) => new BasicEdgeCost(a.Position.DistanceTo(b.Position));
            network.AddUndirectedEdge(node00, node10, costFunc);
            network.AddUndirectedEdge(node00, node01, costFunc);
            network.AddUndirectedEdge(node10, node20, costFunc);
            network.AddUndirectedEdge(node10, node11, costFunc);
            network.AddUndirectedEdge(node20, node30, costFunc);
            network.AddUndirectedEdge(node20, node21, costFunc);
            network.AddUndirectedEdge(node30, node31, costFunc);

            network.AddUndirectedEdge(node01, node11, costFunc);
            network.AddUndirectedEdge(node01, node02, costFunc);
            network.AddUndirectedEdge(node11, node21, costFunc);
            network.AddUndirectedEdge(node11, node12, costFunc);
            network.AddUndirectedEdge(node21, node31, costFunc);
            network.AddUndirectedEdge(node21, node22, costFunc);
            network.AddUndirectedEdge(node31, node32, costFunc);

            network.AddUndirectedEdge(node02, node12, costFunc);
            network.AddUndirectedEdge(node02, node03, costFunc);
            network.AddUndirectedEdge(node12, node22, costFunc);
            network.AddUndirectedEdge(node12, node13, costFunc);
            network.AddUndirectedEdge(node22, node32, costFunc);
            network.AddUndirectedEdge(node22, node23, costFunc);
            network.AddUndirectedEdge(node32, node33, costFunc);

            network.AddUndirectedEdge(node03, node13, costFunc);
            network.AddUndirectedEdge(node13, node23, costFunc);
            network.AddUndirectedEdge(node23, node33, costFunc);

            var tuple = algorithm.Compute(network, node00, node32);
        }

        [TestMethod]
        public void ComputeTest3()
        {
            var tuple1 = ComputeTransitPath(new Position2f(0f, 0f), new Position2f(10000f, 10000f));
            var tuple2 = ComputeTransitPath(new Position2f(0f, 0f), new Position2f(10000f, 0f));
            var tuple3 = ComputeTransitPath(new Position2f(0f, 0f), new Position2f(0f, 10000f));
            var tuple4 = ComputeTransitPath(new Position2f(1000f, 1000f), new Position2f(3000f, 1000f));
        }

        private Tuple<TimeEdgeCost, List<DirectedEdge<TimeEdgeCost, Position2f>>> ComputeTransitPath(Position2f fromPos, Position2f toPos)
        {
            Func<Node<Position2f>, Node<Position2f>, TimeEdgeCost> heuristicCostEstimateFunc = (a, b) => new TimeEdgeCost(a.Position.DistanceTo(b.Position) / 28f); // 100 km/h
            IGraphAlgorithm<Position2f, TimeEdgeCost> algorithm = new AStarAlgorithm<Position2f, TimeEdgeCost>(heuristicCostEstimateFunc);

            var network = GetTransitNetwork();

            Func<Node<Position2f>, Node<Position2f>, TimeEdgeCost> costFuncWalkingWaiting = (a, b) => new TimeEdgeCost(a.Position.DistanceTo(b.Position) / 2.2f + 120f); // 8km/h + 2 min waiting
            Func<Node<Position2f>, Node<Position2f>, TimeEdgeCost> costFuncWalking = (a, b) => new TimeEdgeCost(a.Position.DistanceTo(b.Position) / 2.2f); // 8km/h

            var from = network.CreateNode(fromPos);
            var to = network.CreateNode(toPos);
            network.AddDirectedEdge(from, to, costFuncWalking);

            foreach (var node in network.Nodes)
            {
                network.AddDirectedEdge(from, node, costFuncWalkingWaiting);
                network.AddDirectedEdge(node, to, costFuncWalking);
            }

           return algorithm.Compute(network, from, to);
        }

        private Network<Position2f, TimeEdgeCost> GetTransitNetwork()
        {
            var network = new Network<Position2f, TimeEdgeCost>();

            // line 1
            var node1_1 = network.CreateNode(new Position2f(1500, 1000));
            var node1_2 = network.CreateNode(new Position2f(2500, 1000));
            var node1_3 = network.CreateNode(new Position2f(3500, 1500));
            var node1_4 = network.CreateNode(new Position2f(4000, 2500));
            var node1_5 = network.CreateNode(new Position2f(4500, 3500));
            var node1_6 = network.CreateNode(new Position2f(5500, 4000));
            var node1_7 = network.CreateNode(new Position2f(6500, 4000));
            var node1_8 = network.CreateNode(new Position2f(7500, 4000));
            var node1_9 = network.CreateNode(new Position2f(8000, 5000));
            var node1_10 = network.CreateNode(new Position2f(8000, 6000));
            var node1_11 = network.CreateNode(new Position2f(7500, 7000));
            var node1_12 = network.CreateNode(new Position2f(7500, 8000));
            var node1_13 = network.CreateNode(new Position2f(8000, 9000));

            // line 2
            var node2_1 = network.CreateNode(new Position2f(1500, 8000));
            var node2_2 = network.CreateNode(new Position2f(2500, 7500));
            var node2_3 = network.CreateNode(new Position2f(3500, 7000));
            var node2_4 = network.CreateNode(new Position2f(4500, 6500));
            var node2_5 = network.CreateNode(new Position2f(5500, 5500));
            var node2_6 = network.CreateNode(new Position2f(6000, 4750));
            var node2_7 = node1_7;
            var node2_8 = network.CreateNode(new Position2f(7000, 3000));
            var node2_9 = network.CreateNode(new Position2f(7000, 2000));
            var node2_10 = network.CreateNode(new Position2f(8000, 1500));
            var node2_11 = network.CreateNode(new Position2f(9000, 1500));

            Func<Node<Position2f>, Node<Position2f>, TimeEdgeCost> costFuncTransit = (a, b) =>
            {
                const float meanAcceleration = 0.6f;
                const float maximalSpeed = 70f / 3.6f; // 70 km/h
                const float timeToReachMaximalSpeed = maximalSpeed / meanAcceleration;
                const float neededDistanceToReachMaximalSpeed = meanAcceleration / 2 * timeToReachMaximalSpeed * timeToReachMaximalSpeed;
                var distance = a.Position.DistanceTo(b.Position);
                var baseTime = 30f; // waiting time at station
                if (distance < 2 * neededDistanceToReachMaximalSpeed) // distance is too small to reach maximalSpeed
                {
                    baseTime += 2 * (float) Math.Sqrt(distance / meanAcceleration);
                }
                else
                {
                    var remainingDistance = distance - 2 * neededDistanceToReachMaximalSpeed;
                    baseTime += 2 * timeToReachMaximalSpeed + remainingDistance / maximalSpeed;
                }

                return new TimeEdgeCost(baseTime);
            };
            Func<Node<Position2f>, Node<Position2f>, TimeEdgeCost> costFuncChanging = (a, b) => new TimeEdgeCost(60f + 120f);

            // line 1
            network.AddUndirectedEdge(node1_1, node1_2, costFuncTransit);
            network.AddUndirectedEdge(node1_2, node1_3, costFuncTransit);
            network.AddUndirectedEdge(node1_3, node1_4, costFuncTransit);
            network.AddUndirectedEdge(node1_4, node1_5, costFuncTransit);
            network.AddUndirectedEdge(node1_5, node1_6, costFuncTransit);
            network.AddUndirectedEdge(node1_6, node1_7, costFuncTransit);
            network.AddUndirectedEdge(node1_7, node1_8, costFuncTransit);
            network.AddUndirectedEdge(node1_8, node1_9, costFuncTransit);
            network.AddUndirectedEdge(node1_9, node1_10, costFuncTransit);
            network.AddUndirectedEdge(node1_10, node1_11, costFuncTransit);
            network.AddUndirectedEdge(node1_11, node1_12, costFuncTransit);
            network.AddUndirectedEdge(node1_12, node1_13, costFuncTransit);

            // line 2
            network.AddUndirectedEdge(node2_1, node2_2, costFuncTransit);
            network.AddUndirectedEdge(node2_2, node2_3, costFuncTransit);
            network.AddUndirectedEdge(node2_3, node2_4, costFuncTransit);
            network.AddUndirectedEdge(node2_4, node2_5, costFuncTransit);
            network.AddUndirectedEdge(node2_5, node2_6, costFuncTransit);
            network.AddUndirectedEdge(node2_6, node2_7, costFuncTransit);
            network.AddUndirectedEdge(node2_7, node2_8, costFuncTransit);
            network.AddUndirectedEdge(node2_8, node2_9, costFuncTransit);
            network.AddUndirectedEdge(node2_9, node2_10, costFuncTransit);
            network.AddUndirectedEdge(node2_10, node2_11, costFuncTransit);

            network.AddUndirectedEdge(node1_7, node2_7, costFuncChanging);

            return network;
        }
    }
}
