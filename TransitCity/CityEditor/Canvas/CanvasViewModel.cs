namespace CityEditor.Canvas
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    using Utility.MVVM;

    using WpfDrawing.Panel;
    using WpfDrawing.Utility;

    public class CanvasViewModel : PropertyChangedBase
    {
        private Point _center;
        private double _centerX;
        private double _centerY;
        private double _zoom = 1.0;

        private Point _viewOffset = new Point(0, 0);

        /// <summary>
        /// The view size. This is the actual size of the panel control.
        /// </summary>
        private Size _viewSize = new Size(0, 0);

        private bool _mouseLeftButtonIsDown;

        private Point _previousMousePosition;

        public event EventHandler<Point> MouseMoved;

        public ObservableNotifiableCollection<PanelObject> PanelObjects { get; } = new ObservableNotifiableCollection<PanelObject>();

        public Point Center
        {
            get => _center;
            private set
            {
                if (_center != value)
                {
                    _center = value;
                    OnPropertyChanged();
                }
            }
        }

        public double CenterX
        {
            get => _centerX;
            set
            {
                if (Math.Abs(value - _centerX) > double.Epsilon)
                {
                    _centerX = value;
                    OnPropertyChanged();
                    Center = new Point(CenterX, CenterY);
                }
            }
        }

        public double CenterY
        {
            get => _centerY;
            set
            {
                if (Math.Abs(value - _centerY) > double.Epsilon)
                {
                    _centerY = value;
                    OnPropertyChanged();
                    Center = new Point(CenterX, CenterY);
                }
            }
        }

        public double Zoom
        {
            get => _zoom;
            set
            {
                if (Math.Abs(value - _zoom) > double.Epsilon)
                {
                    _zoom = value;
                    OnPropertyChanged();
                }
            }
        }

        public double MinZoom { get; } = 0.1;

        public double MaxZoom { get; } = 50.0;

        public Size WorldSize { get; } = new Size(10000, 10000);

        public Point ViewOffset
        {
            get => _viewOffset;
            private set
            {
                _viewOffset = value;
                OnPropertyChanged();
            }
        }

        public void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _viewSize = e.NewSize;
        }

        public void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _mouseLeftButtonIsDown = true;
            _previousMousePosition = e.GetPosition((IInputElement)sender);
            Mouse.OverrideCursor = Cursors.ScrollAll;
        }

        public void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _mouseLeftButtonIsDown = false;
            Mouse.OverrideCursor = null;
        }

        public void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _mouseLeftButtonIsDown = false;
            Mouse.OverrideCursor = null;
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            var currentMousePositionView = e.GetPosition((IInputElement)sender);

            if (_mouseLeftButtonIsDown)
            {
                var offset = currentMousePositionView - _previousMousePosition;
                ViewOffset += offset;
                _previousMousePosition = currentMousePositionView;
            }

            var currentMousePositionWorld = CoordinateSystem.TransformPointFromViewToWorld(
                currentMousePositionView,
                _viewSize,
                new Rect(WorldSize),
                ViewOffset,
                Zoom);
            MouseMoved?.Invoke(this, currentMousePositionWorld);
        }

        public void SetInitialViewSize(Size actualSize)
        {
            _viewSize = actualSize;
        }
    }
}
