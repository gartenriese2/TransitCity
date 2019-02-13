using System;
using System.Windows;
using System.Windows.Media;
using CitySimulation;
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

        public ResidentObject(Position2d position, Vector2d direction, Resident resident)
        {
            Resident = resident;
            Update(position.X, position.Y, Math.Atan2(direction.Y, direction.X) * 180.0 / Math.PI - 90.0, 2);
        }

        public Resident Resident { get; }

        public override void Draw(DrawingContext dc)
        {
            dc.DrawDrawing(Drawing);
        }

        public void Update(Position2d position, Vector2d direction)
        {
            Update(position.X, position.Y, Math.Atan2(direction.Y, direction.X) * 180.0 / Math.PI - 90.0, Scale);
        }
    }
}
