namespace TransitCity.Utility.Coordinates
{
    using System;

    public class Position
    {
        protected static readonly Random Rnd = new Random();

        protected Position(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }

        public double Y { get; set; }

        public override string ToString()
        {
            return $"Position: ({X} | {Y})";
        }

        public double GetDistanceTo(Position other)
        {
            return Math.Sqrt(Math.Pow(X - other.X, 2.0) + Math.Pow(Y - other.Y, 2.0)); 
        }
    }
}
