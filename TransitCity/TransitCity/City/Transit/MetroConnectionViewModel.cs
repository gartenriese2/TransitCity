namespace TransitCity.City.Transit
{
    using System;
    using System.Windows.Media;

    using Models;

    using Utility;

    public class MetroConnectionViewModel : LineViewModel
    {
        private const double BaseWidth = 4.0;
        private const double MaxWidth = 10.0;

        public MetroConnectionViewModel(MetroConnectionModel model)
            : base(null, 0, 0)
        {
            Model = model;
            Width = BaseWidth;
            From = model.StationA.Position.ToViewPosition();
            To = model.StationB.Position.ToViewPosition();
            Bottom = Math.Min(From.Y, To.Y);
            Left = Math.Min(From.X, To.X);
            Brush = new SolidColorBrush(Colors.Blue);
        }

        public MetroConnectionModel Model { get; }

        public void ResetWidth()
        {
            Width = BaseWidth;
        }

        public void SetWidthRelative(double value)
        {
            if (value < 0.0 || value > 1.0)
            {
                return;
            }

            var diff = MaxWidth - BaseWidth;
            Width = BaseWidth + diff * value;
        }
    }
}
