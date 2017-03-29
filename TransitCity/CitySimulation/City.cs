using System;
using System.Collections.Generic;
using System.Linq;
using Utility.Units;

namespace CitySimulation
{
    public class City
    {
        private readonly List<IDistrict> _districts;

        public City(List<IDistrict> districts)
        {
            _districts = districts ?? throw new ArgumentNullException(nameof(districts));
            Connect();
        }

        public IEnumerable<IDistrict> Districts => _districts;

        public IEnumerable<Resident> Residents => _districts.SelectMany(d => d.Residents);

        public IEnumerable<Job> Jobs => _districts.SelectMany(d => d.Jobs);

        public Area Area => _districts.Aggregate(Area.FromSqaureMeters(0), (a, d) => a + d.Area);

        public float PopulationDensity => Residents.Count() / Area.SquareKilometers;

        public float JobDensity => Jobs.Count() / Area.SquareKilometers;

        private void Connect()
        {
            var rnd = new Random();
            var shuffledResidents = Residents.ToArray();
            for (var i = shuffledResidents.Length - 1; i > 0; --i)
            {
                var j = rnd.Next(i + 1);
                var tmp = shuffledResidents[j];
                shuffledResidents[j] = shuffledResidents[i];
                shuffledResidents[i] = tmp;
            }

            var shuffledJobs = Jobs.ToArray();
            for (var i = shuffledJobs.Length - 1; i > 0; --i)
            {
                var j = rnd.Next(i + 1);
                var tmp = shuffledJobs[j];
                shuffledJobs[j] = shuffledJobs[i];
                shuffledJobs[i] = tmp;
            }

            for (var i = 0; i < Math.Min(shuffledResidents.Length, shuffledJobs.Length); ++i)
            {
                shuffledResidents[i].Job = shuffledJobs[i];
                shuffledJobs[i].Worker = shuffledResidents[i];
            }
        }
    }
}
