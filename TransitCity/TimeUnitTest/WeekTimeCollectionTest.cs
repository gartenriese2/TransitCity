using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Time;

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
    }
}
