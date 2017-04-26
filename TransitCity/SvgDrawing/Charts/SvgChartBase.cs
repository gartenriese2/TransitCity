using System.Drawing;
using System.Globalization;
using Svg;

namespace SvgDrawing.Charts
{
    public class SvgChartBase
    {
        protected const float AxisLabelMargin = 4f;

        protected SvgDocumentWrapper Document;

        protected float CalculateTextWidth(string text, float fontSize)
        {
            var tmpDoc = new SvgDocumentWrapper(1,1);
            var elem = new SvgText(text)
            {
                FontSize = fontSize
            };
            tmpDoc.Add(elem);
            return elem.Bounds.Width;
        }

        protected float CalculateTextHeight(string text, float fontSize)
        {
            var tmpDoc = new SvgDocumentWrapper(1, 1);
            var elem = new SvgText(text)
            {
                FontSize = fontSize
            };
            tmpDoc.Add(elem);
            return elem.Bounds.Height;
        }

        protected SvgLine CreateXAxis(float chartOffsetX, float width, float height, float borderThickness, float textSize)
        {
            return new SvgLine
            {
                StartX = chartOffsetX,
                EndX = width - borderThickness,
                StartY = height - borderThickness - textSize,
                EndY = height - borderThickness - textSize,
                Stroke = new SvgColourServer(Color.Black),
                StrokeWidth = 2
            };
        }

        protected SvgLine CreateYAxis(float chartOffsetX, float height, float borderThickness, float textSize)
        {
            return new SvgLine
            {
                StartX = chartOffsetX,
                EndX = chartOffsetX,
                StartY = height - borderThickness - textSize,
                EndY = borderThickness,
                Stroke = new SvgColourServer(Color.Black),
                StrokeWidth = 2
            };
        }

        protected void AddAxisLabels(float axisSteps, float axisStepSize, float textSize, float borderThickness, float chartHeight, float axisLabelWidth)
        {
            for (var i = 0; i <= (int)axisSteps; ++i)
            {
                var value = i * axisStepSize;
                var label = new SvgText(value.ToString(CultureInfo.InvariantCulture))
                {
                    FontSize = textSize,
                    X = new SvgUnitCollection { 0 },
                    Y = new SvgUnitCollection { 0 }
                };
                Document.Add(label);
                var textWidth = CalculateTextWidth(value.ToString(CultureInfo.InvariantCulture), textSize);
                var textHeight = CalculateTextHeight(value.ToString(CultureInfo.InvariantCulture), textSize);
                label.X = new SvgUnitCollection { borderThickness + axisLabelWidth - textWidth - AxisLabelMargin };
                label.Y = new SvgUnitCollection { borderThickness + chartHeight - i * (chartHeight / axisSteps) + textHeight / 2f };
            }
        }
    }
}
