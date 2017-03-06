namespace TransitCity.Utility.Coordinates
{
    using System;

    public class ModelPosition : Position
    {
        public ModelPosition(double x, double y)
            : base(x, y)
        {
        }

        public static ModelPosition CreateRandom()
        {
            return new ModelPosition(Rnd.NextDouble(), Rnd.NextDouble());
        }

        public static ModelPosition CreateRandomBetween(ModelPosition lowerleft, ModelPosition upperright)
        {
            if (lowerleft == null || upperright == null)
            {
                throw new ArgumentNullException();
            }

            if (lowerleft.X > upperright.X || lowerleft.Y > upperright.Y)
            {
                throw new ArgumentException("lowerleft must be smaller than upperright");
            }

            var dX = Math.Abs(lowerleft.X - upperright.X);
            var dY = Math.Abs(lowerleft.Y - upperright.Y);
            var minX = Math.Min(lowerleft.X, upperright.X);
            var minY = Math.Min(lowerleft.Y, upperright.Y);
            return new ModelPosition(Rnd.NextDouble() * dX + minX, Rnd.NextDouble() * dY + minY);
        }

        public ViewPosition ToViewPosition()
        {
            return new ViewPosition(X * ViewPosition.Size, Y * ViewPosition.Size);
        }

        public WorldPosition ToWorldPosition()
        {
            return new WorldPosition(X * WorldPosition.Size, Y * WorldPosition.Size);
        }
    }
}
