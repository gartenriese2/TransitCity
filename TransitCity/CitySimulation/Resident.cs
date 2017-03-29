using Geometry;

namespace CitySimulation
{
    public class Resident
    {
        public Resident(Position2f position)
        {
            Position = position;
        }

        public Position2f Position { get; }

        public Job Job { get; set; }

        public bool HasJob => Job != null;
    }
}
