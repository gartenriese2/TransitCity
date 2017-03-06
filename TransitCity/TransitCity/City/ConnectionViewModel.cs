namespace TransitCity.City
{
    using System;
    using System.Windows.Media;

    using Utility.Coordinates;

    using Utility;

    public class ConnectionViewModel : DrawableViewModel
    {
        private ViewPosition _from;

        private ViewPosition _to;

        private Brush _brush = new SolidColorBrush(Colors.DarkGray);

        public ConnectionViewModel(ResidentViewModel residentViewModel, JobViewModel jobViewModel)
            : base(null, 0, 0)
        {
            From = residentViewModel.ViewPosition;
            To = jobViewModel.ViewPosition;
            Bottom = Math.Min(From.Y, To.Y);
            Left = Math.Min(From.X, To.X);
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
    }
}