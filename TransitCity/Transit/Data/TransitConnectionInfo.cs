using System;
using System.Collections.Generic;
using System.Linq;
using CitySimulation;
using Geometry;
using Time;
using Transit.Timetable;
using Utility.Extensions;

namespace Transit.Data
{
    using ConnectionList = List<Connection>;

    public class TransitConnectionInfo
    {
        private readonly Dictionary<Resident, List<ConnectionList>> _connectionsDictionary;
        private readonly WeekTimeDictionary<Connection> _weekTimeConnectionsDictionary;
        private readonly Dictionary<Connection, Resident> _connectionToResidentDictionary = new Dictionary<Connection, Resident>();

        public TransitConnectionInfo(Dictionary<Resident, List<ConnectionList>> connectionsDictionary)
        {
            _connectionsDictionary = connectionsDictionary ?? throw new ArgumentNullException(nameof(connectionsDictionary));
            _weekTimeConnectionsDictionary = new WeekTimeDictionary<Connection>(WeekTimeDictionary<Connection>.Granularity.HalfHour, connectionsDictionary.Values.SelectMany(cll => cll).SelectMany(cl => cl));
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
            
            _weekTimeConnectionsDictionary.AddRange(connectionsDictionary.Values.SelectMany(cll => cll).SelectMany(cl => cl));
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
            return _weekTimeConnectionsDictionary[wtp].Where(c => new WeekTimeSpan(c.SourceTime, c.TargetTime).IsInside(wtp)).Select(c => (_connectionToResidentDictionary[c], c));
        }

        public IEnumerable<(Resident, Position2d, Vector2d)> GetActiveResidents(IEnumerable<(Resident, Connection)> activeConnections, WeekTimePoint wtp)
        {
            var walkingConnections = activeConnections.Where(c => c.Item2.Type == Connection.TypeEnum.WalkToStation || c.Item2.Type == Connection.TypeEnum.WalkFromStation || c.Item2.Type == Connection.TypeEnum.Transfer);
            var activeResidents = new List<(Resident, Position2d, Vector2d)>();
            foreach (var (r, wc) in walkingConnections)
            {
                var isTransfer = wc.Type == Connection.TypeEnum.Transfer;
                if (isTransfer)
                {
                    var vec = wc.TargetStation.EntryPosition - wc.SourceStation.ExitPosition;
                    var from = wc.SourceStation.ExitPosition;
                    var to = wc.TargetStation.EntryPosition;
                    var t = (wtp - wc.SourceTime).TotalMilliseconds / (wc.TargetTime - wc.SourceTime).TotalMilliseconds;
                    var pos = Position2d.Lerp(t, from, to);
                    activeResidents.Add((r, pos, vec));
                }
                else
                {
                    var toStation = wc.Type == Connection.TypeEnum.WalkToStation;
                    var vec = toStation ? wc.TargetStation.EntryPosition - wc.SourcePos : wc.TargetPos - wc.SourceStation.ExitPosition;
                    var from = toStation ? wc.SourcePos : wc.SourceStation.ExitPosition;
                    var to = toStation ? wc.TargetStation.EntryPosition : wc.TargetPos;
                    var t = (wtp - wc.SourceTime).TotalMilliseconds / (wc.TargetTime - wc.SourceTime).TotalMilliseconds;
                    var pos = Position2d.Lerp(t, from, to);
                    activeResidents.Add((r, pos, vec));
                }
            }

            return activeResidents;
        }
    }
}
