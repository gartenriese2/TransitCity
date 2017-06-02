using System;
using System.Collections.ObjectModel;
using System.Linq;
using Time;
using Transit.Data;
using Utility.MVVM;

namespace WpfDrawing.Timetable
{
    public class NextStationsInfoViewModel : ViewModelBase
    {
        public NextStationsInfoViewModel()
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException();
            }

            CurrentStationName = "Stratford";
            Names.Add("Leyton");
            Minutes.Add(2);

            Names.Add("Leytonstone");
            Minutes.Add(5);

            Names.Add("Snaresbrook");
            Minutes.Add(8);

            Names.Add("South Woodford");
            Minutes.Add(10);

            Names.Add("Woodford");
            Minutes.Add(12);

            Names.Add("Buckhurst Hill");
            Minutes.Add(14);

            Names.Add("Loughton");
            Minutes.Add(17);

            Names.Add("Debden");
            Minutes.Add(20);

            Names.Add("Theydon Bois");
            Minutes.Add(23);

            Names.Add("Epping");
            Minutes.Add(26);
        }

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
