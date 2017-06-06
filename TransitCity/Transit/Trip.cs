using System;
using System.Collections.Generic;
using System.Linq;
using Time;

namespace Transit
{
    using Arrival = WeekTimePoint;
    using Departure = WeekTimePoint;

    public class Trip : WeekTimeSpan
    {
        private readonly List<(Station, (Arrival, Departure))> _stationTimes;
        private readonly List<Departure> _departureTimes;
        private readonly List<Arrival> _arrivalTimes;
        private readonly List<Station> _stations;
        private readonly Dictionary<Station, Arrival> _arrivals;
        private readonly Dictionary<Station, Departure> _departures;

        public Trip(List<(Station, (Arrival, Departure))> stationTimes)
            : base(stationTimes?[0].Item2.Item2, stationTimes?[stationTimes.Count - 1].Item2.Item1)
        {
            _stationTimes = stationTimes ?? throw new ArgumentNullException(nameof(stationTimes));
            _departureTimes = _stationTimes.Select(tuple => tuple.Item2.Item2).ToList();
            _arrivalTimes = _stationTimes.Select(tuple => tuple.Item2.Item1).ToList();
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

            return _arrivalTimes.Skip(idx + 1);
        }

        public IEnumerable<Departure> GetLastDepartures(Station station)
        {
            var idx = _stationTimes.FindIndex(vt => vt.Item1 == station);
            if (idx == -1)
            {
                throw new ArgumentException();
            }

            return _departureTimes.Take(idx).Reverse();
        }

        public IEnumerable<Departure> GetNextDepartures(Station station)
        {
            var idx = _stationTimes.FindIndex(vt => vt.Item1 == station);
            if (idx == -1)
            {
                throw new ArgumentException();
            }

            return _departureTimes.Skip(idx + 1);
        }
    }
}
