using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Statistics.Charts;
using Svg;

namespace SvgDrawing.Charts
{
    public class SvgLineChart : SvgChartBase
    {
        private readonly float _borderThickness;
        private float _lineThickness;
        private readonly SvgColourServer _lineColor = new SvgColourServer(Color.DarkGreen);

        public SvgLineChart(LineChart chart, float chartWidth, float chartHeight, float axisStepSize, int textSize = 12, float borderThickness = 32f, float lineThickness = 2f)
        {
            _borderThickness = borderThickness;
            _lineThickness = lineThickness;

            var axisSteps = (float)Math.Ceiling(chart.YMax / axisStepSize);
            var axisMaxY = axisSteps * axisStepSize;
            var axisLabelWidth = CalculateTextWidth(axisMaxY.ToString(CultureInfo.InvariantCulture), textSize);
            var chartOffsetX = _borderThickness + axisLabelWidth + AxisLabelMargin;

            var width = CalculateWidth(chartWidth, axisLabelWidth);
            var height = CalculateHeight(chartHeight, textSize);
            Document = new SvgDocumentWrapper((int)width, (int)height);

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
                var x = chartOffsetX + i * xDelta;
                var y = _borderThickness + chartHeight * (1f - value / axisMaxY);
                svgPolyline.Points.Add(x);
                svgPolyline.Points.Add(y);
            }
            Document.Add(svgPolyline);

            // axes
            Document.Add(CreateXAxis(chartOffsetX, width, height, _borderThickness, textSize));
            Document.Add(CreateYAxis(chartOffsetX, height, _borderThickness, textSize));

            AddAxisLabels(axisSteps, axisStepSize, textSize, _borderThickness, chartHeight, axisLabelWidth);
        }

        public void Save(string path)
        {
            Document.Save(path);
        }

        private float CalculateWidth(float chartWidth, float axisLabelWidth)
        {
            return chartWidth + 2 * _borderThickness + axisLabelWidth + AxisLabelMargin;
        }

        private float CalculateHeight(float chartHeight, int textSize)
        {
            return chartHeight + textSize + 2 * _borderThickness;
        }
    }
}
