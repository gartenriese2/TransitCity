namespace TransitCity.UI
{
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for CityCanvas.xaml
    /// </summary>
    public partial class CityCanvas
    {
        public CityCanvas()
        {
            InitializeComponent();
        }

        private void CityCanvas_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var vm = (CityCanvasViewModel)DataContext;
            vm.OnPreviewMouseLeftButtonDown(sender, e);
        }

        private void CityCanvas_OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as CityCanvasViewModel;
            if (vm != null)
            {
                vm.ActualHeight = ((CityCanvas)sender).ActualHeight;
            }
        }
    }
}
