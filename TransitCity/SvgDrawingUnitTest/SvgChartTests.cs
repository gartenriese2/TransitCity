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

        [TestMethod]
        public void TestLineChart()
        {
            var data = new RangedData(0f, 5f, 20);
            data.AddDatapoint(new FloatDatapoint(2, 7));
            data.AddDatapoint(new FloatDatapoint(21, 79));
            data.AddDatapoint(new FloatDatapoint(42, 37));
            var chart = new LineChart(data);
            var svgChart = new SvgLineChart(chart, 512, 512, 12);
            svgChart.Save("lineChart.svg");
        }

        [TestMethod]
        public void TestLineChartWithJobSchedules()
        {
            const int count = 1000000;
            var jobSchedules = Enumerable.Repeat(new Random(), count).Select(JobSchedule.CreateRandom).ToList();
            var list = SortWorkers(jobSchedules, TimeSpan.FromMinutes(5));
            var data = new RangedData(0f, 1f, list.Count - 1);
            for (var i = 0; i < list.Count; ++i)
            {
                data.AddDatapoint(new FloatDatapoint(i, list[i]));
            }

            var chart = new LineChart(data);
            var svgChart = new SvgLineChart(chart, 512, 512, 12, 32, 1);
            svgChart.Save("workersPerHour.svg");
        }

        private static int CountWorkers(IEnumerable<JobSchedule> schedules, DayOfWeek day)
        {
            var dayBegin = new WeekTimePoint(day);
            var dayEnd = dayBegin + TimeSpan.FromHours(24);
            var dayWts = new WeekTimeSpan(dayBegin, dayEnd);
            return schedules.Count(js => js.WeekTimeSpans.Any(wts => dayWts.IsInside(wts.Begin)));
        }

        private static int CountWorkers(IEnumerable<JobSchedule> schedules, WeekTimeSpan targetWts)
        {
            return schedules.Count(js => js.WeekTimeSpans.Any(wts => targetWts.IsInside(wts.Begin) || wts.IsInside(targetWts.Begin)));
        }

        private static List<int> SortWorkers(IEnumerable<JobSchedule> schedules, TimeSpan delta)
        {
            var num = (int)Math.Ceiling(7.0 / delta.TotalDays);
            var list = Enumerable.Range(0, num).Select(i => 0).ToList();
            foreach (var wts in schedules.SelectMany(s => s.WeekTimeSpans))
            {
                var beginIdx = (int) (wts.Begin.TimePoint.TotalDays / delta.TotalDays);
                var endIdx = (int) (wts.End.TimePoint.TotalDays / delta.TotalDays);
                if (endIdx >= beginIdx)
                {
                    for (var i = beginIdx; i <= endIdx; ++i)
                    {
                        ++list[i];
                    }
                }
                else
                {
                    for (var i = 0; i <= endIdx; ++i)
                    {
                        ++list[i];
                    }

                    for (var i = beginIdx; i < list.Count; ++i)
                    {
                        ++list[i];
                    }
                }
            }

            return list;
        }
    }
}
