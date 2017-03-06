namespace TransitCity.City.Transit
{
    using System;
    using System.Windows;

    using Models;

    using Utility;
    using Utility.Coordinates;

    public class MetroStationViewModel : DrawableViewModel
    {
        private double _reachSize;
        private Visibility _reachVisibility;

        private bool _active;

        public MetroStationViewModel(ViewPosition viewPosition, MetroStationModel model)
            : base(viewPosition, 0, 0)
        {
            ReachSize = new ModelPosition(model.GetMaxWalkingDistanceInModelCoordinates(), 0).ToViewPosition().X * 2.0;
            Bottom = viewPosition.Y - ReachSize / 2;
            Left = viewPosition.X - ReachSize / 2;
            Model = model;
        }

        public MetroStationModel Model { get; }

        public double ReachSize
        {
            get { return _reachSize; }
            set
            {
                if (Math.Abs(value - _reachSize) > double.Epsilon)
                {
                    _reachSize = value;
                    OnPropertyChanged();
                }
            }
        }

        public Visibility ReachVisibility
        {
            get { return _reachVisibility; }
            set
            {
                if (value != _reachVisibility)
                {
                    _reachVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public void Clicked()
        {
            _active = !_active;
            ReachVisibility = _active ? Visibility.Visible : Visibility.Collapsed;
        }

        public void OnSizeChanged(SizeChangedEventArgs e)
        {
            Bottom = ViewPosition.Y - e.NewSize.Height / 2.0;
            Left = ViewPosition.X - e.NewSize.Width / 2.0;
        }
    }
}
