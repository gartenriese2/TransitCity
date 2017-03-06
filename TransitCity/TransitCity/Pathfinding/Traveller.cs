namespace TransitCity.Pathfinding
{
    using System.Collections.Generic;
    using System.Linq;

    public struct Traveller
    {
        // ---------------------------------------------------------------------
        // Properties
        // ---------------------------------------------------------------------
        public List<NodeInfo.AllowedType> ReusableTypes { get; private set; }

        public List<NodeInfo.AllowedType> NonReusableTypes { get; private set; }

        public List<NodeInfo.AllowedType> AllTypes => ReusableTypes.Concat(NonReusableTypes).ToList();

        public NodeInfo.AllowedType CurrentType { get; set; }

        public List<Node> Keys { get; private set; }

        // ---------------------------------------------------------------------
        // Methods
        // ---------------------------------------------------------------------
        public static Traveller Create()
        {
            return new Traveller
            {
                ReusableTypes = new List<NodeInfo.AllowedType> { NodeInfo.AllowedType.Pedstrian, NodeInfo.AllowedType.Metro }, 
                NonReusableTypes = new List<NodeInfo.AllowedType>(), 
                CurrentType = NodeInfo.AllowedType.Pedstrian, 
                Keys = new List<Node>()
            };
        }

        public static Traveller Copy(Traveller other)
        {
            var newTraveller = Create();
            newTraveller.ReusableTypes.Clear();
            foreach (var reusableType in other.ReusableTypes)
            {
                newTraveller.ReusableTypes.Add(reusableType);
            }

            foreach (var nonReusableType in other.NonReusableTypes)
            {
                newTraveller.NonReusableTypes.Add(nonReusableType);
            }

            newTraveller.CurrentType = other.CurrentType;
            foreach (var key in other.Keys)
            {
                newTraveller.Keys.Add(key);
            }

            return newTraveller;
        }
    }
}