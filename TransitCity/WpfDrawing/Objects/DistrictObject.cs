namespace WpfDrawing.Objects
{
    using System.Windows;
    using System.Windows.Media;

    using Geometry.Shapes;

    using WpfDrawing.Panel;

    public class DistrictObject : PanelObject
    {
        private readonly Drawing _drawing;

        public DistrictObject(Polygon polygon)
        {
            var startVertex = polygon.Vertices[0];
            var figure = new PathFigure { StartPoint = new Point(startVertex.X, startVertex.Y), IsClosed = true };
            for (var i = 1; i < polygon.Vertices.Count; ++i)
            {
                var vertex = polygon.Vertices[i];
                figure.Segments.Add(new LineSegment(new Point(vertex.X, vertex.Y), true));
            }

            var geo = new PathGeometry(new[]
            {
                figure
            });
            var brush = new SolidColorBrush(Colors.Transparent);
            var pen = new Pen(new SolidColorBrush(Colors.Black), 2);
            _drawing = new GeometryDrawing(brush, pen, geo);

            Update(0, 0, 0, 1);
        }

        public override void Draw(DrawingContext dc)
        {
            dc.DrawDrawing(_drawing);
        }
    }
}
