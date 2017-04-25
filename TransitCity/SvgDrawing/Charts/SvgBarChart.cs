﻿using System;
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

                var textElement = new SvgText(chart.Names.ElementAt(i))
                {
                    FontSize = textSize,
                    Y = new SvgUnitCollection { _borderThickness + maxBarHeight + textSize }
                };
                _document.Add(textElement);
                var textWidth = textElement.Bounds.Width;
                textElement.X = new SvgUnitCollection { offsetX + barWidth / 2 - textWidth / 2 };
            }

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
