using System.Collections.Generic;

namespace Table
{
    public class Table<TEntry>
    {
        private readonly List<TEntry> _table = new List<TEntry>();

        public IEnumerable<TEntry> All => _table;

        public void AddEntry(TEntry entry)
        {
            _table.Add(entry);
        }

        public IEnumerable<TEntry> Query(IQuery<TEntry> query)
        {
            return query.Execute(_table);
        }
    }
}
