using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using PathFinding.Network;

namespace Transit
{
    using Node = Node<Position2d>;

    public class TransitNetwork : Network<Position2d, TimeEdgeCost>
    {
        private readonly Dictionary<Station, Tuple<Node, Node, Node>> _stationNodeDictionary = new Dictionary<Station, Tuple<Node, Node, Node>>();
        private readonly List<TransferStation> _transferStations = new List<TransferStation>();

        public void ConnectLine(Line line, Func<Node, Node, TimeEdgeCost> transitCostFunc)
        {
            foreach (var route in line.Routes)
            {
                ConnectRoute(route, transitCostFunc);
                ConnectEntryExit(route, (a, b) => new TimeEdgeCost(120f), (a, b) => new TimeEdgeCost(10f)); // 10 seconds exit time, 240 seconds frequency TODO
            }
        }

        public void ConnectTransferStation(TransferStation transferStation, Func<Node, Node, TimeEdgeCost> walkingCostFunc)
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

        public Node ConnectToStations(Position2d position, float radius, Func<Node, Node, TimeEdgeCost> walkingCostFunc)
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

        private void ConnectRoute(Route route, Func<Node, Node, TimeEdgeCost> transitCostFunc)
        {
            for (var i = 0; i < route.Stations.Count() - 1; ++i)
            {
                var nodeA = GetTransitNode(route.Stations.ElementAt(i));
                var nodeB = GetTransitNode(route.Stations.ElementAt(i + 1));
                AddDirectedEdge(nodeA, nodeB, transitCostFunc);
            }
        }

        private void ConnectEntryExit(Route route, Func<Node, Node, TimeEdgeCost> entryCostFunc, Func<Node, Node, TimeEdgeCost> exitCostFunc)
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

        private Node GetEntryNode(Station station)
        {
            return GetNodeTuple(station).Item1;
        }

        private Node GetTransitNode(Station station)
        {
            return GetNodeTuple(station).Item2;
        }

        private Node GetExitNode(Station station)
        {
            return GetNodeTuple(station).Item3;
        }

        private Tuple<Node, Node, Node> GetNodeTuple(Station station)
        {
            if (!_stationNodeDictionary.ContainsKey(station))
            {
                _stationNodeDictionary.Add(station, new Tuple<Node, Node, Node>(CreateNode(station.EntryPosition), CreateNode(station.Position), CreateNode(station.ExitPosition)));
            }

            return _stationNodeDictionary[station];
        }
    }
}
