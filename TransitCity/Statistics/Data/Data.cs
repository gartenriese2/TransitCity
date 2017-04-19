namespace Statistics.Data
{
    public class Data
    {
        protected readonly DatapointCollection DatapointCollection = new DatapointCollection();

        protected void AddDatapoint(Datapoint dp)
        {
            DatapointCollection.Add(dp);
        }
    }
}
