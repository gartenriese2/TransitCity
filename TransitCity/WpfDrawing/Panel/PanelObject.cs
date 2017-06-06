using System;
using System.Windows.Media;
using Utility.MVVM;

namespace WpfDrawing.Panel
{
    public abstract class PanelObject : PropertyChangedBase
    {
        private double _x;
        private double _y;
        private double _angle;
        private double _scale;

        public double X
        {
            get => _x;
            set
            {
                if (Math.Abs(_x - value) > double.Epsilon)
                {
                    _x = value;
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSelected { get; set; }

        public abstract Drawing GetDrawing();

        public TransformGroup GetTransformGroup()
        {
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform(Scale, Scale));
            transformGroup.Children.Add(new TranslateTransform(X, Y));
            transformGroup.Children.Add(new RotateTransform(Angle, X, Y));
            return transformGroup;
        }
    }
}
