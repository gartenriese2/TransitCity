using System.Collections.Generic;

namespace Table
{
    public interface IQuery<T>
    {
        IEnumerable<T> Execute(IEnumerable<T> table);
    }
}
