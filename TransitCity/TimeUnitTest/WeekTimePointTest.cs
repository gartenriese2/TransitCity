using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Time;

namespace TimeUnitTest
{
    [TestClass]
    public class WeekTimePointTest
    {
        [TestMethod]
        public void ToStringTest()
        {
            var weekTimePoint = new WeekTimePoint(3, 11, 24, 31);
            var str = weekTimePoint.ToString();
            Assert.AreEqual(str, "Thursday 11:24:31");
        }

        [TestMethod]
        public void AddTest()
        {
            var wtp = new WeekTimePoint(DayOfWeek.Sunday, 23, 30);
            wtp += TimeSpan.FromHours(1);
            Assert.AreEqual(wtp.GetDayOfWeek(), DayOfWeek.Monday);
            wtp += TimeSpan.FromDays(13);
            Assert.AreEqual(wtp.GetDayOfWeek(), DayOfWeek.Sunday);
        }

        [TestMethod]
        public void SubTest()
        {
            var wtp = new WeekTimePoint(DayOfWeek.Monday, 0, 30);
            wtp -= TimeSpan.FromHours(1);
            Assert.AreEqual(wtp.GetDayOfWeek(), DayOfWeek.Sunday);
            wtp -= TimeSpan.FromDays(13);
            Assert.AreEqual(wtp.GetDayOfWeek(), DayOfWeek.Monday);
        }
    }
}
