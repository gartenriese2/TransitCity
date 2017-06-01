using System;
using System.Collections.ObjectModel;
using System.Linq;
using Time;
using Transit.Data;

namespace WpfDrawing.Timetable
{
    public class NextStationsInfoViewModel
    {
        public NextStationsInfoViewModel(StationInfo stationInfo, RouteInfo routeInfo)
        {
            CurrentStationName = stationInfo.TransferStation.Name;
            var idx = routeInfo.StationInfos.IndexOf(stationInfo);
            for (var i = idx + 1; i < routeInfo.StationInfos.Count; ++i)
            {
                Names.Add(routeInfo.StationInfos[i].TransferStation.Name);
            }

            var trip = stationInfo.Trips.First();
            var departure = trip.DepartureAtStation(stationInfo.Station);
            var arrivals = trip.GetNextArrivals(stationInfo.Station);
            foreach (var arrival in arrivals)
            {
                Minutes.Add((int) Math.Round(WeekTimePoint.GetCorrectedDifference(departure, arrival).TotalMinutes));
            }

            if (Names.Count != Minutes.Count)
            {
                throw new InvalidOperationException();
            }
        }

        public string CurrentStationName { get; }

        public ObservableCollection<string> Names { get; } = new ObservableCollection<string>();

        public ObservableCollection<int> Minutes { get; } = new ObservableCollection<int>();
    }
}
