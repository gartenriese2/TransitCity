namespace Statistics.Data
{
    public class FloatDatapoint : IDatapoint
    {
        public FloatDatapoint(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X { get; }

        public float Y { get; }
    }
}
