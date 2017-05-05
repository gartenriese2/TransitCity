using System.Collections.ObjectModel;
using System.Windows.Media;
using WpfDrawing.Objects;

namespace WpfTestApp
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            PanelElements.Add(new Station
            {
                Width = 100,
                Height = 200,
                Fill = Brushes.Red,
                X = 20,
                Y = 120
            });

            new DataRandomizer<DataPoint>(DataPoints, 1000);
            //new DataRandomizer2<PanelObject>(DataPoints, 1000);
        }

        public ObservableCollection<PanelObject> PanelElements { get; } = new ObservableCollection<PanelObject>();

        public ObservableNotifiableCollection<DataPoint> DataPoints { get; } = new ObservableNotifiableCollection<DataPoint>();

        //public ObservableNotifiableCollection<PanelObject> DataPoints { get; } = new ObservableNotifiableCollection<PanelObject>();
    }
}
