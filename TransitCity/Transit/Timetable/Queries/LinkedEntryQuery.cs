using System.Collections.Generic;
using System.Linq;
using Geometry;
using Table;

namespace Transit.Timetable.Queries
{
    internal class LinkedEntryQuery : IQuery<KeyValuePair<long, LinkedEntry>>
    {
        private readonly long _id;

        internal LinkedEntryQuery(LinkedEntry entry)
        {
            _id = entry.Id;
        }

        public IEnumerable<KeyValuePair<long, LinkedEntry>> Execute(IEnumerable<KeyValuePair<long, LinkedEntry>> table)
        {
            var dic = table.ToDictionary(pair => pair.Key, pair => pair.Value);
            if (!dic.ContainsKey(_id))
            {
                return new List<KeyValuePair<long, LinkedEntry>>();
            }

            return dic[_id].NextEntries.Select(id => new KeyValuePair<long, LinkedEntry>(id, dic.ContainsKey(id) ? dic[id] : null));
        }
    }
}
