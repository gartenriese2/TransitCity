using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitySimulation;
using Time;
using Transit.Timetable;

namespace Transit.Data
{
    using ConnectionList = List<Connection>;

    public class TransitConnectionInfo
    {
        private readonly Dictionary<Resident, List<ConnectionList>> _connectionsDictionary;

        public TransitConnectionInfo(Dictionary<Resident, List<ConnectionList>> connectionsDictionary)
        {
            _connectionsDictionary = connectionsDictionary ?? throw new ArgumentNullException(nameof(connectionsDictionary));
            Initialize();
        }

        public float GetPercentagOfConnectionsWithTransit()
        {
            var connectionsLists = _connectionsDictionary.SelectMany(p => p.Value).ToList();
            return connectionsLists.Count(c => c.Count > 1) * 100f / connectionsLists.Count ;
        }

        public float GetPercentageOfWorkersUsingTransitAtLeastOnce()
        {
            return _connectionsDictionary.Select(d => d.Value).Count(l => l.Any(c => c.Count > 1)) * 100f / _connectionsDictionary.Count;
        }

        public float GetPercentageOfWorkersUsingTransitOnly()
        {
            return _connectionsDictionary.Select(d => d.Value).Count(l => l.All(c => c.Count > 1)) * 100f / _connectionsDictionary.Count;
        }

        public IEnumerable<Connection> GetActiveConnections(WeekTimePoint wtp)
        {
            return _connectionsDictionary.Values.SelectMany(cll => cll)
                                                .SelectMany(cl => cl)
                                                .Where(c => new WeekTimeSpan(c.SourceTime, c.TargetTime).IsInside(wtp));
        }

        public IEnumerable<Connection> GetActiveConnectionsThreaded(WeekTimePoint wtp)
        {
            var numThreads = Environment.ProcessorCount;
            if (numThreads <= 1)
            {
                return GetActiveConnections(wtp);
            }

            var taskList = new List<Task<List<Connection>>>(numThreads);
            var connections = _connectionsDictionary.Values.SelectMany(cll => cll).SelectMany(cl => cl);
            var i = 0;
            var groups = connections.GroupBy(x => i++ % numThreads).ToList();
            taskList.AddRange(groups.Select(g => Task.Factory.StartNew(() => g.Where(c => new WeekTimeSpan(c.SourceTime, c.TargetTime).IsInside(wtp)).ToList())));
            Task.WaitAll(taskList.ToArray());
            return taskList.Select(t => t.Result).SelectMany(r => r);
        }

        private void Initialize()
        {
            var connectionsLists = _connectionsDictionary.SelectMany(p => p.Value);
        }
    }
}
