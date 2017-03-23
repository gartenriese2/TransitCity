using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Time;
using Utility.Timing;

namespace TimeUnitTest
{
    [TestClass]
    public class WeekTimeCollectionTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var wtc = new WeekTimeCollection(new TimeSpan(5, 30, 0), new TimeSpan(23, 30, 0), TimeSpan.FromMinutes(10.0), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
        }

        [TestMethod]
        public void FindTest()
        {
            var wtc = new WeekTimeCollection(new TimeSpan(5, 30, 0), new TimeSpan(23, 30, 0), TimeSpan.FromMinutes(1.0), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday });
            var departure = new WeekTimePoint(DayOfWeek.Tuesday, 11, 30, 24);

            var vt = Timing.Profile(() => GetNextDeparture(wtc, departure), 100000);
            Console.WriteLine($"FirstOrDefault: {vt.timespan:g}");
            Console.WriteLine(vt.results[0]);

            var sortedList = new SortedSet<WeekTimePoint>(wtc.SortedWeekTimePoints);
            var vt2 = Timing.Profile(() => GetNextDeparture2(sortedList, departure), 100000);
            Console.WriteLine($"FirstOrDefault SortedSet: {vt2.timespan:g}");
            Console.WriteLine(vt.results[0]);

            var sortedArray = wtc.SortedWeekTimePoints.ToArray();
            var vt3 = Timing.Profile(() => GetNextDeparture3(sortedArray, departure), 100000);
            Console.WriteLine($"Array BinarySearch: {vt3.timespan:g}");
            Console.WriteLine(vt.results[0]);

            WeekTimePoint GetNextDeparture(WeekTimeCollection collection, WeekTimePoint time)
            {
                return collection.SortedWeekTimePoints.FirstOrDefault(wtp => time <= wtp) ?? collection.SortedWeekTimePoints.FirstOrDefault();
            }

            WeekTimePoint GetNextDeparture2(SortedSet<WeekTimePoint> collection, WeekTimePoint time)
            {
                return collection.FirstOrDefault(wtp => time <= wtp) ?? collection.FirstOrDefault();
            }

            WeekTimePoint GetNextDeparture3(WeekTimePoint[] array, WeekTimePoint time)
            {
                var idx = Array.BinarySearch(array, time);
                if (idx < 0)
                {
                    idx = ~idx - 1;
                }

                if (idx >= 0)
                {
                    return array[idx];
                }

                throw new InvalidOperationException();
            }
        }

        
    }
}
