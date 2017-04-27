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
                new Dictionary<Position2f, string>
                {
                    [new Position2f(1500, 1000)] = "Stanmore",
                    [new Position2f(2500, 1000)] = "Canons Park",
                    [new Position2f(3500, 1500)] = "Queensbury",
                    [new Position2f(4000, 2500)] = "Kingsbury",
                    [new Position2f(4500, 3500)] = "Wembley Park",
                    [new Position2f(5500, 4000)] = "Neasden",
                    [new Position2f(6500, 3940)] = "Clapham South",
                    [new Position2f(7500, 4000)] = "Dollis Hill",
                    [new Position2f(8000, 5000)] = "Willesden Green",
                    [new Position2f(8000, 6000)] = "Kilburn",
                    [new Position2f(7500, 7000)] = "West Hampstead",
                    [new Position2f(7500, 8000)] = "Finchley Road",
                    [new Position2f(8000, 9000)] = "Swiss Cottage"
                },
                "1",
                CreateTimetableLine1(),
                new WeekTimeCollection(new TimeSpan(5, 30, 0), new TimeSpan(23, 30, 0), TimeSpan.FromMinutes(4), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday }),
                Duration.FromSeconds(30));
            DataManager.AddSubwayLine(
                new Dictionary<Position2f, string>
                {
                    [new Position2f(1500, 8000)] = "Morden",
                    [new Position2f(2500, 7500)] = "South Wimbledon",
                    [new Position2f(3500, 7000)] = "Colliers Wood",
                    [new Position2f(4500, 6500)] = "Tooting Broadway",
                    [new Position2f(5500, 5500)] = "Tooting Bec",
                    [new Position2f(5970, 4800)] = "Balham",
                    [new Position2f(6500, 3900)] = "Clapham South",
                    [new Position2f(7000, 3000)] = "Clapham Common",
                    [new Position2f(7000, 2000)] = "Clapham North",
                    [new Position2f(8000, 1500)] = "Stockwell",
                    [new Position2f(9000, 1500)] = "Oval"
                },
                "2",
                new WeekTimeCollection(new TimeSpan(5, 30, 0), new TimeSpan(23, 30, 0), TimeSpan.FromMinutes(10), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday }),
                new WeekTimeCollection(new TimeSpan(5, 30, 0), new TimeSpan(23, 30, 0), TimeSpan.FromMinutes(10), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday }),
                Duration.FromSeconds(30));
            DataManager.AddSubwayLine(
                new Dictionary<Position2f, string>
                {
                    [new Position2f(5500, 750)] = "Chesham",
                    [new Position2f(5500, 2100)] = "Amersham",
                    [new Position2f(5500, 3000)] = "Bow Church",
                    [new Position2f(5500, 4000)] = "Neasden",
                    [new Position2f(6060, 4850)] = "Balham",
                    [new Position2f(6750, 5750)] = "Chalfont",
                    [new Position2f(8000, 6000)] = "Kilburn",
                    [new Position2f(9000, 6250)] = "Watford",
                    [new Position2f(9900, 6300)] = "Croxley"
                },
                "3",
                new WeekTimeCollection(new TimeSpan(5, 30, 0), new TimeSpan(23, 30, 0), TimeSpan.FromMinutes(2), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday }),
                new WeekTimeCollection(new TimeSpan(5, 30, 0), new TimeSpan(23, 30, 0), TimeSpan.FromMinutes(2), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday }),
                Duration.FromSeconds(30));
            DataManager.AddSubwayLine(
                new Dictionary<Position2f, string>
                {
                    [new Position2f(500, 5500)] = "Epping",
                    [new Position2f(1500, 5000)] = "Theydon Bois",
                    [new Position2f(2500, 4500)] = "Debden",
                    [new Position2f(3500, 4500)] = "Loughton",
                    [new Position2f(4750, 5000)] = "Buckhurst Hill",
                    [new Position2f(5950, 4900)] = "Balham",
                    [new Position2f(6900, 4500)] = "Woodford",
                    [new Position2f(7500, 4100)] = "Dollis Hill",
                    [new Position2f(8500, 3500)] = "Roding Valley",
                    [new Position2f(9600, 3600)] = "Chigwell"
                },
                "4",
                new WeekTimeCollection(new TimeSpan(5, 30, 0), new TimeSpan(23, 30, 0), TimeSpan.FromMinutes(4), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday }),
                new WeekTimeCollection(new TimeSpan(5, 30, 0), new TimeSpan(23, 30, 0), TimeSpan.FromMinutes(4), new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday }),
                Duration.FromSeconds(30));
        }

        private WeekTimeCollection CreateTimetableLine1()
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
    }
}
