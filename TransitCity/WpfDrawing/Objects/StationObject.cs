using System.Windows;
using System.Windows.Media;
using Geometry;
using WpfDrawing.Panel;

namespace WpfDrawing.Objects
{
    public class StationObject : PanelObject
    {
        private static readonly Drawing Drawing;

        static StationObject()
        {
            var brush = new SolidColorBrush(Colors.White);
            var pen = new Pen(new SolidColorBrush(Colors.Black), 1);
            var geo = new EllipseGeometry(new Point(0, 0), 4, 4);
            Drawing = new GeometryDrawing(brush, pen, geo);
        }

        public StationObject(Position2d position)
        {
            Update(position.X, position.Y, 0.0, 5);
        }

        public override Drawing GetDrawing() => Drawing;
    }
}
