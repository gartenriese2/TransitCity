using System;
using System.Collections.Generic;
using System.Linq;
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

            if (value < 0.5) // Bürojob
            {
                var duration = TimeSpan.FromHours(rnd.NextDouble() * 2 + 7); // 7 - 9 workhours
                var startRange = TimeSpan.FromHours(rnd.NextDouble() * 3); // 3 hours
                var blur = TimeSpan.FromMinutes(30);
                var earliestStart = TimeSpan.FromHours(6.5);
                return CreateJobSchedule(
                    new[]
                    {
                        DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday
                    }, earliestStart, startRange, duration, blur, rnd);
            }

            if (value < 0.85) // Dienstleistung
            {
                var duration = TimeSpan.FromHours(rnd.NextDouble() + 8); // 8 - 9 workhours
                var startRange = TimeSpan.FromHours(rnd.NextDouble() * 3); // 3 hours
                var blur = TimeSpan.FromMinutes(20);
                var startRnd = rnd.NextDouble();
                var earliestStart = startRnd < 0.333 ? TimeSpan.FromHours(5.5) : startRnd < 0.666 ? TimeSpan.FromHours(11) : TimeSpan.FromHours(16.5);
                if (value < 0.7) // Mo - Fr
                {
                    return CreateJobSchedule(
                        new[]
                        {
                            DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday
                        }, earliestStart, startRange, duration, blur, rnd);
                }

                if (value < 0.8) // Tu - Sa
                {
                    return CreateJobSchedule(
                        new[]
                        {
                            DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday
                        }, earliestStart, startRange, duration, blur, rnd);
                }

                if (value < 0.81) // We - Su
                {
                    return CreateJobSchedule(
                        new[]
                        {
                            DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday
                        }, earliestStart, startRange, duration, blur, rnd);
                }

                if (value < 0.82) // Th - Mo
                {
                    return CreateJobSchedule(
                        new[]
                        {
                            DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday, DayOfWeek.Monday
                        }, earliestStart, startRange, duration, blur, rnd);
                }

                if (value < 0.83) // Fr - Tu
                {
                    return CreateJobSchedule(
                        new[]
                        {
                            DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday
                        }, earliestStart, startRange, duration, blur, rnd);
                }

                if (value < 0.84) // Sa - We
                {
                    return CreateJobSchedule(
                        new[]
                        {
                            DayOfWeek.Saturday, DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday
                        }, earliestStart, startRange, duration, blur, rnd);
                }

                // Su - Th
                return CreateJobSchedule(
                    new[]
                    {
                        DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday
                    }, earliestStart, startRange, duration, blur, rnd);
            }

            {
                // shift work
                var duration = TimeSpan.FromHours(8); // 8 workhours
                var startRange = TimeSpan.FromMinutes(rnd.NextDouble() * 20); // 20 minutes
                var blur = TimeSpan.FromMinutes(10);
                var startRnd = rnd.NextDouble();
                var earliestStart = startRnd < 0.333 ? TimeSpan.FromHours(5.5) : startRnd < 0.666 ? TimeSpan.FromHours(13.5) : TimeSpan.FromHours(21.5);
                if (value < 0.88) // Mo - Fr
                {
                    return CreateJobSchedule(
                        new[]
                        {
                            DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday
                        }, earliestStart, startRange, duration, blur, rnd);
                }

                if (value < 0.9) // Tu - Sa
                {
                    return CreateJobSchedule(
                        new[]
                        {
                            DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday
                        }, earliestStart, startRange, duration, blur, rnd);
                }

                if (value < 0.92) // We - Su
                {
                    return CreateJobSchedule(
                        new[]
                        {
                            DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday
                        }, earliestStart, startRange, duration, blur, rnd);
                }

                if (value < 0.94) // Th - Mo
                {
                    return CreateJobSchedule(
                        new[]
                        {
                            DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday, DayOfWeek.Monday
                        }, earliestStart, startRange, duration, blur, rnd);
                }

                if (value < 0.96) // Fr - Tu
                {
                    return CreateJobSchedule(
                        new[]
                        {
                            DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday
                        }, earliestStart, startRange, duration, blur, rnd);
                }

                if (value < 0.98) // Sa - We
                {
                    return CreateJobSchedule(
                        new[]
                        {
                            DayOfWeek.Saturday, DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday
                        }, earliestStart, startRange, duration, blur, rnd);
                }

                // Su - Th
                return CreateJobSchedule(
                    new[]
                    {
                        DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday
                    }, earliestStart, startRange, duration, blur, rnd);
            }
        }

        private static JobSchedule CreateJobSchedule(IEnumerable<DayOfWeek> days, TimeSpan earliestStart, TimeSpan startRange, TimeSpan duration, TimeSpan blur, Random rnd)
        {
            return new JobSchedule(days.Select(day => CreateWeekTimeSpan(day, earliestStart, startRange, duration, blur, rnd)).ToList());
        }

        private static WeekTimeSpan CreateWeekTimeSpan(DayOfWeek startDay, TimeSpan earliestStart, TimeSpan startRange, TimeSpan duration, TimeSpan blur, Random rnd)
        {
            var startTime = new WeekTimePoint(startDay) + earliestStart + startRange + BlurredMinutes(rnd, blur);
            var endTime = startTime + duration + BlurredMinutes(rnd, blur);
            return new WeekTimeSpan(startTime, endTime);
        }

        private static TimeSpan BlurredMinutes(Random rnd, TimeSpan timespan)
        {
            return TimeSpan.FromMinutes((rnd.NextDouble() - 0.5) * timespan.TotalMinutes);
        }
    }
}
