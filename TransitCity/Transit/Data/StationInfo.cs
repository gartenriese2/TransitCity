using System;
using System.Collections.Generic;
using System.Linq;
using Time;

namespace Transit.Data
{
    public class StationInfo
    {
        private readonly WeekTimePoint[] _arrivalsArray;
        private readonly WeekTimePoint[] _departuresArray;
        private readonly List<Trip> _trips;
        private readonly List<Trip> _tripsSortedByArrival;
        private readonly List<Trip> _tripsSortedByDeparture;

        public StationInfo(Station station, TransferStation transferStation, WeekTimeCollection arrivals, WeekTimeCollection departures, List<Trip> trips)
        {
            Station = station ?? throw new ArgumentNullException(nameof(station));
            TransferStation = transferStation ?? throw new ArgumentNullException(nameof(transferStation));

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
        }

        public Station Station { get; }

        public TransferStation TransferStation { get; }

        public IEnumerable<Trip> Trips => _trips;

        public (WeekTimePoint, Trip) GetNextDepartureAndTripArrayBinarySearch(WeekTimePoint time)
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

        public (WeekTimePoint, Trip) GetLastArrivalAndTripArrayBinarySearch(WeekTimePoint time)
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
