using System;
using System.Collections.Generic;
using System.Linq;
using Time;

namespace Transit
{
    using Arrival = WeekTimePoint;
    using Departure = WeekTimePoint;

    public class Trip
    {
        private readonly List<(Station, (Arrival, Departure))> _stationTimes;
        private readonly List<Station> _stations;
        private readonly Dictionary<Station, Arrival> _arrivals;
        private readonly Dictionary<Station, Departure> _departures;

        public Trip(List<(Station, (Arrival, Departure))> stationTimes)
        {
            _stationTimes = stationTimes ?? throw new ArgumentNullException(nameof(stationTimes));
            _stations = new List<Station>(_stationTimes.Select(x => x.Item1));
            _arrivals = stationTimes.ToDictionary(t => t.Item1, t => t.Item2.Item1);
            _departures = stationTimes.ToDictionary(t => t.Item1, t => t.Item2.Item2);
        }

        public IEnumerable<Station> Stations => _stations;

        public Station GetNextStation(Station from)
        {
            var stationList = Stations.ToList();
            var idx = stationList.IndexOf(from);
            if (idx == -1)
            {
                throw new ArgumentException();
            }

            return idx == stationList.Count - 1 ? null : stationList[idx + 1];
        }

        public Arrival ArrivalAtStation(Station station)
        {
            return _arrivals[station];
        }

        public Departure DepartureAtStation(Station station)
        {
            return _departures[station];
        }

        public IEnumerable<Arrival> GetNextArrivals(Station station)
        {
            var idx = _stationTimes.FindIndex(vt => vt.Item1 == station);
            if (idx == -1)
            {
                throw new ArgumentException();
            }

            return _stationTimes.Skip(idx + 1).Select(vt => vt.Item2.Item1);
        }

        public IEnumerable<Departure> GetLastDepartures(Station station)
        {
            var idx = _stationTimes.FindIndex(vt => vt.Item1 == station);
            if (idx == -1)
            {
                throw new ArgumentException();
            }

            return _stationTimes.Take(idx).Reverse().Select(vt => vt.Item2.Item2);
        }

        public IEnumerable<Departure> GetNextDepartures(Station station)
        {
            var idx = _stationTimes.FindIndex(vt => vt.Item1 == station);
            if (idx == -1)
            {
                throw new ArgumentException();
            }

            return _stationTimes.Skip(idx + 1).Select(vt => vt.Item2.Item2);
        }
    }
}
