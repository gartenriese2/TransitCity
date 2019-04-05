namespace WpfDrawing.Panel
{
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;

    using WpfDrawing.Objects;
    using WpfDrawing.Utility;

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

        public static readonly DependencyProperty ViewOffsetProperty = DependencyProperty.Register(
            "ViewOffset",
            typeof(Point),
            typeof(PanelControl),
            new PropertyMetadata(new Point(0, 0)));

        public static readonly DependencyProperty WorldSizeProperty = DependencyProperty.Register(
            "WorldSize",
            typeof(Rect),
            typeof(PanelControl),
            new PropertyMetadata(new Rect(new Size(0, 0))),
            value => ((Rect)value).Width >= 0 && ((Rect)value).Height >= 0);

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

        public Point ViewOffset
        {
            get => (Point)GetValue(ViewOffsetProperty);
            set => SetValue(ViewOffsetProperty, value);
        }

        public Rect WorldSize
        {
            get => (Rect)GetValue(WorldSizeProperty);
            set => SetValue(WorldSizeProperty, value);
        }

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var pt = e.GetPosition((UIElement)sender);
            VisualTreeHelper.HitTest(this, null, Callback, new PointHitTestParameters(pt));

            HitTestResultBehavior Callback(HitTestResult result)
            {
                var obj = (result.VisualHit as PanelDrawingVisual)?.PanelObject;
                if (obj is StationObject)
                {
                    if (obj.IsSelected)
                    {
                        obj.Scale /= 2;
                        obj.IsSelected = false;
                    }
                    else
                    {
                        obj.Scale *= 2;
                        obj.IsSelected = true;
                    }
                }

                // Stop the hit test enumeration of objects in the visual tree.
                return HitTestResultBehavior.Stop;
            }
        }
    }
}
