namespace CitySimulation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Geometry.Shapes;

    using Utility.Units;

    public class CombinedDistrict : IDistrict
    {
        private readonly List<IDistrict> _subDistricts;

        public CombinedDistrict(string name, IEnumerable<IDistrict> subDistricts)
        {
            _subDistricts = subDistricts?.ToList() ?? throw new ArgumentNullException(nameof(subDistricts));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Shape = new CombinedShape(_subDistricts.Select(d => d.Shape));
            Residents = _subDistricts.SelectMany(d => d.Residents).ToList();
            Jobs = _subDistricts.SelectMany(d => d.Jobs).ToList();
            Area = Area.FromSqaureMeters(_subDistricts.Sum(d => d.Area.SquareMeters));
            PopulationDensity = Residents.Count / Area.SquareKilometers;
            JobDensity = Jobs.Count / Area.SquareKilometers;
        }

        public string Name { get; }

        public IShape Shape { get; }

        public List<Resident> Residents { get; }

        public List<Job> Jobs { get; }

        public Area Area { get; }

        public double PopulationDensity { get; }

        public double JobDensity { get; }

        public IEnumerable<IDistrict> SubDistricts => _subDistricts;
    }
}
