namespace TransitCity.City
{
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for JobControl.xaml
    /// </summary>
    public partial class JobControl
    {
        public JobControl()
        {
            InitializeComponent();
        }

        private void JobControl_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var vm = (JobViewModel)((JobControl)sender).DataContext;
            vm.Clicked();
        }
    }
}
