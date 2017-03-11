using System.Collections.Generic;
using Table;

namespace Transit.Timetable
{
    public interface ITimetable<TEntry>
    {
        void AddEntry(TEntry entry);

        IEnumerable<TEntry> Query(IQuery<TEntry> query);
    }
}
