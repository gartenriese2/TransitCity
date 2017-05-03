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
        }

        public ObservableCollection<PanelObject> PanelElements { get; } = new ObservableCollection<PanelObject>();
    }
}
