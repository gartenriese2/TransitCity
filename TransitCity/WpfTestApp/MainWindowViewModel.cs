using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Shapes;

namespace WpfTestApp
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            PanelElements.Add(new Rectangle
            {
                Width = 100,
                Height = 200
            });
        }

        public ObservableCollection<FrameworkElement> PanelElements { get; } = new ObservableCollection<FrameworkElement>();
    }
}
