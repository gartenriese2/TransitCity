using System;
using System.Windows;
using System.Windows.Media;

namespace WpfTestApp
{
    public class Station : DataPoint
    {
        private static readonly Drawing Drawing;

        static Station()
        {
            var brush = new SolidColorBrush(Colors.White);
            var pen = new Pen(new SolidColorBrush(Colors.Black), 4);
            var geo = new EllipseGeometry(new Point(0, 0), 8, 8);
            Drawing = new GeometryDrawing(brush, pen, geo);
        }

        public override Drawing GetDrawing()
        {
            return Drawing;
        }
    }
}
