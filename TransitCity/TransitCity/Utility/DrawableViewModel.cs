namespace TransitCity.Utility
{
    using System;
    using System.Windows;

    using Coordinates;

    using MVVM;

    public class DrawableViewModel : PropertyChangedBase
    {
        private Visibility _isVisible;

        private double _bottom;

        private double _left;

        public DrawableViewModel(ViewPosition position, double bottom, double left)
        {
            ViewPosition = position;
            Bottom = bottom;
            Left = left;
        }

        public ViewPosition ViewPosition { get; }

        public Visibility IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Bottom
        {
            get { return _bottom; }
            set
            {
                if (Math.Abs(_bottom - value) > double.Epsilon)
                {
                    _bottom = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Left
        {
            get { return _left; }
            set
            {
                if (Math.Abs(_left - value) > double.Epsilon)
                {
                    _left = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
