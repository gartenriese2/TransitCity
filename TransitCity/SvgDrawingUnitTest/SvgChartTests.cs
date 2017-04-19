using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Statistics.Charts;
using Statistics.Data;
using SvgDrawing.Charts;

namespace SvgDrawingUnitTest
{
    [TestClass]
    public class SvgChartTests
    {
        [TestMethod]
        public void TestBarChart()
        {
            var data = new NamedData<int>(new List<Enum>{DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday});
            var chart = new BarChart<int>(data);
            var svgChart = new SvgBarChart(chart, 32, 16, 512, 12);
        }
    }
}
