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

        public StationInfo(Station<Position2f> station, WeekTimeCollection arrivals, WeekTimeCollection departures, List<Trip<Position2f>> trips)
        {
            Station = station ?? throw new ArgumentNullException(nameof(station));
            Arrivals = arrivals ?? throw new ArgumentNullException(nameof(arrivals));
            Departures = departures ?? throw new ArgumentNullException(nameof(departures));
            _arrivalsArray = arrivals.SortedWeekTimePoints.ToArray();
            _departuresArray = departures.SortedWeekTimePoints.ToArray();
            _trips = trips;

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

        public WeekTimeCollection Arrivals { get; }

        public WeekTimeCollection Departures { get; }

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

            var idx = Arrivals.IndexOf(arrival);
            if (idx == -1)
            {
                throw new ArgumentException();
            }

            return Departures[idx];
        }

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
                return (_departuresArray[0], _trips[0]);
            }

            if (idx >= 0)
            {
                return (_departuresArray[idx], _trips[idx]);
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
                return (_arrivalsArray[idx], _trips[idx]);
            }

            if (idx < 0)
            {
                idx = ~idx;
            }

            if (idx == 0)
            {
                return (_arrivalsArray.Last(), _trips.Last());
            }

            if (idx > 0)
            {
                return (_arrivalsArray[idx - 1], _trips[idx - 1]);
            }

            throw new InvalidOperationException();
        }
    }
}
