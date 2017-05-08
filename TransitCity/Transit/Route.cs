using System;
using System.Collections.Generic;
using System.Linq;

namespace Transit
{
    public class Route
    {
        private readonly List<Station> _stations = new List<Station>();

        public Route(IEnumerable<Station> stations)
        {
            if (stations == null || stations.Count() < 2)
            {
                throw new ArgumentException();
            }

            _stations.AddRange(stations);
        }

        public IEnumerable<Station> Stations => _stations;

        public override string ToString()
        {
            return $"{_stations.First()} -> {_stations.Last()}";
        }

        public IEnumerable<Station> GetNextStations(Station station)
        {
            if (!_stations.Contains(station))
            {
                throw new InvalidOperationException();
            }

            var idx = _stations.IndexOf(station);
            return _stations.TakeWhile((s, i) => i > idx);
        }

        public Station GetNextStation(Station station)
        {
            if (!_stations.Contains(station))
            {
                throw new InvalidOperationException();
            }

            var idx = _stations.IndexOf(station);

            if (idx == _stations.Count - 1)
            {
                return null;
            }

            return _stations[idx + 1];
        }
    }
}
