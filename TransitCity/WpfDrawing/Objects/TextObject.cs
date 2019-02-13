namespace WpfDrawing.Objects
{
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;

    using Geometry;

    using WpfDrawing.Panel;

    public class TextObject : PanelObject
    {
        private FormattedText _formattedText;

        private string _text;

        public TextObject(string text, Position2d position)
        {
            _formattedText = new FormattedText(
                text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Tahoma"),
                16,
                Brushes.Black,
                1.0);
            _text = text;
            Update(position.X, position.Y, 0.0, 5);
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

        public override void Draw(DrawingContext dc)
        {
            dc.DrawText(_formattedText, new Point(0, 0));
        }
    }
}
