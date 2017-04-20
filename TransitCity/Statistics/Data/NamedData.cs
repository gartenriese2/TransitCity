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

        public T this[string key] => DatapointCollection.Where(dp => ((NamedDatapoint<T>) dp).Name == key).Aggregate(Operator<T>.Zero, (sum, dp) => Operator<T>.Add(sum, ((NamedDatapoint<T>)dp).Value));

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
