﻿using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WpfDrawing.Objects;
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

        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(
            "Center",
            typeof(Point),
            typeof(PanelControl),
            new PropertyMetadata(new Point(0.5, 0.5)),
            value => ((Point)value).X >= 0.0 && ((Point)value).X <= 1.0 && ((Point)value).Y >= 0.0 && ((Point)value).Y <= 1.0);

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

        public Point Center
        {
            get => (Point)GetValue(CenterProperty);
            set => SetValue(CenterProperty, value);
        }

        public Size WorldSize
        {
            get => (Size)GetValue(WorldSizeProperty);
            set => SetValue(WorldSizeProperty, value);
        }

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var pt = e.GetPosition((UIElement)sender);
            VisualTreeHelper.HitTest(this, null, Callback, new PointHitTestParameters(pt));

            HitTestResultBehavior Callback(HitTestResult result)
            {
                var obj = (result.VisualHit as PanelDrawingVisual)?.PanelObject;
                if (obj is Station)
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
