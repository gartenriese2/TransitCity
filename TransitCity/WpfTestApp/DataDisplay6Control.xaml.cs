using System.Windows;
using WpfDrawing.Objects;

namespace WpfTestApp
{
    /// <summary>
    /// Interaction logic for DataDisplay6Control.xaml
    /// </summary>
    public partial class DataDisplay6Control
    {
        public static readonly DependencyProperty DataPointsProperty = DependencyProperty.Register(
            "DataPoints",
            typeof(ObservableNotifiableCollection<DataPoint>),
            typeof(DataDisplay6Control),
            new PropertyMetadata(new ObservableNotifiableCollection<DataPoint>()));

        //public static readonly DependencyProperty DataPointsProperty = DependencyProperty.Register(
        //    "DataPoints",
        //    typeof(ObservableNotifiableCollection<PanelObject>),
        //    typeof(DataDisplay6Control),
        //    new PropertyMetadata(new ObservableNotifiableCollection<PanelObject>()));

        public DataDisplay6Control()
        {
            InitializeComponent();
        }

        public ObservableNotifiableCollection<DataPoint> DataPoints
        {
            get => (ObservableNotifiableCollection<DataPoint>)GetValue(DataPointsProperty);
            set => SetValue(DataPointsProperty, value);
        }

        //public ObservableNotifiableCollection<PanelObject> DataPoints
        //{
        //    get => (ObservableNotifiableCollection<PanelObject>)GetValue(DataPointsProperty);
        //    set => SetValue(DataPointsProperty, value);
        //}
    }
}
