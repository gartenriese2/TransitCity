namespace TransitCity.Models
{
    using System.Collections.Generic;

    public class MetroConnectionModel
    {
        public MetroConnectionModel(MetroStationModel stationA, MetroStationModel stationB, List<MetroLineModel> lines)
        {
            StationA = stationA;
            StationB = stationB;
            Lines = lines;
        }

        public MetroStationModel StationA { get; }

        public MetroStationModel StationB { get; }

        public List<MetroLineModel> Lines { get; }
    }
}
