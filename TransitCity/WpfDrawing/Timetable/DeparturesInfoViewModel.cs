using System;
using System.Collections.Generic;
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

            DepartureInfoCollection.Add(new DepartureData(0, new List<int>{ 9, 19, 29, 39, 47 }, new List<int>{ 9, 19, 29, 39, 47 }, new List<int>{ 9, 19, 29, 39, 47 }, new List<int>{ 9, 19, 29, 39, 47 }));
            DepartureInfoCollection.Add(new DepartureData(1, new List<int>(), new List<int>(), new List<int>(), new List<int>()));
            DepartureInfoCollection.Add(new DepartureData(2, new List<int>(), new List<int>(), new List<int>(), new List<int>()));
            DepartureInfoCollection.Add(new DepartureData(3, new List<int>(), new List<int>(), new List<int>(), new List<int>()));
            DepartureInfoCollection.Add(new DepartureData(4, new List<int> { 51, 58 }, new List<int> { 51, 58 }, new List<int> { 8 }, new List<int> { 8, 48 }));
            DepartureInfoCollection.Add(new DepartureData(5, new List<int> { 6, 13, 20, 26, 33, 40, 46, 53 }, new List<int> { 6, 13, 20, 26, 33, 40, 46, 53 }, new List<int> { 8 }, new List<int> { 8, 48 }));
            DepartureInfoCollection.Add(new DepartureData(6, new List<int> { 0, 6, 13, 20, 26, 33, 39, 44, 49, 54, 59 }, new List<int> { 0, 6, 13, 20, 26, 33, 39, 44, 49, 54, 59 }, new List<int> { 8 }, new List<int> { 8, 48 }));
            DepartureInfoCollection.Add(new DepartureData(7, new List<int> { 4, 9 , 14, 19, 24, 29, 34, 39, 44, 49, 54, 59 }, new List<int> { 4, 9, 14, 19, 24, 29, 34, 39, 44, 49, 54, 59 }, new List<int> { 8 }, new List<int> { 8, 48 }));
            DepartureInfoCollection.Add(new DepartureData(8, new List<int> { 4, 9, 14, 19, 23, 26, 33, 39, 46, 53, 59 }, new List<int> { 4, 9, 14, 19, 23, 26, 33, 39, 46, 53, 59 }, new List<int> { 8 }, new List<int> { 8, 48 }));
            DepartureInfoCollection.Add(new DepartureData(9, new List<int> { 6, 13, 20, 26, 33, 40, 46, 53 }, new List<int> { 6, 13, 20, 26, 33, 40, 46, 53 }, new List<int> { 8 }, new List<int> { 8, 48 }));
            DepartureInfoCollection.Add(new DepartureData(10, new List<int> { 0, 6, 13, 20, 26, 33, 40, 46, 53 }, new List<int> { 0, 6, 13, 20, 26, 33, 40, 46, 53 }, new List<int> { 8 }, new List<int> { 8, 48 }));
            DepartureInfoCollection.Add(new DepartureData(11, new List<int> { 0, 6, 13, 20, 26, 33, 40, 46, 53 }, new List<int> { 0, 6, 13, 20, 26, 33, 40, 46, 53 }, new List<int> { 8 }, new List<int> { 8, 48 }));
            DepartureInfoCollection.Add(new DepartureData(12, new List<int> { 0, 6, 13, 20, 26, 33, 39, 44, 49, 54, 59 }, new List<int> { 0, 6, 13, 20, 26, 33, 39, 44, 49, 54, 59 }, new List<int> { 8 }, new List<int> { 8, 48 }));
            DepartureInfoCollection.Add(new DepartureData(13, new List<int> { 4, 9, 14, 19, 24, 29, 34, 39, 44, 49, 54, 59 }, new List<int> { 4, 9, 14, 19, 24, 29, 34, 39, 44, 49, 54, 59 }, new List<int> { 8 }, new List<int> { 8, 48 }));
            DepartureInfoCollection.Add(new DepartureData(14, new List<int> { 4, 9, 14, 19, 24, 29, 34, 39, 44, 49, 54, 59 }, new List<int> { 4, 9, 14, 19, 24, 29, 34, 39, 44, 49, 54, 59 }, new List<int> { 8 }, new List<int> { 8, 48 }));
            DepartureInfoCollection.Add(new DepartureData(15, new List<int> { 4, 9, 14, 19, 24, 29, 34, 39, 44, 49, 54, 59 }, new List<int> { 4, 9, 14, 19, 24, 29, 34, 39, 44, 49, 54, 59 }, new List<int> { 8 }, new List<int> { 8, 48 }));
            DepartureInfoCollection.Add(new DepartureData(16, new List<int> { 4, 9, 14, 19, 24, 29, 34, 39, 44, 49, 54, 59 }, new List<int> { 4, 9, 14, 19, 24, 29, 34, 39, 44, 49, 54, 59 }, new List<int> { 8 }, new List<int> { 8, 48 }));
            DepartureInfoCollection.Add(new DepartureData(17, new List<int> { 4, 9, 14, 19, 24, 29, 34, 39, 44, 49, 54, 59 }, new List<int> { 4, 9, 14, 19, 24, 29, 34, 39, 44, 49, 54, 59 }, new List<int> { 8 }, new List<int> { 8, 48 }));
            DepartureInfoCollection.Add(new DepartureData(18, new List<int> { 4, 9, 14, 19, 24, 29, 34, 39, 44, 49, 54, 59 }, new List<int> { 4, 9, 14, 19, 24, 29, 34, 39, 44, 49, 54, 59 }, new List<int> { 8 }, new List<int> { 8, 48 }));
            DepartureInfoCollection.Add(new DepartureData(19, new List<int> { 2, 6, 13, 20, 26, 33, 40, 46, 53 }, new List<int> { 2, 6, 13, 20, 26, 33, 40, 46, 53 }, new List<int> { 8 }, new List<int> { 8, 48 }));
            DepartureInfoCollection.Add(new DepartureData(20, new List<int> { 0, 6, 13, 20, 26, 33, 40, 49, 59 }, new List<int> { 0, 6, 13, 20, 26, 33, 40, 49, 59 }, new List<int> { 8 }, new List<int> { 8, 48 }));
            DepartureInfoCollection.Add(new DepartureData(21, new List<int> { 9, 19, 29, 39, 49, 59 }, new List<int> { 9, 19, 29, 39, 49, 59 }, new List<int> { 8 }, new List<int> { 8, 48 }));
            DepartureInfoCollection.Add(new DepartureData(22, new List<int> { 9, 19, 29, 39, 49, 59 }, new List<int> { 9, 19, 29, 39, 49, 59 }, new List<int> { 8 }, new List<int> { 8, 48 }));
            DepartureInfoCollection.Add(new DepartureData(23, new List<int> { 9, 19, 29, 39, 49, 59 }, new List<int> { 9, 19, 29, 39, 49, 59 }, new List<int> { 8 }, new List<int> { 8, 48 }));
        }

        public DeparturesInfoViewModel(StationInfo stationInfo)
        {
            
        }

        public ObservableCollection<DepartureData> DepartureInfoCollection { get; } = new ObservableCollection<DepartureData>();
    }
}
