using System.Collections.Generic;

namespace Table
{
    public class Table<TCollectionType, T> where TCollectionType : ICollection<T>, new()
    {
        private readonly TCollectionType _table = new TCollectionType();

        public IEnumerable<T> All => _table;

        public void AddEntry(T entry)
        {
            _table.Add(entry);
        }

        public IEnumerable<T> Query(IQuery<T> query)
        {
            return query.Execute(_table);
        }
    }
}
