namespace TransitCity.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MVVM;

    using Utility.Coordinates;

    public class CityModel : PropertyChangedBase
    {
        private static readonly Random Rnd = new Random();
        private readonly uint _residentialBuildings;
        private uint _residents;

        public CityModel(uint residentialBuildings, double size)
        {
            _residentialBuildings = residentialBuildings;
            WorldPosition.Size = size;
        }

        public List<ResidentialBuildingModel> ResidentialBuildings { get; } = new List<ResidentialBuildingModel>();

        public List<JobModel> Jobs { get; } = new List<JobModel>();

        public List<Connection> Connections { get; } = new List<Connection>();

        public uint Residents
        {
            get { return _residents; }
            set
            {
                if (value != _residents)
                {
                    _residents = value;
                    OnPropertyChanged();
                }
            }
        }

        public void Initialize()
        {
            var residentialBuildingList = new List<ResidentialBuildingModel>((int)_residentialBuildings);
            var jobBuildingDictionary = new Dictionary<JobModel, int>();
            var residentCounter = 0;
            for (var i = 0u; i < _residentialBuildings; ++i)
            {
                var numResidents = (int)CreateRandomResidentSize();
                Residents += (uint)numResidents;
                var residentList = new List<ResidentModel>(numResidents);
                for (var j = 0; j < numResidents; ++j)
                {
                    residentList.Add(new ResidentModel(null));
                }

                residentialBuildingList.Add(new ResidentialBuildingModel(ModelPosition.CreateRandom(), residentList));

                residentCounter += numResidents;
                if (i != 0 && i % 5 == 0)
                {
                    jobBuildingDictionary[new JobModel(CreateJobPosition(), null, (uint)residentCounter)] = residentCounter;
                    residentCounter = 0;
                }
            }

            if (residentCounter > 0)
            {
                jobBuildingDictionary[new JobModel(CreateJobPosition(), null, (uint)residentCounter)] = residentCounter;
            }

            foreach (var residentialBuilding in residentialBuildingList)
            {
                foreach (var resident in residentialBuilding.Residents)
                {
                    var remainingJobs = jobBuildingDictionary.Where(tuple => tuple.Value > 0).Select(pair => pair.Key).ToList();
                    var idx = Rnd.Next(remainingJobs.Count);
                    var job = remainingJobs[idx];
                    var connection = new Connection(residentialBuilding, job);
                    resident.Connection = connection;
                    if (job.Connections.Where(c => c.ResidentialBuilding == residentialBuilding).ToList().Count == 0)
                    {
                        job.Connections.Add(connection);
                        Connections.Add(connection);
                    }

                    --jobBuildingDictionary[job];
                }

                ResidentialBuildings.Add(residentialBuilding);
            }

            foreach (var pair in jobBuildingDictionary)
            {
                Jobs.Add(pair.Key);
            }
        }

        private static uint CreateRandomResidentSize()
        {
            var value = Rnd.NextDouble();
            if (value < 0.5)
            {
                // 1 - 2
                return (uint)Rnd.Next(1, 3);
            }

            if (value < 0.75)
            {
                // 3 - 5
                return (uint)Rnd.Next(3, 6);
            }

            if (value < 0.85)
            {
                // 6 - 10
                return (uint)Rnd.Next(6, 11);
            }

            if (value < 0.92)
            {
                // 11 - 20
                return (uint)Rnd.Next(11, 21);
            }

            if (value < 0.97)
            {
                // 21 - 50
                return (uint)Rnd.Next(21, 51);
            }

            // 51 - 100
            return (uint)Rnd.Next(51, 101);
        }

        private double GetNextGaussian()
        {
            var u1 = Rnd.NextDouble();
            var u2 = Rnd.NextDouble();
            var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return 0.5 + 0.2 * randStdNormal;
        }

        private ModelPosition CreateJobPosition()
        {
            var x = GetNextGaussian();
            while (x < 0.0 || x > 1.0)
            {
                x = GetNextGaussian();
            }

            var y = GetNextGaussian();
            while (y < 0.0 || y > 1.0)
            {
                y = GetNextGaussian();
            }

            return new ModelPosition(x, y);
        }
    }
}
