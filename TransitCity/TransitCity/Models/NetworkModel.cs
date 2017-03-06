namespace TransitCity.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using Transit;

    using Utility.Coordinates;

    public class NetworkModel
    {
        public List<Line> Lines { get; } = new List<Line>();

        private IEnumerable<Station> Stations { get; } = new List<Station>();

        public IEnumerable<Station> GetStationsNearPosition(ModelPosition position, double distance)
        {
            return Stations.Where(s => s.Position.GetDistanceTo(position) <= distance).OrderBy(s => s.Position.GetDistanceTo(position));
        }

        public IEnumerable<Station> GetStationsSortedByDistance(ModelPosition position)
        {
            return SortStationsByDistance(position, Stations);
        }

        public IEnumerable<Station> SortStationsByDistance(ModelPosition position, IEnumerable<Station> stations)
        {
            return stations.OrderBy(s => s.Position.GetDistanceTo(position));
        }
    }
}
