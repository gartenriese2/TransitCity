namespace TransitCity.Models
{
    public class Connection
    {
        public Connection(ResidentialBuildingModel residentialBuilding, JobModel job)
        {
            ResidentialBuilding = residentialBuilding;
            Job = job;
        }

        public ResidentialBuildingModel ResidentialBuilding { get; }

        public JobModel Job { get; }
    }
}
