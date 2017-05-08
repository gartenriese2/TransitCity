using Geometry;

namespace CitySimulation
{
    public class Job
    {
        public Job(Position2d position)
        {
            Position = position;
        }

        public Position2d Position { get; }

        public Resident Worker { get; set; }
    }
}
