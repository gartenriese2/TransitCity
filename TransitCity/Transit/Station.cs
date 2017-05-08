using Geometry;

namespace Transit
{
    public class Station
    {
        public Station(Position2d position)
        {
            Position = position;
            EntryPosition = position;
            ExitPosition = position;
        }

        public Position2d Position { get; }

        public Position2d EntryPosition { get; }

        public Position2d ExitPosition { get; }

        public override string ToString()
        {
            return $"Station ({Position})";
        }
    }
}
