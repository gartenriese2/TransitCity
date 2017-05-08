using Geometry;

namespace CitySimulation
{
    public class Resident
    {
        public Resident(Position2d position)
        {
            Position = position;
        }

        public Position2d Position { get; }

        public Job Job { get; set; }

        public bool HasJob => Job != null;
    }
}
