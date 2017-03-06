namespace TransitCity.City
{
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for ResidentControl.xaml
    /// </summary>
    public partial class ResidentControl
    {
        public ResidentControl()
        {
            InitializeComponent();
        }

        private void ResidentControl_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var vm = (ResidentViewModel)((ResidentControl)sender).DataContext;
            vm.Clicked();
        }
    }
}
