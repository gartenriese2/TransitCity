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
        private readonly Dictionary<WeekTimeSpan, ConnectionList> _hourlyConnectionsDictionary = new Dictionary<WeekTimeSpan, ConnectionList>(24 * 7);

        public TransitConnectionInfo(Dictionary<Resident, List<ConnectionList>> connectionsDictionary)
        {
            _connectionsDictionary = connectionsDictionary ?? throw new ArgumentNullException(nameof(connectionsDictionary));
            Initialize();
        }

        public void AddConnections(Dictionary<Resident, List<ConnectionList>> connectionsDictionary)
        {
            foreach (var (key, value) in connectionsDictionary)
            {
                _connectionsDictionary.Add(key, value);
            }
            
            AddToHourlyConnectionsDictionary(connectionsDictionary);
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
            return _hourlyConnectionsDictionary.First(pair => pair.Key.IsInside(wtp)).Value.Where(c => new WeekTimeSpan(c.SourceTime, c.TargetTime).IsInside(wtp));
        }

        private void Initialize()
        {
            for (var i = 0; i < 24 * 7; ++i)
            {
                var begin = new WeekTimePoint(TimeSpan.FromHours(i));
                var end = new WeekTimePoint(TimeSpan.FromHours((i + 1) % 168));
                _hourlyConnectionsDictionary.Add(new WeekTimeSpan(begin, end), new ConnectionList());
            }

            AddToHourlyConnectionsDictionary(_connectionsDictionary);
        }

        private void AddToHourlyConnectionsDictionary(Dictionary<Resident, List<ConnectionList>> connectionsDictionary)
        {
            var connections = connectionsDictionary.Values.SelectMany(cll => cll).SelectMany(cl => cl);
            foreach (var connection in connections)
            {
                var cwts = new WeekTimeSpan(connection.SourceTime, connection.TargetTime);
                foreach (var (wts, list) in _hourlyConnectionsDictionary)
                {
                    if (wts.Overlaps(cwts))
                    {
                        list.Add(connection);
                    }
                }
            }
        }
    }
}
