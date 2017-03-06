namespace TransitCity.Utility.Coordinates
{
    public class ViewPosition : Position
    {
        public ViewPosition(double x, double y)
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
