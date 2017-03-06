using System.Collections.Generic;
using Geometry;

namespace Transit
{
    public class TransferStation<P> where P : IPosition
    {
        private readonly List<Station<P>> _stations = new List<Station<P>>();

        public TransferStation(string name)
        {
            Name = name;
        }

        public TransferStation(string name, params Station<P>[] stations)
        {
            Name = name;
            _stations.AddRange(stations);
        }

        public string Name { get; }

        public IEnumerable<Station<P>> Stations => _stations;

        public override string ToString()
        {
            return Name;
        }

        public void AddStation(Station<P> station)
        {
            if (!_stations.Contains(station))
            {
                _stations.Add(station);
            }
        }
    }
}
