using System;
using System.Collections.Generic;
using System.Linq;
using CitySimulation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Statistics.Charts;
using Statistics.Data;
using SvgDrawing.Charts;
using Time;

namespace SvgDrawingUnitTest
{
    [TestClass]
    public class SvgChartTests
    {
        [TestMethod]
        public void TestBarChart()
        {
            var data = new NamedData<int>(new List<Enum>{DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday});
            data.AddDatapoint(new NamedDatapoint<int>(DayOfWeek.Monday, 3));
            data.AddDatapoint(new NamedDatapoint<int>(DayOfWeek.Tuesday, 27));
            data.AddDatapoint(new NamedDatapoint<int>(DayOfWeek.Friday, 64));
            var chart = new BarChart<int>(data);
            var svgChart = new SvgBarChart(chart, 64, 16, 512, 12);
            svgChart.Save("barChart.svg");
        }

        [TestMethod]
        public void TestBarChartWithJobSchedules()
        {
            const int count = 1000000;
            var jobSchedules = Enumerable.Repeat(new Random(), count).Select(JobSchedule.CreateRandom).ToList();
            var workersOnMonday = CountWorkers(jobSchedules, DayOfWeek.Monday);
            var workersOnTuesday = CountWorkers(jobSchedules, DayOfWeek.Tuesday);
            var workersOnWednesday = CountWorkers(jobSchedules, DayOfWeek.Wednesday);
            var workersOnThursday = CountWorkers(jobSchedules, DayOfWeek.Thursday);
            var workersOnFriday = CountWorkers(jobSchedules, DayOfWeek.Friday);
            var workersOnSaturday = CountWorkers(jobSchedules, DayOfWeek.Saturday);
            var workersOnSunday = CountWorkers(jobSchedules, DayOfWeek.Sunday);
            var data = new NamedData<int>(new List<Enum> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday });
            data.AddDatapoint(new NamedDatapoint<int>(DayOfWeek.Monday, workersOnMonday));
            data.AddDatapoint(new NamedDatapoint<int>(DayOfWeek.Tuesday, workersOnTuesday));
            data.AddDatapoint(new NamedDatapoint<int>(DayOfWeek.Wednesday, workersOnWednesday));
            data.AddDatapoint(new NamedDatapoint<int>(DayOfWeek.Thursday, workersOnThursday));
            data.AddDatapoint(new NamedDatapoint<int>(DayOfWeek.Friday, workersOnFriday));
            data.AddDatapoint(new NamedDatapoint<int>(DayOfWeek.Saturday, workersOnSaturday));
            data.AddDatapoint(new NamedDatapoint<int>(DayOfWeek.Sunday, workersOnSunday));
            var chart = new BarChart<int>(data);
            var svgChart = new SvgBarChart(chart, 32, 16, 256, 12, 16);
            svgChart.Save("workersPerDay.svg");
        }

        private static int CountWorkers(IEnumerable<JobSchedule> schedules, DayOfWeek day)
        {
            var dayBegin = new WeekTimePoint(day);
            var dayEnd = dayBegin + TimeSpan.FromHours(24);
            var dayWts = new WeekTimeSpan(dayBegin, dayEnd);
            return schedules.Count(js => js.WeekTimeSpans.Any(wts => dayWts.IsInside(wts.Begin)));
        }
    }
}
