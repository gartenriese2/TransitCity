using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;

namespace Transit
{
    public class Route<P> where P : IPosition
    {
        private readonly List<Station<P>> _stations = new List<Station<P>>();

        public Route(IEnumerable<Station<P>> stations)
        {
            if (stations == null || stations.Count() < 2)
            {
                throw new ArgumentException();
            }

            _stations.AddRange(stations);
        }

        public IEnumerable<Station<P>> Stations => _stations;

        public override string ToString()
        {
            return $"{_stations.First()} -> {_stations.Last()}";
        }

        public IEnumerable<Station<P>> GetNextStations(Station<P> station)
        {
            if (!_stations.Contains(station))
            {
                throw new InvalidOperationException();
            }

            var idx = _stations.IndexOf(station);
            return _stations.TakeWhile((s, i) => i > idx);
        }

        public Station<P> GetNextStation(Station<P> station)
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
