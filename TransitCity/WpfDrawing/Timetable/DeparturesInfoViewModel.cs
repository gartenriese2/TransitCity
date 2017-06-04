using System;
using System.Collections.ObjectModel;
using Transit.Data;
using Utility.MVVM;

namespace WpfDrawing.Timetable
{
    public class DeparturesInfoViewModel : ViewModelBase
    {
        public DeparturesInfoViewModel()
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException();
            }

            DepartureInfoCollection.Add(new DepartureData
            {
                Hour = 0,
                MinutesWeekday = { 8, 18, 28, 38, 48, 58 },
                MinutesFriday = { 8, 28, 48 },
                MinutesSaturday = { 8, 38 }
            });
            DepartureInfoCollection.Add(new DepartureData
            {
                Hour = 1,
                MinutesWeekday = { 8, 18, 28, 38, 48, 58 },
                MinutesFriday = { 8, 28, 48 },
                MinutesSaturday = { 8, 38 }
            });
            DepartureInfoCollection.Add(new DepartureData
            {
                Hour = 2,
                MinutesWeekday = { 8, 18, 28, 38, 48, 58 },
                MinutesFriday = { 8, 28, 48 },
                MinutesSaturday = { 8, 38 }
            });
        }

        public DeparturesInfoViewModel(StationInfo stationInfo)
        {
            
        }

        public ObservableCollection<DepartureData> DepartureInfoCollection { get; } = new ObservableCollection<DepartureData>();
    }
}
