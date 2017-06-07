using System;
using System.Windows;
using System.Windows.Media;
using Geometry;
using Transit;
using WpfDrawing.Panel;

namespace WpfDrawing.Objects
{
    public class VehicleObject : PanelObject
    {
        private static readonly Drawing Drawing;

        static VehicleObject()
        {
            var brush = new SolidColorBrush(Colors.Gray);
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

        public VehicleObject(Position2d position, Vector2d direction, Trip trip)
        {
            Trip = trip;
            Update(position.X, position.Y, Math.Atan2(direction.Y, direction.X) * 180.0 / Math.PI - 90.0, 5);
        }

        public Trip Trip { get; }

        public override Drawing GetDrawing() => Drawing;

        public void Update(Position2d position, Vector2d direction)
        {
            Update(position.X, position.Y, Math.Atan2(direction.Y, direction.X) * 180.0 / Math.PI - 90.0, Scale);
        }
    }
}
