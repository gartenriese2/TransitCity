namespace TransitCity.Models.Transit
{
    public class StationConnection
    {
        public StationConnection(Station stationA, Station stationB)
        {
            StationA = stationA;
            StationB = stationB;
        }

        public Station StationA { get; }

        public Station StationB { get; }

        public double GetModelDistance() => StationA.Position.GetDistanceTo(StationB.Position);
    }
}
