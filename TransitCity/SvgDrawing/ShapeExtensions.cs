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
                CenterX = (SvgUnit) shape.Center.X,
                CenterY = (SvgUnit) shape.Center.Y,
                Radius = (SvgUnit) shape.Radius
            };
        }

        public static SvgPolygon ToSvg(this Triangle shape)
        {
            return new SvgPolygon
            {
                Points = new SvgPointCollection
                {
                    (SvgUnit) shape.A.X,
                    (SvgUnit) shape.A.Y,
                    (SvgUnit) shape.B.X,
                    (SvgUnit) shape.B.Y,
                    (SvgUnit) shape.C.X,
                    (SvgUnit) shape.C.Y
                }
            };
        }

        public static SvgPolygon ToSvg(this Polygon shape)
        {
            var pointCollection = new SvgPointCollection();
            foreach (var vertex in shape.Vertices)
            {
                pointCollection.Add((SvgUnit) vertex.X);
                pointCollection.Add((SvgUnit) vertex.Y);
            }
            return new SvgPolygon
            {
                Points = pointCollection
            };
        }
    }
}
