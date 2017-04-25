using System;
using System.Drawing;
using System.Linq;
using Statistics.Charts;
using Svg;

namespace SvgDrawing.Charts
{
    public class SvgLineChart
    {
        private readonly SvgDocumentWrapper _document;
        private readonly float _borderThickness;
        private float _lineThickness;
        private readonly SvgColourServer _lineColor = new SvgColourServer(Color.DarkGreen);

        public SvgLineChart(LineChart chart, float chartWidth, float chartHeight, int textSize, float borderThickness = 32f, float lineThickness = 2f)
        {
            _borderThickness = borderThickness;
            _lineThickness = lineThickness;

            var width = CalculateWidth(chart, chartWidth);
            var height = CalculateHeight(chartHeight, textSize);
            _document = new SvgDocumentWrapper((int)width, (int)height);

            var values = chart.GetValues().ToList();
            var numRanges = chart.Ranges.Count;
            var xDelta = chartWidth / (numRanges - 1);
            var svgPolyline = new SvgPolyline
            {
                StrokeWidth = _lineThickness,
                Stroke = _lineColor,
                Fill = SvgPaintServer.None,
                Points = new SvgPointCollection()
            };
            for (var i = 0; i < numRanges; ++i)
            {
                var value = values[i];
                var x = _borderThickness + i * xDelta;
                var y = _borderThickness + chartHeight * (1f - value / chart.YMax);
                svgPolyline.Points.Add(x);
                svgPolyline.Points.Add(y);
            }
            _document.Add(svgPolyline);

            // axes
            var axisOffset = 4f;
            var xAxis = new SvgLine
            {
                StartX = _borderThickness - axisOffset,
                EndX = width - _borderThickness + axisOffset,
                StartY = height - _borderThickness - textSize,
                EndY = height - _borderThickness - textSize,
                Stroke = new SvgColourServer(Color.Black),
                StrokeWidth = 2
            };
            _document.Add(xAxis);
            var yAxis = new SvgLine
            {
                StartX = _borderThickness,
                EndX = _borderThickness,
                StartY = height - _borderThickness - textSize + axisOffset,
                EndY = _borderThickness - axisOffset,
                Stroke = new SvgColourServer(Color.Black),
                StrokeWidth = 2
            };
            _document.Add(yAxis);
        }

        public void Save(string path)
        {
            _document.Save(path);
        }

        private float CalculateWidth(LineChart chart, float chartWidth)
        {
            return chartWidth + 2 * _borderThickness;
        }

        private float CalculateHeight(float chartHeight, int textSize)
        {
            return chartHeight + textSize + 2 * _borderThickness;
        }
    }
}
