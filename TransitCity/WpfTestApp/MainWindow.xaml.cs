using System.Windows;
using System.Windows.Shapes;

namespace WpfTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            ((sender as MainWindow)?.DataContext as MainWindowViewModel)?.PanelElements.Add(new Rectangle
            {
                Width = 100,
                Height = 200
            });
        }
    }
}
