using Geometry;

namespace Transit
{
    public class Station<P> where P : IPosition
    {
        public Station(P position)
        {
            Position = position;
            EntryPosition = position;
            ExitPosition = position;
        }

        public P Position { get; }

        public P EntryPosition { get; }

        public P ExitPosition { get; }

        public override string ToString()
        {
            return $"Station ({Position})";
        }
    }
}
