using System;
using System.Drawing;
using System.Linq;
using Statistics.Charts;
using Svg;

namespace SvgDrawing.Charts
{
    public class SvgBarChart
    {
        private SvgDocumentWrapper _document;
        private float _borderThickness = 32f;
        private readonly SvgColourServer _barColor = new SvgColourServer(Color.DarkGreen);

        public SvgBarChart(BarChart<int> chart, float barWidth, float gapWidth, float maxBarHeight, int textSize, float borderThickness = -1f)
        {
            
            Initialize(chart, barWidth, gapWidth, maxBarHeight, textSize, borderThickness);
        }

        public SvgBarChart(BarChart<float> chart, float barWidth, float gapWidth, int maxBarHeight, int textSize, float borderThickness = -1f)
        {
            Initialize(chart, barWidth, gapWidth, maxBarHeight, textSize, borderThickness);
        }

        public void Save(string path)
        {
            _document.Save(path);
        }

        private void Initialize<T>(BarChart<T> chart, float barWidth, float gapWidth, float maxBarHeight, int textSize, float borderThickness) where T : struct
        {
            if (Math.Abs(borderThickness + 1f) > float.Epsilon)
            {
                _borderThickness = borderThickness;
            }

            var width = CalculateWidth(chart, barWidth, gapWidth);
            var height = CalculateHeight(maxBarHeight, textSize);
            _document = new SvgDocumentWrapper((int)width, (int)height);

            var values = chart.GetValues().ToList();
            var maxValue = chart.Maximum;
            var maxValueAsFloat = (float) Convert.ChangeType(maxValue, typeof(float));
            for (var i = 0; i < chart.BarCount; ++i)
            {
                var offsetX = (i + 1) * gapWidth + i * barWidth + _borderThickness;
                var valueAsFloat = (float)Convert.ChangeType(values[i], typeof(float));
                var fVal = valueAsFloat / maxValueAsFloat * maxBarHeight;
                _document.Add(new SvgRectangle
                {
                    X = offsetX,
                    Y = _borderThickness + maxBarHeight - fVal,
                    Width = barWidth,
                    Height = fVal,
                    Fill = _barColor
                });
            }
        }

        private float CalculateWidth<T>(BarChart<T> chart, float barWidth, float gapWidth) where T : struct
        {
            return chart.BarCount * barWidth + (chart.BarCount + 1) * gapWidth + 2 * _borderThickness;
        }

        private float CalculateHeight(float height, int textSize)
        {
            return height + textSize + 2 * _borderThickness;
        }
    }
}
