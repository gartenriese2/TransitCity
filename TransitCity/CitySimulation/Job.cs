using Geometry;

namespace CitySimulation
{
    public class Job
    {
        public Job(Position2f position)
        {
            Position = position;
        }

        public Position2f Position { get; }

        public Resident Worker { get; set; }
    }
}
