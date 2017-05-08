using System.Windows;
using WpfDrawing.Utility;

namespace WpfDrawing.Panel
{
    /// <summary>
    /// Interaction logic for PanelControl.xaml
    /// </summary>
    public partial class PanelControl
    {
        public static readonly DependencyProperty DataPointsProperty = DependencyProperty.Register(
            "DataPoints",
            typeof(ObservableNotifiableCollection<PanelObject>),
            typeof(PanelControl),
            new PropertyMetadata(new ObservableNotifiableCollection<PanelObject>()));

        public PanelControl()
        {
            InitializeComponent();
        }

        public ObservableNotifiableCollection<PanelObject> DataPoints
        {
            get => (ObservableNotifiableCollection<PanelObject>)GetValue(DataPointsProperty);
            set => SetValue(DataPointsProperty, value);
        }
    }
}
