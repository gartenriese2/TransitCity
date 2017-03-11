using System;

namespace Utility.Threading
{
    public class Atomic<T> where T : IAtomic<T>
    {
        private readonly T _data;

        private readonly object _lock = new object();

        public Atomic(T data)
        {
            _data = data;
        }

        public T Data()
        {
            return _data;
        }

        public void ReplaceOnCondition(Predicate<T> pred, T other)
        {
            lock (_lock)
            {
                if (pred(_data))
                {
                    _data.Replace(other);
                }
            }
        }

        public void ReplaceOnCondition(Predicate<T> pred, T other, Action action)
        {
            lock (_lock)
            {
                if (pred(_data))
                {
                    _data.Replace(other);
                    action();
                }
            }
        }
    }
}
