namespace TransitCity.City.Transit
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    using MVVM;

    using TransitCity.Models.Transit;

    using Utility;
    using Utility.Coordinates;

    public class StationViewModel : DrawableViewModel
    {
        private ICommand _clickCommand;

        public StationViewModel(ViewPosition viewPosition, double size, Station model)
            : base(viewPosition, viewPosition.Y - size / 2, viewPosition.X - size / 2)
        {
            Size = size;
            Model = model;
        }

        public double Size { get; }

        public Station Model { get; }

        public ICommand ClickCommand => _clickCommand ?? (_clickCommand = new RelayCommand(p => OnClick()));

        public void OnSizeChanged(SizeChangedEventArgs e)
        {
            Bottom = ViewPosition.Y - e.NewSize.Height / 2.0;
            Left = ViewPosition.X - e.NewSize.Width / 2.0;
        }

        private void OnClick()
        {
            Console.WriteLine(@"Click!");
        }
    }
}