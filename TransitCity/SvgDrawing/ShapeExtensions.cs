using System;
using Geometry.Shapes;
using Svg;

namespace SvgDrawing
{
    public static class ShapeExtensions
    {
        public static SvgVisualElement ToSvg(this IShape shape)
        {
            if (shape is Circle)
            {
                return ((Circle) shape).ToSvg();
            }

            if (shape is Triangle)
            {
                return ((Triangle) shape).ToSvg();
            }

            if (shape is Polygon)
            {
                return ((Polygon) shape).ToSvg();
            }

            throw new ArgumentException();
        }

        public static SvgCircle ToSvg(this Circle shape)
        {
            return new SvgCircle
            {
                CenterX = shape.Center.X,
                CenterY = shape.Center.Y,
                Radius = shape.Radius
            };
        }

        public static SvgPolygon ToSvg(this Triangle shape)
        {
            return new SvgPolygon
            {
                Points = new SvgPointCollection
                {
                    shape.A.X,
                    shape.A.Y,
                    shape.B.X,
                    shape.B.Y,
                    shape.C.X,
                    shape.C.Y
                }
            };
        }

        public static SvgPolygon ToSvg(this Polygon shape)
        {
            var pointCollection = new SvgPointCollection();
            foreach (var vertex in shape.Vertices)
            {
                pointCollection.Add(vertex.X);
                pointCollection.Add(vertex.Y);
            }
            return new SvgPolygon
            {
                Points = pointCollection
            };
        }
    }
}
