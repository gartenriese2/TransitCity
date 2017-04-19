using Statistics.Charts;

namespace SvgDrawing.Charts
{
    public class SvgBarChart
    {
        public SvgBarChart(BarChart<int> chart, float barWidth, float gapWidth, float height, int textSize)
        {
            var width = CalculateWidth(chart, barWidth, gapWidth);
        }

        public SvgBarChart(BarChart<float> chart, float barWidth, float gapWidth, float height, int textSize)
        {
            var width = CalculateWidth(chart, barWidth, gapWidth);
        }

        private static float CalculateWidth<T>(BarChart<T> chart, float barWidth, float gapWidth) where T : struct
        {
            return chart.BarCount * barWidth + (chart.BarCount + 1) * gapWidth;
        }
    }
}
