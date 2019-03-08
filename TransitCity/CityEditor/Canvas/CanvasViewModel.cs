namespace CityEditor.Canvas
{
    using System;
    using System.Windows;

    using Utility.MVVM;

    using WpfDrawing.Panel;
    using WpfDrawing.Utility;

    public class CanvasViewModel : PropertyChangedBase
    {
        private Point _center;
        private double _centerX;
        private double _centerY;
        private double _zoom;

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
    }
}
