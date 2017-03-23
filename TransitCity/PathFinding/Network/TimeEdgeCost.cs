using System;
using Utility.Units;

namespace PathFinding.Network
{
    public class TimeEdgeCost : IEstimateableEdgeCost
    {
        private readonly TimeSpan _cost; // seconds

        public TimeEdgeCost()
        {
            _cost = new TimeSpan(0);
        }

        public TimeEdgeCost(float seconds)
        {
            _cost = TimeSpan.FromSeconds(seconds);
        }

        public TimeEdgeCost(TimeSpan ts)
        {
            _cost = ts;
        }

        public TimeEdgeCost(Duration duration)
        {
            _cost = TimeSpan.FromSeconds(duration.Seconds);
        }

        public TimeSpan TimeSpan => _cost;

        public override string ToString()
        {
            return _cost.ToString(@"h\:mm\:ss\ \h");
        }

        public int CompareTo(object other)
        {
            if (!(other is TimeEdgeCost))
            {
                throw new ArgumentException();
            }

            return _cost.CompareTo(((TimeEdgeCost)other)._cost);
        }

        public bool Equals(IEdgeCost other)
        {
            if (!(other is TimeEdgeCost))
            {
                throw new ArgumentException();
            }

            return _cost.Equals(((TimeEdgeCost)other)._cost);
        }

        public IEdgeCost Add(IEdgeCost other)
        {
            if (!(other is TimeEdgeCost))
            {
                throw new ArgumentException();
            }

            return new TimeEdgeCost((float) (_cost + ((TimeEdgeCost)other)._cost).TotalSeconds);
        }

        public bool GreaterOrEquals(IEdgeCost other)
        {
            if (!(other is TimeEdgeCost))
            {
                throw new ArgumentException();
            }

            return _cost >= ((TimeEdgeCost)other)._cost;
        }
    }
}
