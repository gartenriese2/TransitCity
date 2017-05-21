using System.Windows;
using WpfDrawing.Utility;

namespace WpfDrawing.Panel
{
    /// <summary>
    /// Interaction logic for PanelControl.xaml
    /// </summary>
    public partial class PanelControl
    {
        public static readonly DependencyProperty ObjectsProperty = DependencyProperty.Register(
            "Objects",
            typeof(ObservableNotifiableCollection<PanelObject>),
            typeof(PanelControl),
            new PropertyMetadata(new ObservableNotifiableCollection<PanelObject>()));

        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(
            "Zoom",
            typeof(double),
            typeof(PanelControl),
            new PropertyMetadata(1.0),
            value => (double)value > 0.0);

        public static readonly DependencyProperty MinZoomProperty = DependencyProperty.Register(
            "MinZoom",
            typeof(double),
            typeof(PanelControl),
            new PropertyMetadata(0.001),
            value => (double)value > 0.0);

        public static readonly DependencyProperty MaxZoomProperty = DependencyProperty.Register(
            "MaxZoom",
            typeof(double),
            typeof(PanelControl),
            new PropertyMetadata(1000.0),
            value => (double)value > 0.0);

        public static readonly DependencyProperty WorldSizeProperty = DependencyProperty.Register(
            "WorldSize",
            typeof(Size),
            typeof(PanelControl),
            new PropertyMetadata(new Size(0, 0)),
            value => ((Size)value).Width >= 0 && ((Size)value).Height >= 0);

        public PanelControl()
        {
            InitializeComponent();
        }

        public ObservableNotifiableCollection<PanelObject> Objects
        {
            get => (ObservableNotifiableCollection<PanelObject>)GetValue(ObjectsProperty);
            set => SetValue(ObjectsProperty, value);
        }

        public double Zoom
        {
            get => (double) GetValue(ZoomProperty);
            set => SetValue(ZoomProperty, value);
        }

        public double MinZoom
        {
            get => (double)GetValue(MinZoomProperty);
            set => SetValue(MinZoomProperty, value);
        }

        public double MaxZoom
        {
            get => (double)GetValue(MaxZoomProperty);
            set => SetValue(MaxZoomProperty, value);
        }

        public Size WorldSize
        {
            get => (Size)GetValue(WorldSizeProperty);
            set => SetValue(WorldSizeProperty, value);
        }
    }
}
