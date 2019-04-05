namespace WpfDrawing.Utility
{
    using System;
    using System.Windows;
    using System.Windows.Media;

    using Geometry.Shapes;

    public static class ShapeToDrawingGeometryExtensions
    {
        public static Geometry ToDrawingGeometry(this IShape shape)
        {
            if (shape is Polygon polygon)
            {
                var startVertex = polygon.Vertices[0];
                var figure = new PathFigure { StartPoint = new Point(startVertex.X, startVertex.Y), IsClosed = true };
                for (var i = 1; i < polygon.Vertices.Count; ++i)
                {
                    var vertex = polygon.Vertices[i];
                    figure.Segments.Add(new LineSegment(new Point(vertex.X, vertex.Y), true));
                }

                return new PathGeometry(new[] { figure });
            }

            throw new NotImplementedException($"Conversion from {shape.GetType()} to Geometry not implemented yet.");
        }
    }
}
