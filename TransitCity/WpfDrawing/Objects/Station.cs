﻿using System.Windows;
using System.Windows.Media;
using WpfDrawing.Panel;

namespace WpfDrawing.Objects
{
    public class Station : PanelObject
    {
        private static readonly Drawing Drawing;

        static Station()
        {
            var brush = new SolidColorBrush(Colors.White);
            var pen = new Pen(new SolidColorBrush(Colors.Black), 2);
            var geo = new EllipseGeometry(new Point(0, 0), 4, 4);
            Drawing = new GeometryDrawing(brush, pen, geo);
        }

        public override Drawing GetDrawing()
        {
            return Drawing;
        }
    }
}
