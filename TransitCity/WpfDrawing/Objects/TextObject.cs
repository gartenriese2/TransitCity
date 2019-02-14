namespace WpfDrawing.Objects
{
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;

    using Geometry;

    using WpfDrawing.Panel;

    public class TextObject : PanelObject
    {
        private readonly SolidColorBrush _brush;

        private FormattedText _formattedText;
        private string _text;


        public TextObject(string text, Position2d position, Color color)
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
            color.A = 128;
            _brush = new SolidColorBrush(color);
            _brush.Freeze();
            Update(position.X, position.Y, 0.0, 8);
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
            var pen = new Pen(new SolidColorBrush(Colors.Black), 1);
            dc.DrawEllipse(_brush, pen, new Point(16, 0), 8, 8);
            dc.DrawText(_formattedText, new Point(10, -10));
        }
    }
}
