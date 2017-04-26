using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Statistics.Charts;
using Svg;

namespace SvgDrawing.Charts
{
    public class SvgBarChart : SvgChartBase
    {
        private float _borderThickness = 32f;
        private readonly SvgColourServer _barColor = new SvgColourServer(Color.DarkGreen);

        public SvgBarChart(BarChart<int> chart, float axisStepSize, float barWidth, float gapWidth, float chartHeight, int textSize, float borderThickness = -1f)
        {
            Initialize(chart, axisStepSize, barWidth, gapWidth, chartHeight, textSize, borderThickness);
        }

        public SvgBarChart(BarChart<float> chart, float axisStepSize, float barWidth, float gapWidth, int chartHeight, int textSize, float borderThickness = -1f)
        {
            Initialize(chart, axisStepSize, barWidth, gapWidth, chartHeight, textSize, borderThickness);
        }

        public void Save(string path)
        {
            Document.Save(path);
        }

        private void Initialize<T>(BarChart<T> chart, float axisStepSize, float barWidth, float gapWidth, float chartHeight, int textSize, float borderThickness) where T : struct
        {
            if (Math.Abs(borderThickness + 1f) > float.Epsilon)
            {
                _borderThickness = borderThickness;
            }

            var maxValueAsFloat = (float)Convert.ChangeType(chart.Maximum, typeof(float));
            var axisSteps = (float)Math.Ceiling(maxValueAsFloat / axisStepSize);
            var axisMaxY = axisSteps * axisStepSize;
            var axisLabelWidth = CalculateTextWidth(axisMaxY.ToString(CultureInfo.InvariantCulture), textSize);
            var chartOffsetX = _borderThickness + axisLabelWidth + AxisLabelMargin;

            var width = CalculateWidth(chart, barWidth, gapWidth, axisLabelWidth);
            var height = CalculateHeight(chartHeight, textSize);
            Document = new SvgDocumentWrapper((int)width, (int)height);

            var values = chart.GetValues().ToList();
            for (var i = 0; i < chart.BarCount; ++i)
            {
                var offsetX = (i + 1) * gapWidth + i * barWidth + chartOffsetX;
                var valueAsFloat = (float)Convert.ChangeType(values[i], typeof(float));
                var fVal = valueAsFloat / axisMaxY * chartHeight;
                Document.Add(new SvgRectangle
                {
                    X = offsetX,
                    Y = _borderThickness + chartHeight - fVal,
                    Width = barWidth,
                    Height = fVal,
                    Fill = _barColor
                });

                var textElement = new SvgText(chart.Names.ElementAt(i))
                {
                    FontSize = textSize,
                    Y = new SvgUnitCollection { _borderThickness + chartHeight + textSize }
                };
                Document.Add(textElement);
                var textWidth = textElement.Bounds.Width;
                textElement.X = new SvgUnitCollection { offsetX + barWidth / 2 - textWidth / 2 };
            }

            // axes
            Document.Add(CreateXAxis(chartOffsetX, width, height, _borderThickness, textSize));
            Document.Add(CreateYAxis(chartOffsetX, height, _borderThickness, textSize));

            AddAxisLabels(axisSteps, axisStepSize, textSize, _borderThickness, chartHeight, axisLabelWidth);
        }

        private float CalculateWidth<T>(BarChart<T> chart, float barWidth, float gapWidth, float axisLabelWidth) where T : struct
        {
            return chart.BarCount * barWidth + (chart.BarCount + 1) * gapWidth + 2 * _borderThickness + axisLabelWidth + AxisLabelMargin;
        }

        private float CalculateHeight(float height, int textSize)
        {
            return height + textSize + 2 * _borderThickness;
        }
    }
}
