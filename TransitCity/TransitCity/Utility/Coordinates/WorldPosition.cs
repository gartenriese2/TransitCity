namespace TransitCity.Utility.Coordinates
{
    public class WorldPosition : Position
    {
        public WorldPosition(double x, double y)
            : base(x, y)
        {
        }

        public static double Size { get; set; } = 1.0;

        public ModelPosition ToModelPosition()
        {
            return new ModelPosition(X / Size, Y / Size);
        }
    }
}
