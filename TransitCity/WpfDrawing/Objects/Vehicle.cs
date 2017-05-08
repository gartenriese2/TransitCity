using System.Windows;
using System.Windows.Media;
using WpfDrawing.Panel;

namespace WpfDrawing.Objects
{
    public class Vehicle : PanelObject
    {
        private static readonly Drawing Drawing;

        static Vehicle()
        {
            var brush = new SolidColorBrush(Colors.Red);
            var pen = new Pen(new SolidColorBrush(Colors.Black), 1);
            var geo = new PathGeometry(new []
            {
                new PathFigure(new Point(-4, -8), new []
                {
                    new LineSegment(new Point(4, -8), true),
                    new LineSegment(new Point(0, 8), true),
                    new LineSegment(new Point(-4, -8), true)
                },
                true)
            });
            Drawing = new GeometryDrawing(brush, pen, geo);
        }

        public override Drawing GetDrawing()
        {
            return Drawing;
        }
    }
}
