namespace TransitCity.City
{
    using System;
    using System.Windows.Media;

    using Models;

    using Utility;
    using Utility.Coordinates;

    public class ResidentViewModel : DrawableViewModel
    {
        private bool _activated;
        private double _size;

        public ResidentViewModel(ResidentialBuildingModel model, ViewPosition viewPosition)
            : base(viewPosition, 0, 0)
        {
            Model = model;
            Size = ResidentsToSize((uint)Model.Residents.Count) + 4;
            Bottom = ViewPosition.Y - Size / 2;
            Left = ViewPosition.X - Size / 2;
        }

        public ResidentialBuildingModel Model { get; }

        public ConnectionViewModel ConnectionViewModel { get; set; }

        public double Size
        {
            get { return _size; }
            set
            {
                if (Math.Abs(value - _size) > double.Epsilon)
                {
                    _size = value;
                    OnPropertyChanged();
                }
            }
        }

        public void Clicked()
        {
            _activated = !_activated;
            ConnectionViewModel.Brush = new SolidColorBrush(_activated ? Colors.Black : Colors.DarkGray);
        }

        private double ResidentsToSize(uint residents)
        {
            return Math.Sqrt(residents / Math.PI);
        }
    }
}
