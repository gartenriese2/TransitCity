using System.Collections.Generic;
using System.Linq;
using Geometry;
using Table;

namespace Transit.Timetable.Queries
{
    internal class LinkedEntryQuery<TPos> : IQuery<KeyValuePair<long, LinkedEntry<TPos>>> where TPos : IPosition
    {
        private readonly long _id;

        internal LinkedEntryQuery(LinkedEntry<TPos> entry)
        {
            _id = entry.Id;
        }

        public IEnumerable<KeyValuePair<long, LinkedEntry<TPos>>> Execute(IEnumerable<KeyValuePair<long, LinkedEntry<TPos>>> table)
        {
            var dic = table.ToDictionary(pair => pair.Key, pair => pair.Value);
            if (!dic.ContainsKey(_id))
            {
                return new List<KeyValuePair<long, LinkedEntry<TPos>>>();
            }

            return dic[_id].NextEntries.Select(id => new KeyValuePair<long, LinkedEntry<TPos>>(id, dic.ContainsKey(id) ? dic[id] : null));
        }
    }
}
