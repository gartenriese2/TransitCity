using System;
using System.Collections.Generic;
using System.Linq;
using MiscUtil;

namespace Statistics.Data
{
    public class NamedData<T> : Data where T : struct
    {
        public NamedData(IEnumerable<string> names)
        {
            Names = names;
        }

        public NamedData(IEnumerable<Enum> names)
        {
            Names = names.Select(n => n.ToString());
        }

        public IEnumerable<string> Names { get; }

        public T Maximum => DatapointCollection.Aggregate(Operator<T>.Zero, (value, dp) => Operator<T>.GreaterThan(((NamedDatapoint<T>)dp).Value, value) ? ((NamedDatapoint<T>)dp).Value : value);

        public void AddDatapoint(NamedDatapoint<T> dp)
        {
            if (Names.All(n => n != dp.Name))
            {
                throw new ArgumentException($"Invalid name: {dp.Name}");
            }

            base.AddDatapoint(dp);
        }
    }
}
