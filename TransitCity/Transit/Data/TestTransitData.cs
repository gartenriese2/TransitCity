using System;
using System.Collections.Generic;
using Geometry;
using Time;
using Utility.Units;

namespace Transit.Data
{
    public class TestTransitData
    {
        public TestTransitData()
        {
            Initialize();
        }

        public DataManager DataManager { get; } = new DataManager();

        private void Initialize()
        {
            CreateSubwayLines();
        }

        private void CreateSubwayLines()
        {
            DataManager.AddSubwayLine(
                new Dictionary<Position2d, string>
                {
                    [new Position2d(1500, 1000)] = "Stanmore",
                    [new Position2d(2500, 1000)] = "Canons Park",
                    [new Position2d(3500, 1500)] = "Queensbury",
                    [new Position2d(4000, 2500)] = "Kingsbury",
                    [new Position2d(4500, 3500)] = "Wembley Park",
                    [new Position2d(5500, 4000)] = "Neasden",
                    [new Position2d(6500, 3940)] = "Clapham South",
                    [new Position2d(7500, 4000)] = "Dollis Hill",
                    [new Position2d(8000, 5000)] = "Willesden Green",
                    [new Position2d(8000, 6000)] = "Kilburn",
                    [new Position2d(7500, 7000)] = "West Hampstead",
                    [new Position2d(7500, 8000)] = "Finchley Road",
                    [new Position2d(8000, 9000)] = "Swiss Cottage"
                },
                "1",
                CreateTimetableLine1Outward(),
                CreateTimetableLine1Inward(),
                Duration.FromSeconds(30));
            DataManager.AddSubwayLine(
                new Dictionary<Position2d, string>
                {
                    [new Position2d(1500, 8000)] = "Morden",
                    [new Position2d(2500, 7500)] = "South Wimbledon",
                    [new Position2d(3500, 7000)] = "Colliers Wood",
                    [new Position2d(4500, 6500)] = "Tooting Broadway",
                    [new Position2d(5500, 5500)] = "Tooting Bec",
                    [new Position2d(5970, 4800)] = "Balham",
                    [new Position2d(6500, 3900)] = "Clapham South",
                    [new Position2d(7000, 3000)] = "Clapham Common",
                    [new Position2d(7000, 2000)] = "Clapham North",
                    [new Position2d(8000, 1500)] = "Stockwell",
                    [new Position2d(9000, 1500)] = "Oval"
                },
                "2",
                CreateTimetableLine2Outward(),
                CreateTimetableLine2Inward(),
                Duration.FromSeconds(30));
            DataManager.AddSubwayLine(
                new Dictionary<Position2d, string>
                {
                    [new Position2d(5500, 750)] = "Chesham",
                    [new Position2d(5500, 2100)] = "Amersham",
                    [new Position2d(5500, 3000)] = "Bow Church",
                    [new Position2d(5500, 4000)] = "Neasden",
                    [new Position2d(6060, 4850)] = "Balham",
                    [new Position2d(6750, 5750)] = "Chalfont",
                    [new Position2d(8000, 6000)] = "Kilburn",
                    [new Position2d(9000, 6250)] = "Watford",
                    [new Position2d(9900, 6300)] = "Croxley"
                },
                "3",
                CreateTimetableLine3Outward(),
                CreateTimetableLine3Inward(),
                Duration.FromSeconds(30));
            DataManager.AddSubwayLine(
                new Dictionary<Position2d, string>
                {
                    [new Position2d(500, 5500)] = "Epping",
                    [new Position2d(1500, 5000)] = "Theydon Bois",
                    [new Position2d(2500, 4500)] = "Debden",
                    [new Position2d(3500, 4500)] = "Loughton",
                    [new Position2d(4750, 5000)] = "Buckhurst Hill",
                    [new Position2d(5950, 4900)] = "Balham",
                    [new Position2d(6900, 4500)] = "Woodford",
                    [new Position2d(7500, 4100)] = "Dollis Hill",
                    [new Position2d(8500, 3500)] = "Roding Valley",
                    [new Position2d(9600, 3600)] = "Chigwell"
                },
                "4",
                CreateTimetableLine4Outward(),
                CreateTimetableLine4Inward(),
                Duration.FromSeconds(30));
        }

