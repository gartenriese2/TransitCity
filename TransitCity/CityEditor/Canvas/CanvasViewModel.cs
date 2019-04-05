namespace CityEditor.Canvas
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    using CitySimulation;

    using Geometry;
    using Geometry.Shapes;

    using Utility.MVVM;

    using WpfDrawing.Objects;
    using WpfDrawing.Panel;
    using WpfDrawing.Utility;

    public class CanvasViewModel : PropertyChangedBase
    {
        private double _zoom = 1.0;

        private Point _viewOffset = new Point(0, 0);

        private List<Position2d> _newDistrictPoints;

        private List<PanelObject> _newDistrictPanelObjects;

        /// <summary>
        /// The view size. This is the actual size of the panel control.
        /// </summary>
        private Size _viewSize = new Size(0, 0);

        private bool _mouseLeftButtonIsDown;

        private Point _previousMousePosition;

        private bool _isInDrawingMode;

        public CanvasViewModel()
        {
            PanelObjects.Add(new StationObject(new Position2d(0, 0)) { Scale = 10 });
            PanelObjects.Add(new StationObject(new Position2d(5000, 0)) { Scale = 10 });
            PanelObjects.Add(new StationObject(new Position2d(10000, 0)) { Scale = 10 });
            PanelObjects.Add(new StationObject(new Position2d(0, 5000)) { Scale = 10 });
            PanelObjects.Add(new StationObject(new Position2d(5000, 5000)) { Scale = 10 });
            PanelObjects.Add(new StationObject(new Position2d(10000, 5000)) { Scale = 10 });
            PanelObjects.Add(new StationObject(new Position2d(0, 10000)) { Scale = 10 });
            PanelObjects.Add(new StationObject(new Position2d(5000, 10000)) { Scale = 10 });
            PanelObjects.Add(new StationObject(new Position2d(10000, 10000)) { Scale = 10 });
        }

        public event EventHandler<Point> MouseMoved;

        public ObservableNotifiableCollection<PanelObject> PanelObjects { get; } = new ObservableNotifiableCollection<PanelObject>();

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

        public Rect WorldSize { get; } = new Rect(new Size(10000, 10000));

        public Point ViewOffset
        {
            get => _viewOffset;
            private set
            {
                _viewOffset = value;
                OnPropertyChanged();
            }
        }

        public bool IsInDrawingMode
        {
            get => _isInDrawingMode;
            private set
            {
                _isInDrawingMode = value;
                OnPropertyChanged();
            }
        }

        public void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _viewSize = e.NewSize;
        }

        public void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsInDrawingMode)
            {
                return;
            }

            _mouseLeftButtonIsDown = true;
            _previousMousePosition = e.GetPosition((IInputElement)sender);
            Mouse.OverrideCursor = Cursors.ScrollAll;
        }

        public void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_mouseLeftButtonIsDown)
            {
                return;
            }

            if (IsInDrawingMode)
            {
                IsInDrawingMode = false;

                var district = new RandomDistrict("New District", new Polygon(_newDistrictPoints), 1000, 1000);
                PanelObjects.Add(new DistrictObject(district));
                foreach (var obj in _newDistrictPanelObjects)
                {
                    PanelObjects.Remove(obj);
                }
                
                _newDistrictPoints = null;
                _newDistrictPanelObjects = null;
            }
        }

        public void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsInDrawingMode)
            {
                if (_newDistrictPoints == null)
                {
                    _newDistrictPoints = new List<Position2d>();
                }

                if (_newDistrictPanelObjects == null)
                {
                    _newDistrictPanelObjects = new List<PanelObject>();
                }

                var currentMousePositionView = e.GetPosition((IInputElement)sender);

                // Snap
                var closeObjects = PanelObjects.OfType<DistrictObject>().Where(d => d.District.Shape is Polygon)
                    .SelectMany(d => ((Polygon)d.District.Shape).Vertices).Select(
                        p => CoordinateSystem.TransformPointFromWorldToView(
                            new Point(p.X, p.Y),
                            _viewSize,
                            WorldSize,
                            ViewOffset,
                            Zoom)).Where(p => (p - currentMousePositionView).Length < 10);
                if (closeObjects.Any())
                {
                    currentMousePositionView = closeObjects.Aggregate(
                        (p1, p2) => (p1 - currentMousePositionView).Length.CompareTo(
                                        (p2 - currentMousePositionView).Length) <= 0
                                        ? p1
                                        : p2);
                }

                var currentMousePositionWorld = CoordinateSystem.TransformPointFromViewToWorld(
                    currentMousePositionView,
                    _viewSize,
                    WorldSize,
                    ViewOffset,
                    Zoom);
                var pos = new Position2d(currentMousePositionWorld.X, currentMousePositionWorld.Y);
                _newDistrictPoints.Add(pos);

                var obj = new StationObject(pos) { Scale = 10 };
                _newDistrictPanelObjects.Add(obj);
                PanelObjects.Add(obj);
            }

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
                WorldSize,
                ViewOffset,
                Zoom);
            MouseMoved?.Invoke(this, currentMousePositionWorld);
        }

        public void SetInitialViewSize(Size actualSize)
        {
            _viewSize = actualSize;
        }

        public void StartDrawingPolygon()
        {
            IsInDrawingMode = true;
        }
    }
}
