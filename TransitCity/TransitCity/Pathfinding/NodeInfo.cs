namespace TransitCity.Pathfinding
{
    using System.Collections.Generic;

    public class NodeInfo
    {

        //---------------------------------------------------------------------
        // Enumerations
        //---------------------------------------------------------------------
        public enum AllowedType
        {
            Pedstrian, Car, Metro
        }

        //---------------------------------------------------------------------
        // Properties
        //---------------------------------------------------------------------
        public List<AllowedType> AllowedTypes { get; set; } = new List<AllowedType>();

        public bool Public { get; set; }

        public Dictionary<AllowedType, Utility.Units.Time> TimePenalties { get; set; } = new Dictionary<AllowedType, Utility.Units.Time>();
    }
}