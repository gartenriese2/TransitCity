using System.Collections.Generic;

namespace Transit
{
    public class TransferStation
    {
        private readonly List<Station> _stations = new List<Station>();

        public TransferStation(string name)
        {
            Name = name;
        }

        public TransferStation(string name, params Station[] stations)
        {
            Name = name;
            _stations.AddRange(stations);
        }

        public string Name { get; }

        public IEnumerable<Station> Stations => _stations;

        public override string ToString()
        {
            return Name;
        }

        public void AddStation(Station station)
        {
            if (!_stations.Contains(station))
            {
                _stations.Add(station);
            }
        }
    }
}
