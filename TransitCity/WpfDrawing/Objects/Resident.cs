using System;
using System.Windows;
using System.Windows.Media;
using Geometry;
using WpfDrawing.Panel;

namespace WpfDrawing.Objects
{
    public class ResidentObject : PanelObject
    {
        private static readonly Drawing Drawing;

        static ResidentObject()
        {
            var brush = new SolidColorBrush(Colors.DarkGreen);
            var pen = new Pen(new SolidColorBrush(Colors.Black), 1);
            var geo = new PathGeometry(new[]
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

        public ResidentObject(Position2d position, Vector2d direction)
        {
            X = position.X;
            Y = position.Y;
            Angle = Math.Atan2(direction.Y, direction.X) * 180.0 / Math.PI - 90.0;
            Scale = 2;
        }

        public override Drawing GetDrawing() => Drawing;
    }
}