        private static WeekTimeCollection CreateTimetableLine1Outward()
        {
            var wtc = new WeekTimeCollection();
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                var isWeekend = day == DayOfWeek.Saturday || day == DayOfWeek.Sunday;
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day), new WeekTimePoint(day, 4, 59), TimeSpan.FromMinutes(isWeekend ? 20 : 30)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 5), new WeekTimePoint(day, 8, 59), TimeSpan.FromMinutes(isWeekend ? 10 : 5)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 9), new WeekTimePoint(day, 15, 29), TimeSpan.FromMinutes(10)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 15, 30), new WeekTimePoint(day, 18, 29), TimeSpan.FromMinutes(isWeekend ? 10 : 5)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 18, 30), new WeekTimePoint(day, 21, 59), TimeSpan.FromMinutes(10)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 22), new WeekTimePoint(day, 23, 59), TimeSpan.FromMinutes(20)));
            }
            return wtc;
        }

        private static WeekTimeCollection CreateTimetableLine1Inward()
        {
            var offset = TimeSpan.FromMinutes(25);
            var wtc = new WeekTimeCollection();
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                var isWeekend = day == DayOfWeek.Saturday || day == DayOfWeek.Sunday;
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day) + offset, new WeekTimePoint(day, 4, 59) + offset, TimeSpan.FromMinutes(isWeekend ? 20 : 30)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 5) + offset, new WeekTimePoint(day, 8, 59) + offset, TimeSpan.FromMinutes(isWeekend ? 10 : 5)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 9) + offset, new WeekTimePoint(day, 15, 29) + offset, TimeSpan.FromMinutes(10)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 15, 30) + offset, new WeekTimePoint(day, 18, 29) + offset, TimeSpan.FromMinutes(isWeekend ? 10 : 5)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 18, 30) + offset, new WeekTimePoint(day, 21, 59) + offset, TimeSpan.FromMinutes(10)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 22) + offset, new WeekTimePoint(day, 23, 59) + offset, TimeSpan.FromMinutes(20)));
            }
            return wtc;
        }

        private static WeekTimeCollection CreateTimetableLine2Outward()
        {
            var wtc = new WeekTimeCollection();
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                var isWeekend = day == DayOfWeek.Saturday || day == DayOfWeek.Sunday;
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day), new WeekTimePoint(day, 4, 59), TimeSpan.FromMinutes(isWeekend ? 20 : 30)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 5), new WeekTimePoint(day, 8, 59), TimeSpan.FromMinutes(isWeekend ? 10 : 5)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 9), new WeekTimePoint(day, 15, 29), TimeSpan.FromMinutes(10)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 15, 30), new WeekTimePoint(day, 18, 29), TimeSpan.FromMinutes(isWeekend ? 10 : 5)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 18, 30), new WeekTimePoint(day, 21, 59), TimeSpan.FromMinutes(10)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 22), new WeekTimePoint(day, 23, 59), TimeSpan.FromMinutes(20)));
            }
            return wtc;
        }

        private static WeekTimeCollection CreateTimetableLine2Inward()
        {
            var offset = TimeSpan.FromMinutes(29);
            var wtc = new WeekTimeCollection();
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                var isWeekend = day == DayOfWeek.Saturday || day == DayOfWeek.Sunday;
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day) + offset, new WeekTimePoint(day, 4, 59) + offset, TimeSpan.FromMinutes(isWeekend ? 20 : 30)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 5) + offset, new WeekTimePoint(day, 8, 59) + offset, TimeSpan.FromMinutes(isWeekend ? 10 : 5)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 9) + offset, new WeekTimePoint(day, 15, 29) + offset, TimeSpan.FromMinutes(10)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 15, 30) + offset, new WeekTimePoint(day, 18, 29) + offset, TimeSpan.FromMinutes(isWeekend ? 10 : 5)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 18, 30) + offset, new WeekTimePoint(day, 21, 59) + offset, TimeSpan.FromMinutes(10)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 22) + offset, new WeekTimePoint(day, 23, 59) + offset, TimeSpan.FromMinutes(20)));
            }
            return wtc;
        }

        private static WeekTimeCollection CreateTimetableLine3Outward()
        {
            var wtc = new WeekTimeCollection();
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                var isWeekend = day == DayOfWeek.Saturday || day == DayOfWeek.Sunday;
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day), new WeekTimePoint(day, 4, 59), TimeSpan.FromMinutes(isWeekend ? 15 : 20)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 5), new WeekTimePoint(day, 8, 59), TimeSpan.FromMinutes(isWeekend ? 7.5 : 4)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 9), new WeekTimePoint(day, 15, 29), TimeSpan.FromMinutes(7.5)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 15, 30), new WeekTimePoint(day, 18, 29), TimeSpan.FromMinutes(isWeekend ? 7.5 : 4)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 18, 30), new WeekTimePoint(day, 21, 59), TimeSpan.FromMinutes(7.5)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 22), new WeekTimePoint(day, 23, 59), TimeSpan.FromMinutes(15)));
            }
            return wtc;
        }

        private static WeekTimeCollection CreateTimetableLine3Inward()
        {
            var offset = TimeSpan.FromMinutes(17);
            var wtc = new WeekTimeCollection();
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                var isWeekend = day == DayOfWeek.Saturday || day == DayOfWeek.Sunday;
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day) + offset, new WeekTimePoint(day, 4, 59) + offset, TimeSpan.FromMinutes(isWeekend ? 15 : 20)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 5) + offset, new WeekTimePoint(day, 8, 59) + offset, TimeSpan.FromMinutes(isWeekend ? 7.5 : 4)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 9) + offset, new WeekTimePoint(day, 15, 29) + offset, TimeSpan.FromMinutes(7.5)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 15, 30) + offset, new WeekTimePoint(day, 18, 29) + offset, TimeSpan.FromMinutes(isWeekend ? 7.5 : 4)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 18, 30) + offset, new WeekTimePoint(day, 21, 59) + offset, TimeSpan.FromMinutes(7.5)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 22) + offset, new WeekTimePoint(day, 23, 59) + offset, TimeSpan.FromMinutes(15)));
            }
            return wtc;
        }

        private static WeekTimeCollection CreateTimetableLine4Outward()
        {
            var wtc = new WeekTimeCollection();
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                var isWeekend = day == DayOfWeek.Saturday || day == DayOfWeek.Sunday;
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day), new WeekTimePoint(day, 4, 59), TimeSpan.FromMinutes(isWeekend ? 20 : 40)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 5), new WeekTimePoint(day, 8, 59), TimeSpan.FromMinutes(isWeekend ? 10 : 5)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 9), new WeekTimePoint(day, 15, 29), TimeSpan.FromMinutes(10)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 15, 30), new WeekTimePoint(day, 18, 29), TimeSpan.FromMinutes(isWeekend ? 10 : 5)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 18, 30), new WeekTimePoint(day, 21, 59), TimeSpan.FromMinutes(10)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 22), new WeekTimePoint(day, 23, 59), TimeSpan.FromMinutes(20)));
            }
            return wtc;
        }

        private static WeekTimeCollection CreateTimetableLine4Inward()
        {
            var offset = TimeSpan.FromMinutes(19);
            var wtc = new WeekTimeCollection();
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                var isWeekend = day == DayOfWeek.Saturday || day == DayOfWeek.Sunday;
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day) + offset, new WeekTimePoint(day, 4, 59) + offset, TimeSpan.FromMinutes(isWeekend ? 20 : 40)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 5) + offset, new WeekTimePoint(day, 8, 59) + offset, TimeSpan.FromMinutes(isWeekend ? 10 : 5)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 9) + offset, new WeekTimePoint(day, 15, 29) + offset, TimeSpan.FromMinutes(10)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 15, 30) + offset, new WeekTimePoint(day, 18, 29) + offset, TimeSpan.FromMinutes(isWeekend ? 10 : 5)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 18, 30) + offset, new WeekTimePoint(day, 21, 59) + offset, TimeSpan.FromMinutes(10)));
                wtc.AddCollection(new WeekTimeCollection(new WeekTimePoint(day, 22) + offset, new WeekTimePoint(day, 23, 59) + offset, TimeSpan.FromMinutes(20)));
            }
            return wtc;
        }
    }
}
