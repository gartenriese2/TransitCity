namespace TransitCity.Utility
{
    using System;
    using System.Windows.Media;

    using Coordinates;

    public class LineViewModel : DrawableViewModel
    {
        private ViewPosition _from;

        private ViewPosition _to;

        private Brush _brush = new SolidColorBrush(Colors.Black);

        private double _width;

        public LineViewModel(ViewPosition position, double bottom, double left)
            : base(position, bottom, left)
        {
        }

        public ViewPosition From
        {
            get { return _from; }
            set
            {
                if (_from != value)
                {
                    _from = value;
                    OnPropertyChanged();
                }
            }
        }

        public ViewPosition To
        {
            get { return _to; }
            set
            {
                if (_to != value)
                {
                    _to = value;
                    OnPropertyChanged();
                }
            }
        }

        public Brush Brush
        {
            get { return _brush; }
            set
            {
                if (!Equals(_brush, value))
                {
                    _brush = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Width
        {
            get { return _width; }
            set
            {
                if (Math.Abs(value - _width) > double.Epsilon)
                {
                    _width = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
