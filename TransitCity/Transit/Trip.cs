using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Time;

namespace Transit
{
    using Arrival = WeekTimePoint;
    using Departure = WeekTimePoint;

    public class Trip<TPos> where TPos : IPosition
    {
        private readonly List<(Station<TPos>, (Arrival, Departure))> _stationTimes;
        private readonly Dictionary<Station<TPos>, Arrival> _arrivals;
        private readonly Dictionary<Station<TPos>, Departure> _departures;

        public Trip(List<(Station<TPos>, (Arrival, Departure))> stationTimes)
        {
            _stationTimes = stationTimes ?? throw new ArgumentNullException(nameof(stationTimes));
            _arrivals = stationTimes.ToDictionary(t => t.Item1, t => t.Item2.Item1);
            _departures = stationTimes.ToDictionary(t => t.Item1, t => t.Item2.Item2);
        }

        public IEnumerable<Station<TPos>> Stations => _stationTimes.Select(x => x.Item1);

        public Station<TPos> GetNextStation(Station<TPos> from)
        {
            var stationList = Stations.ToList();
            var idx = stationList.IndexOf(from);
            if (idx == -1)
            {
                throw new ArgumentException();
            }

            return idx == stationList.Count - 1 ? null : stationList[idx + 1];
        }

        public Arrival ArrivalAtStation(Station<TPos> station)
        {
            return _arrivals[station];
        }

        public Departure DepartureAtStation(Station<TPos> station)
        {
            return _departures[station];
        }

        public IEnumerable<Arrival> GetNextArrivals(Station<TPos> station)
        {
            var idx = _stationTimes.FindIndex(vt => vt.Item1 == station);
            if (idx == -1)
            {
                throw new ArgumentException();
            }

            return _stationTimes.Skip(idx + 1).Select(vt => vt.Item2.Item1);
        }

        public IEnumerable<Departure> GetNextDepartures(Station<TPos> station)
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
