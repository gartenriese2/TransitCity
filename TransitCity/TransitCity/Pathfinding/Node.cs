namespace TransitCity.Pathfinding
{
    using System;
    using System.Collections.Generic;

    using Utility.Coordinates;

    public class Node
    {
        //---------------------------------------------------------------------
        // Constructors
        //---------------------------------------------------------------------
        public Node(WorldPosition pos, NodeInfo info)
        {
            WorldPosition = pos;
            Info = info;
        }

        //---------------------------------------------------------------------
        // Properties
        //---------------------------------------------------------------------
        public WorldPosition WorldPosition { get; }

        public NodeInfo Info { get; }

        public Dictionary<Node, List<PathInfo>> NextNodes { get; } = new Dictionary<Node, List<PathInfo>>();

        public List<Node> PreviousNodes { get; } = new List<Node>();

        public bool Removed { get; set; } = false;

        //---------------------------------------------------------------------
        // Methods
        //---------------------------------------------------------------------
        public static void Connect(Node from, Node to, PathInfo info)
        {
            from.AddNextNode(to, info);
            to.AddPreviousNode(from);
        }

        public static void ConnectBidirectional(Node from, Node to, PathInfo info)
        {
            from.AddNextNode(to, info);
            from.AddPreviousNode(to);
            to.AddNextNode(from, info);
            to.AddPreviousNode(from);
        }

        public override string ToString()
        {
            return "(" + WorldPosition.X + "|" + WorldPosition.Y + ")";
        }

        public bool SamePos(Node other)
        {
            return Math.Abs(WorldPosition.X - other.WorldPosition.X) < float.Epsilon && Math.Abs(WorldPosition.Y - other.WorldPosition.Y) < float.Epsilon;
        }

        public bool AddNextNode(Node node, PathInfo info)
        {
            if (!NextNodes.ContainsKey(node))
            {
                NextNodes.Add(node, new List<PathInfo> { info });
                return true;
            }

            if (!NextNodes[node].Contains(info))
            {
                NextNodes[node].Add(info);
                return true;
            }

            return false;
        }

        public bool AddPreviousNode(Node node)
        {
            if (!PreviousNodes.Contains(node))
            {
                PreviousNodes.Add(node);
                return true;
            }

            return false;
        }

        public bool RemoveNextNode(Node node)
        {
            return NextNodes.Remove(node);
        }

        public bool RemovePreviousNode(Node node)
        {
            return PreviousNodes.Remove(node);
        }

        public Utility.Units.Distance GetDistanceTo(Node other)
        {
            return new Utility.Units.Distance((float)Math.Sqrt(Math.Pow(WorldPosition.X - other.WorldPosition.X, 2) + Math.Pow(WorldPosition.Y - other.WorldPosition.Y, 2)));
        }
    }
}