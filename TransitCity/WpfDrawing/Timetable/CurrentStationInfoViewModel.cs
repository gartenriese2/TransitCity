using System;
using System.Linq;
using Transit;
using Transit.Data;
using Utility.MVVM;

namespace WpfDrawing.Timetable
{
    public class CurrentStationInfoViewModel : ViewModelBase
    {
        public CurrentStationInfoViewModel()
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException();
            }

            CurrentStationName = "Stratford";
            LastStationName = "Epping";
            LineName = "Central";
            Type = TransitType.Subway;
        }

        public CurrentStationInfoViewModel(StationInfo stationInfo, LineInfo lineInfo, RouteInfo routeInfo)
        {
            CurrentStationName = stationInfo.TransferStation.Name;
            LastStationName = routeInfo.StationInfos.Last().TransferStation.Name;
            LineName = lineInfo.Line.Name;
            Type = lineInfo.Line.Type;
        }

        public string CurrentStationName { get; }

        public string LastStationName { get; }

        public string LineName { get; }

        public TransitType Type { get; }
    }
}
