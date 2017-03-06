namespace TransitCity.Models
{
    using System.Collections.Generic;

    using MVVM;

    using Utility.Coordinates;

    public class ResidentialBuildingModel : PropertyChangedBase
    {
        public ResidentialBuildingModel(ModelPosition pos, List<ResidentModel> residents)
        {
            Position = pos;
            Residents = residents;
            NumResidents = residents.Count;
        }

        public ModelPosition Position { get; }

        public List<ResidentModel> Residents { get; }

        public int NumResidents { get; }
    }
}
