namespace Statistics.Data
{
    public class Data
    {
        protected readonly DatapointCollection DatapointCollection = new DatapointCollection();

        protected void AddDatapoint(IDatapoint dp)
        {
            DatapointCollection.Add(dp);
        }
    }
}
