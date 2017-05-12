using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Geometry;
using WpfDrawing.Panel;

namespace WpfDrawing.Objects
{
    public class Route : PanelObject
    {
        private readonly Drawing _drawing;

        public Route(Path path, Color color)
        {
            var brush = new SolidColorBrush(Colors.Transparent);
            var pen = new Pen(new SolidColorBrush(color), 4.0);
            var pathSegments = new List<PathSegment>();
            for (var i = 1; i < path.Count; ++i)
            {
                var pos = path[i];
                pathSegments.Add(new LineSegment(new Point(pos.X, pos.Y), true));
            }

            var pathFigure = new PathFigure(new Point(path[0].X, path[0].Y), pathSegments, false);
            var geo = new PathGeometry(new []{pathFigure});
            _drawing = new GeometryDrawing(brush, pen, geo);

            Scale = 0.08;
        }

        public override Drawing GetDrawing() => _drawing;
    }
}
