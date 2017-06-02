using System;
using Transit.Data;
using Utility.MVVM;

namespace WpfDrawing.Timetable
{
    public class TimetableViewModel : ViewModelBase
    {
        public TimetableViewModel()
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException();
            }

            CurrentStationInfoViewModel = new CurrentStationInfoViewModel();
            NextStationsInfoViewModel = new NextStationsInfoViewModel();
        }

        public TimetableViewModel(StationInfo stationInfo, LineInfo lineInfo, RouteInfo routeInfo)
        {
            CurrentStationInfoViewModel = new CurrentStationInfoViewModel(stationInfo, lineInfo, routeInfo);
            NextStationsInfoViewModel = new NextStationsInfoViewModel(stationInfo, routeInfo);
        }

        public CurrentStationInfoViewModel CurrentStationInfoViewModel { get; }

        public NextStationsInfoViewModel NextStationsInfoViewModel { get; }
    }
}
