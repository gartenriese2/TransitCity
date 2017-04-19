using System;
using System.Collections.Generic;
using System.Linq;
using CitySimulation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Time;

namespace CitySimulationUnitTest
{
    [TestClass]
    public class JobScheduleUnitTests
    {
        [TestMethod]
        public void TestRandom()
        {
            const int count = 1000000;
            var jobSchedules = Enumerable.Repeat(new Random(), count).Select(JobSchedule.CreateRandom);
            Assert.AreEqual(count, jobSchedules.Count());
        }

        [TestMethod]
        public void TestWorkersAtWeekTimePoint()
        {
            const int count = 1000000;
            var jobSchedules = Enumerable.Repeat(new Random(), count).Select(JobSchedule.CreateRandom).ToList();
            var workersAtMondayMorning = CountWorkers(jobSchedules, new WeekTimePoint(DayOfWeek.Monday, 7));
            var workersAtMondayNoon = CountWorkers(jobSchedules, new WeekTimePoint(DayOfWeek.Monday, 12));
            var workersAtMondayEvening = CountWorkers(jobSchedules, new WeekTimePoint(DayOfWeek.Monday, 20));
            var workersAtSaturdayNoon = CountWorkers(jobSchedules, new WeekTimePoint(DayOfWeek.Saturday, 12));
            var workersAtSundayNoon = CountWorkers(jobSchedules, new WeekTimePoint(DayOfWeek.Sunday, 12));
            Console.WriteLine($"Monday at 7AM: {workersAtMondayMorning} ({workersAtMondayMorning * 100f / count}%)");
            Console.WriteLine($"Monday at noon: {workersAtMondayNoon} ({workersAtMondayNoon * 100f / count}%)");
            Console.WriteLine($"Monday at 8PM: {workersAtMondayEvening} ({workersAtMondayEvening * 100f / count}%)");
            Console.WriteLine($"Saturday at noon: {workersAtSaturdayNoon} ({workersAtSaturdayNoon * 100f / count}%)");
            Console.WriteLine($"Sunday at noon: {workersAtSundayNoon} ({workersAtSundayNoon * 100f / count}%)");
        }

        private static int CountWorkers(IEnumerable<JobSchedule> schedules, WeekTimePoint wtp)
        {
            return schedules.Count(js => js.WeekTimeSpans.Any(wts => wts.IsInside(wtp)));
        }
    }
}
