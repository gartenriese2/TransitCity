using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using PathFinding.Network;

namespace Transit
{
    public class TransitNetwork<P> : Network<P, TimeEdgeCost> where P : IPosition
    {
        private readonly Dictionary<Station<P>, Tuple<Node<P>, Node<P>, Node<P>>> _stationNodeDictionary = new Dictionary<Station<P>, Tuple<Node<P>, Node<P>, Node<P>>>();
        private readonly List<TransferStation<P>> _transferStations = new List<TransferStation<P>>();

        public void ConnectLine(Line<P> line, Func<Node<P>, Node<P>, TimeEdgeCost> transitCostFunc)
        {
            foreach (var route in line.Routes)
            {
                ConnectRoute(route, transitCostFunc);
                ConnectEntryExit(route, (a, b) => new TimeEdgeCost(route.Frequency / 2), (a, b) => new TimeEdgeCost(10f)); // 10 seconds exit time
            }
        }

        public void ConnectTransferStation(TransferStation<P> transferStation, Func<Node<P>, Node<P>, TimeEdgeCost> walkingCostFunc)
        {
            if (_transferStations.Contains(transferStation))
            {
                return;
            }

            _transferStations.Add(transferStation);

            for (var i = 0; i < transferStation.Stations.Count() - 1; ++i)
            {
                var exitNode = GetExitNode(transferStation.Stations.ElementAt(i));
                for (var j = i + 1; j < transferStation.Stations.Count(); ++j)
                {
                    var entryNode = GetEntryNode(transferStation.Stations.ElementAt(j));
                    AddDirectedEdge(exitNode, entryNode, walkingCostFunc);
                }
            }
        }

        public Node<P> ConnectToStations(P position, float radius, Func<Node<P>, Node<P>, TimeEdgeCost> walkingCostFunc)
        {
            var node = CreateNode(position);
            foreach (var station in _transferStations.SelectMany(t => t.Stations))
            {
                if (station.Position.DistanceTo(position) <= radius)
                {
                    AddDirectedEdge(node, GetEntryNode(station), walkingCostFunc);
                    AddDirectedEdge(GetExitNode(station), node, walkingCostFunc);
                }
            }

            return node;
        }

        private void ConnectRoute(Route<P> route, Func<Node<P>, Node<P>, TimeEdgeCost> transitCostFunc)
        {
            for (var i = 0; i < route.Stations.Count() - 1; ++i)
            {
                var nodeA = GetTransitNode(route.Stations.ElementAt(i));
                var nodeB = GetTransitNode(route.Stations.ElementAt(i + 1));
                AddDirectedEdge(nodeA, nodeB, transitCostFunc);
            }
        }

        private void ConnectEntryExit(Route<P> route, Func<Node<P>, Node<P>, TimeEdgeCost> entryCostFunc, Func<Node<P>, Node<P>, TimeEdgeCost> exitCostFunc)
        {
            foreach (var station in route.Stations)
            {
                var tuple = GetNodeTuple(station);
                var entryNode = tuple.Item1;
                var exitNode = tuple.Item3;
                var node = tuple.Item2;
                AddDirectedEdge(entryNode, node, entryCostFunc);
                AddDirectedEdge(node, exitNode, exitCostFunc);
            }
        }

        private Node<P> GetEntryNode(Station<P> station)
        {
            return GetNodeTuple(station).Item1;
        }

        private Node<P> GetTransitNode(Station<P> station)
        {
            return GetNodeTuple(station).Item2;
        }

        private Node<P> GetExitNode(Station<P> station)
        {
            return GetNodeTuple(station).Item3;
        }

        private Tuple<Node<P>, Node<P>, Node<P>> GetNodeTuple(Station<P> station)
        {
            if (!_stationNodeDictionary.ContainsKey(station))
            {
                _stationNodeDictionary.Add(station, new Tuple<Node<P>, Node<P>, Node<P>>(CreateNode(station.EntryPosition), CreateNode(station.Position), CreateNode(station.ExitPosition)));
            }

            return _stationNodeDictionary[station];
        }
    }
}
