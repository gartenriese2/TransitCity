namespace WpfDrawing.Panel
{
    using System;
    using System.Windows.Media;

    using global::Utility.MVVM;

    public abstract class PanelObject : PropertyChangedBase
    {
        private double _x;
        private double _y;
        private double _angle;
        private double _scale;
        private TransformGroup _transformGroup;

        public TransformGroup TransformGroup
        {
            get => _transformGroup;
            private set
            {
                _transformGroup = value;
                OnPropertyChanged();
            }
        }

        public double X
        {
            get => _x;
            set
            {
                if (Math.Abs(_x - value) > double.Epsilon)
                {
                    _x = value;
                    UpdateTransformGroup();
                }
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                if (Math.Abs(_y - value) > double.Epsilon)
                {
                    _y = value;
                    UpdateTransformGroup();
                }
            }
        }

        public double Angle
        {
            get => _angle;
            set
            {
                if (Math.Abs(value - _angle) > double.Epsilon)
                {
                    _angle = value;
                    UpdateTransformGroup();
                }
            }
        }

        public double Scale
        {
            get => _scale;
            set
            {
                if (Math.Abs(value - _scale) > double.Epsilon)
                {
                    _scale = value;
                    UpdateTransformGroup();
                }
            }
        }

        public bool IsSelected { get; set; }

        public abstract void Draw(DrawingContext dc);

        public void Update(double x, double y, double angle, double scale)
        {
            UpdateSilently(x, y, angle, scale);
            UpdateTransformGroup();
        }

        public void UpdateSilently(double x, double y, double angle, double scale)
        {
            _x = x;
            _y = y;
            _angle = angle;
            _scale = scale;
        }

        public void UpdateTransformGroup()
        {
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform(Scale, Scale));
            transformGroup.Children.Add(new TranslateTransform(X, Y));
            transformGroup.Children.Add(new RotateTransform(Angle, X, Y));
            TransformGroup = transformGroup;
        }
    }
}
