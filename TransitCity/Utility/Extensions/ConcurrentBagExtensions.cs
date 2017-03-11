using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Utility.Extensions
{
    public static class ConcurrentBagExtensions
    {
        public static void Replace<T>(this ConcurrentBag<T> bag, Predicate<T> pred, T connection, object lockObj)
        {
            lock (lockObj)
            {
                var list = bag.ToList();
                var idx = list.FindIndex(pred);
                list[idx] = connection;
                while (!bag.IsEmpty)
                {
                    bag.TryTake(out T c);
                }

                foreach (var c in list)
                {
                    bag.Add(c);
                }
            }
        }

        public static T Find<T>(this ConcurrentBag<T> bag, Predicate<T> pred, object lockObj)
        {
            lock (lockObj)
            {
                return bag.ToList().Find(pred);
            }
        }
    }
}
