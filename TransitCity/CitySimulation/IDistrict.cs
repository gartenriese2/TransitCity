using System.Collections.Generic;
using Geometry.Shapes;
using Utility.Units;

namespace CitySimulation
{
    public interface IDistrict
    {
        string Name { get; }

        IShape Shape { get; }

        List<Resident> Residents { get; }

        List<Job> Jobs { get; }

        Area Area { get; }

        double PopulationDensity { get; }

        double JobDensity { get; }
    }
}
