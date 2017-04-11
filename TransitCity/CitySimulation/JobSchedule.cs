using System;
using System.Collections.Generic;
using Time;

namespace CitySimulation
{
    public class JobSchedule : Schedule
    {
        public JobSchedule(List<WeekTimeSpan> weekTimeSpans) : base(weekTimeSpans)
        {
        }

        public static JobSchedule CreateRandom(Random rnd)
        {
            var value = rnd.NextDouble();

            if (value < 0.5)
            {
                var ts = TimeSpan.FromHours(value * 6);
                var startTimeMon = new WeekTimePoint(DayOfWeek.Monday, 6, 30) + ts;
                var startTimeTue = new WeekTimePoint(DayOfWeek.Tuesday, 6, 30) + ts;
                var startTimeWed = new WeekTimePoint(DayOfWeek.Wednesday, 6, 30) + ts;
                var startTimeThu = new WeekTimePoint(DayOfWeek.Thursday, 6, 30) + ts;
                var startTimeFri = new WeekTimePoint(DayOfWeek.Friday, 6, 30) + ts;

                var caseValue = rnd.Next(3);
                ts = TimeSpan.FromHours(caseValue + 7);
                var endTimeMon = startTimeMon + ts;
                var endTimeTue = startTimeTue + ts;
                var endTimeWed = startTimeWed + ts;
                var endTimeThu = startTimeThu + ts;
                var endTimeFri = startTimeFri + ts;

                return new JobSchedule(new List<WeekTimeSpan>
                {
                    new WeekTimeSpan(startTimeMon, endTimeMon),
                    new WeekTimeSpan(startTimeTue, endTimeTue),
                    new WeekTimeSpan(startTimeWed, endTimeWed),
                    new WeekTimeSpan(startTimeThu, endTimeThu),
                    new WeekTimeSpan(startTimeFri, endTimeFri)
                });
            }
            else if (value < 0.8)
            {
                // TODO
            }
            // TODO

            throw new NotImplementedException();
        }
    }
}
