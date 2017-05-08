using System;
using System.Collections.Generic;
using Geometry.Shapes;
using Utility.Units;

namespace CitySimulation
{
    public class RandomDistrict : IDistrict
    {
        private readonly IShape _shape;

        public RandomDistrict(string name, IShape shape, uint residents, uint jobs)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _shape = shape;
            Residents = new List<Resident>((int) residents);
            Jobs = new List<Job>((int) jobs);

            var rnd = new Random();
            for (var i = 0; i < residents; ++i)
            {
                var pos = _shape.CreateRandomPoint(rnd);
                Residents.Add(new Resident(pos));
            }

            for (var i = 0; i < jobs; ++i)
            {
                var pos = _shape.CreateRandomPoint(rnd);
                Jobs.Add(new Job(pos));
            }
        }

        public string Name { get; }

        public IShape Shape => _shape;

        public List<Resident> Residents { get; }

        public List<Job> Jobs { get; }

        public Area Area => Area.FromSqaureMeters(_shape.Area);

        public double PopulationDensity => Residents.Count / Area.SquareKilometers;

        public double JobDensity => Jobs.Count / Area.SquareKilometers;
    }
}
