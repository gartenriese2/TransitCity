using System;
using System.Collections.Generic;
using System.Linq;
using CitySimulation;
using Time;
using Transit.Timetable;
using Utility.Extensions;

namespace Transit.Data
{
    using ConnectionList = List<Connection>;

    public class TransitConnectionInfo
    {
        private readonly Dictionary<Resident, List<ConnectionList>> _connectionsDictionary;
        private readonly HourlyDictionary<Connection> _hourlyConnectionsDictionary;
        private readonly Dictionary<Connection, Resident> _connectionToResidentDictionary = new Dictionary<Connection, Resident>();

        public TransitConnectionInfo(Dictionary<Resident, List<ConnectionList>> connectionsDictionary)
        {
            _connectionsDictionary = connectionsDictionary ?? throw new ArgumentNullException(nameof(connectionsDictionary));
            _hourlyConnectionsDictionary = new HourlyDictionary<Connection>(HourlyDictionary<Connection>.Granularity.Hour, connectionsDictionary.Values.SelectMany(cll => cll).SelectMany(cl => cl));
            foreach (var (resident, connectionLists) in connectionsDictionary)
            {
                foreach (var connection in connectionLists.SelectMany(cl => cl))
                {
                    _connectionToResidentDictionary.Add(connection, resident);
                }
            }
        }

        public void AddConnections(Dictionary<Resident, List<ConnectionList>> connectionsDictionary)
        {
            foreach (var (key, value) in connectionsDictionary)
            {
                _connectionsDictionary.Add(key, value);
            }
            
            _hourlyConnectionsDictionary.AddRange(connectionsDictionary.Values.SelectMany(cll => cll).SelectMany(cl => cl));
            foreach (var (resident, connectionLists) in connectionsDictionary)
            {
                foreach (var connection in connectionLists.SelectMany(cl => cl))
                {
                    _connectionToResidentDictionary.Add(connection, resident);
                }
            }
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

        public IEnumerable<(Resident, Connection)> GetActiveConnections(WeekTimePoint wtp)
        {
            return _hourlyConnectionsDictionary[wtp].Where(c => new WeekTimeSpan(c.SourceTime, c.TargetTime).IsInside(wtp)).Select(c => (_connectionToResidentDictionary[c], c));
        }
    }
}
