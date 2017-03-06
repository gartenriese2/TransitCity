using System;
using System.Globalization;

namespace PathFinding.Network
{
    internal class BasicEdgeCost : IEstimateableEdgeCost
    {
        private readonly float _cost;

        public BasicEdgeCost()
        {
            _cost = 0f;
        }

        internal BasicEdgeCost(float cost)
        {
            _cost = cost;
        }

        public override string ToString()
        {
            return $"Cost: {_cost.ToString(CultureInfo.InvariantCulture)}";
        }

        public int CompareTo(object other)
        {
            if (!(other is BasicEdgeCost))
            {
                throw new ArgumentException();
            }

            return _cost.CompareTo(((BasicEdgeCost) other)._cost);
        }

        public bool Equals(IEdgeCost other)
        {
            if (!(other is BasicEdgeCost))
            {
                throw new ArgumentException();
            }

            return _cost.Equals(((BasicEdgeCost) other)._cost);
        }

        public IEdgeCost Add(IEdgeCost other)
        {
            if (!(other is BasicEdgeCost))
            {
                throw new ArgumentException();
            }

            return new BasicEdgeCost(_cost + ((BasicEdgeCost)other)._cost);
        }

        public bool GreaterOrEquals(IEdgeCost other)
        {
            if (!(other is BasicEdgeCost))
            {
                throw new ArgumentException();
            }

            return _cost >= ((BasicEdgeCost) other)._cost;
        }
    }
}
