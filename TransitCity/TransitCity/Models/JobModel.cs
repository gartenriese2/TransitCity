namespace TransitCity.Models
{
    using System.Collections.Generic;

    using Utility.Coordinates;

    public class JobModel
    {
        public JobModel(ModelPosition pos, List<Connection> connections, uint numJobs)
        {
            Position = pos;
            if (connections != null)
            {
                Connections = connections;
            }

            NumJobs = numJobs;
        }

        public ModelPosition Position { get; }

        public List<Connection> Connections { get; } = new List<Connection>();

        public uint NumJobs { get; }
    }
}
