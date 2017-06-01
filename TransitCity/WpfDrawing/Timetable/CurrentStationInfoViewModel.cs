using System.Linq;
using Transit;
using Transit.Data;
using Utility.MVVM;

namespace WpfDrawing.Timetable
{
    public class CurrentStationInfoViewModel : PropertyChangedBase
    {
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
