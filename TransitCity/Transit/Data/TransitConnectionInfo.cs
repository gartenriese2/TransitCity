using System;
using System.Collections.Generic;
using System.Linq;
using CitySimulation;
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

        private void Initialize()
        {
            var connectionsLists = _connectionsDictionary.SelectMany(p => p.Value);
        }
    }
}
