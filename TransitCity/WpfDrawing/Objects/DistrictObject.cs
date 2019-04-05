namespace WpfDrawing.Objects
{
    using System;
    using System.Windows;
    using System.Windows.Media;

    using CitySimulation;

    using WpfDrawing.Panel;
    using WpfDrawing.Utility;

    public class DistrictObject : PanelObject
    {
        private readonly Drawing _drawing;

        public DistrictObject(IDistrict district)
        {
            District = district;

            var geo = district.Shape.ToDrawingGeometry();
            var brush = new SolidColorBrush(Colors.Transparent);
            var pen = new Pen(new SolidColorBrush(Colors.Black), 2);
            _drawing = new GeometryDrawing(brush, pen, geo);

            Update(0, 0, 0, 1);
        }

        public IDistrict District { get; }

        public override void Draw(DrawingContext dc)
        {
            dc.DrawDrawing(_drawing);
        }
    }
}
