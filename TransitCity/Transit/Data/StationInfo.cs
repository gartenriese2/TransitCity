using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Time;

namespace Transit.Data
{
    public class StationInfo
    {
        private readonly WeekTimePoint[] _arrivalsArray;
        private readonly WeekTimePoint[] _departuresArray;
        private readonly List<Trip<Position2f>> _trips;
        private readonly List<Trip<Position2f>> _tripsSortedByArrival;
        private readonly List<Trip<Position2f>> _tripsSortedByDeparture;

        public StationInfo(Station<Position2f> station, WeekTimeCollection arrivals, WeekTimeCollection departures, List<Trip<Position2f>> trips)
        {
            Station = station ?? throw new ArgumentNullException(nameof(station));
            Arrivals = arrivals ?? throw new ArgumentNullException(nameof(arrivals));
            Departures = departures ?? throw new ArgumentNullException(nameof(departures));

            _arrivalsArray = arrivals.SortedWeekTimePoints.ToArray();
            _departuresArray = departures.SortedWeekTimePoints.ToArray();
            _trips = trips;
            _tripsSortedByArrival = _trips.OrderBy(trip => trip.ArrivalAtStation(station)).ToList();
            _tripsSortedByDeparture = trips.OrderBy(trip => trip.DepartureAtStation(station)).ToList();

            for (var i = 0; i < _trips.Count; i++)
            {
                if (_arrivalsArray.Length == 0 && _tripsSortedByArrival[i].ArrivalAtStation(station) != null)
                {
                    throw new InvalidOperationException();
                }

                if (_arrivalsArray.Length != 0 && _tripsSortedByArrival[i].ArrivalAtStation(station) != _arrivalsArray[i])
                {
                    throw new InvalidOperationException();
                }

                if (_departuresArray.Length == 0 && _tripsSortedByDeparture[i].DepartureAtStation(station) != null)
                {
                    throw new InvalidOperationException();
                }

                if (_departuresArray.Length != 0 && _tripsSortedByDeparture[i].DepartureAtStation(station) != _departuresArray?[i])
                {
                    throw new InvalidOperationException();
                }
            }

            if (Arrivals.Count == 0 && Departures.Count == 0)
            {
                throw new ArgumentException();
            }

            if (Arrivals.Count != 0 && Departures.Count != 0 && Arrivals.Count != Departures.Count)
            {
                throw new ArgumentException();
            }

            if (Math.Max(Arrivals.Count, Departures.Count) != trips.Count)
            {
                throw new ArgumentException();
            }
        }

        public Station<Position2f> Station { get; }

        [Obsolete]
        public WeekTimeCollection Arrivals { get; }

        [Obsolete]
        public WeekTimeCollection Departures { get; }

        [Obsolete]
        public WeekTimePoint FindCorrespondingDeparture(WeekTimePoint arrival)
        {
            if (arrival == null)
            {
                throw new ArgumentNullException(nameof(arrival));
            }

            if (Departures.Count == 0)
            {
                return null;
            }

            var idx = Arrivals.SortedWeekTimePoints.ToList().IndexOf(arrival);
            if (idx == -1)
            {
                throw new ArgumentException();
            }

            return Departures.SortedWeekTimePoints.ElementAt(idx);
        }

        [Obsolete]
        public WeekTimePoint GetNextDeparture(WeekTimePoint time)
        {
            return Departures.SortedWeekTimePoints.FirstOrDefault(wtp => time <= wtp) ?? Departures.SortedWeekTimePoints.FirstOrDefault();
        }

        public WeekTimePoint GetNextDepartureArrayBinarySearch(WeekTimePoint time)
        {
            if (_departuresArray.Length == 0)
            {
                return null;
            }

            var idx = Array.BinarySearch(_departuresArray, time);
            if (idx < 0)
            {
                idx = ~idx;
            }

            if (idx == _departuresArray.Length)
            {
                return _departuresArray[0];
            }

            if (idx >= 0)
            {
                return _departuresArray[idx];
            }

            throw new InvalidOperationException();
        }

        public (WeekTimePoint, Trip<Position2f>) GetNextDepartureAndTripArrayBinarySearch(WeekTimePoint time)
        {
            if (_departuresArray.Length == 0)
            {
                return (null, null);
            }

            var idx = Array.BinarySearch(_departuresArray, time);
            if (idx < 0)
            {
                idx = ~idx;
            }

            if (idx == _departuresArray.Length)
            {
                return (_departuresArray[0], _tripsSortedByDeparture[0]);
            }

            if (idx >= 0)
            {
                return (_departuresArray[idx], _tripsSortedByDeparture[idx]);
            }

            throw new InvalidOperationException();
        }

        public (WeekTimePoint, Trip<Position2f>) GetLastArrivalAndTripArrayBinarySearch(WeekTimePoint time)
        {
            if (_arrivalsArray.Length == 0)
            {
                return (null, null);
            }

            var idx = Array.BinarySearch(_arrivalsArray, time);
            if (idx >= 0)
            {
                return (_arrivalsArray[idx], _tripsSortedByArrival[idx]);
            }

            if (idx < 0)
            {
                idx = ~idx;
            }

            if (idx == 0)
            {
                return (_arrivalsArray.Last(), _tripsSortedByArrival.Last());
            }

            if (idx > 0)
            {
                return (_arrivalsArray[idx - 1], _tripsSortedByArrival[idx - 1]);
            }

            throw new InvalidOperationException();
        }
    }
}
