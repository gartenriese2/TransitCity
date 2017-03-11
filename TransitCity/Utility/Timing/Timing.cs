using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Utility.Timing
{
    public class Timing
    {
        public static (List<T> results, TimeSpan timespan) Profile<T>(Func<T> func, uint repitions = 1)
        {
            if (repitions == 0)
            {
                return (null, TimeSpan.Zero);
            }

            var list = new List<T>();

            //warmup
            var warmupTask = Task.Factory.StartNew(func);
            warmupTask.Wait();
            list.Add(warmupTask.Result);

            var sw = Stopwatch.StartNew();
            for (var i = 0; i < repitions; ++i)
            {
                var task = Task.Factory.StartNew(func);
                task.Wait();
                list.Add(task.Result);
            }
            sw.Stop();

            return (list, TimeSpan.FromTicks(sw.ElapsedTicks / repitions));
        }
    }
}
