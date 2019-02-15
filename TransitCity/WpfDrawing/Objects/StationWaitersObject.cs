using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Transit;
using WpfDrawing.Panel;

namespace WpfDrawing.Objects
{
    public class StationWaitersObject : PanelObject
    {
        private readonly bool _isFirstRoute;
        private FormattedText _formattedText;
        private string _text;

        public StationWaitersObject(string text, Station station, bool isFirstRoute)
        {
            _isFirstRoute = isFirstRoute;
            _formattedText = new FormattedText(
                text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Tahoma"),
                16,
                Brushes.Black,
                1.0);
            _text = text;
            Station = station;
            Update(station.Position.X, station.Position.Y, 0.0, 8);
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                _formattedText = new FormattedText(
                    _text,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Tahoma"),
                    16,
                    Brushes.Black,
                    1.0);
                OnPropertyChanged();
            }
        }

        public Station Station { get; }

        public override void Draw(DrawingContext dc)
        {
            dc.DrawText(_formattedText, new Point(10, _isFirstRoute ? 0 : -20));
        }
    }
}
