using System.Windows.Input;

namespace TransitCity.City.Transit
{
    using System.Windows;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for MetroStation.xaml
    /// </summary>
    public partial class MetroStation
    {
        public MetroStation()
        {
            InitializeComponent();
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var vm = (MetroStationViewModel)((Ellipse)sender).DataContext;
            vm.Clicked();
        }

        private void FrameworkElement_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var station = (MetroStation)sender;
            if (station.DataContext is MetroStationViewModel)
            {
                ((MetroStationViewModel)station.DataContext).OnSizeChanged(e);
            }
        }
    }
}
