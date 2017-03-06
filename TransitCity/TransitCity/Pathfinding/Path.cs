namespace TransitCity.Pathfinding
{
    using System.Collections.Generic;

    public struct PathInfo
    {
        //---------------------------------------------------------------------
        // Properties
        //---------------------------------------------------------------------
        public NodeInfo.AllowedType Type { get; set; }

        public Utility.Units.Speed Speed { get; set; }

        public bool Hidden { get; set; }
    }

    public class Path
    {
        //---------------------------------------------------------------------
        // Constructors
        //---------------------------------------------------------------------
        public Path(List<Node> nodes, List<PathInfo> infos)
        {
            Nodes = nodes;
            Infos = infos;
        }

        //---------------------------------------------------------------------
        // Properties
        //---------------------------------------------------------------------
        public List<Node> Nodes { get; }

        public List<PathInfo> Infos { get; private set; }

        //---------------------------------------------------------------------
        // Methods
        //---------------------------------------------------------------------
        public Utility.Units.Distance GetLength()
        {
            var len = new Utility.Units.Distance(0);
            for (var i = 0; i < Nodes.Count - 1; ++i)
            {
                len += Nodes[i].GetDistanceTo(Nodes[i + 1]);
            }

            return len;
        }
    }
}