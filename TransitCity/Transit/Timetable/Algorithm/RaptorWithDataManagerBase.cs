using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Time;
using Transit.Data;
using Utility.Units;

namespace Transit.Timetable.Algorithm
{
    public class RaptorWithDataManagerBase : IRaptor
    {
        protected const int NumRounds = 5;
        protected readonly Speed _walkingSpeed;
        protected readonly TimeSpan _maxWalkingTime;
        protected readonly TimeSpan _maxWaitingTime;
        protected readonly DataManager _dataManager;

        protected RaptorWithDataManagerBase(Speed walkingSpeed, TimeSpan maxWalkingTime, TimeSpan maxWaitingTime, DataManager dataManager)
        {
            _walkingSpeed = walkingSpeed;
            _maxWalkingTime = maxWalkingTime;
            _maxWaitingTime = maxWaitingTime;
            _dataManager = dataManager;
        }

        public virtual List<Connection> Compute(Position2d sourcePos, WeekTimePoint startTime, Position2d targetPos)
        {
            throw new NotImplementedException();
        }

        public virtual List<Connection> ComputeReverse(Position2d sourcePos, WeekTimePoint latestArrivalTime, Position2d targetPos)
        {
            throw new NotImplementedException();
        }

        protected static List<Connection> GetTravelPath(IReadOnlyCollection<Connection> earliestConnections, IPosition targetPos)
        {
            var con = earliestConnections.FirstOrDefault(c => c.TargetPos != null && c.TargetPos.DistanceTo(targetPos) < float.Epsilon);

            var connectionList = new List<Connection> { con };

            while (con.Type == Connection.TypeEnum.Ride || con.Type == Connection.TypeEnum.Transfer || con.Type == Connection.TypeEnum.WalkFromStation)
            {
                con = earliestConnections.FirstOrDefault(c => c.TargetStation == con.SourceStation);
                connectionList.Insert(0, con);
                if (connectionList.Count > earliestConnections.Count)
                {
                    throw new InvalidOperationException();
                }
            }

            return connectionList;
        }

        protected static List<Connection> GetTravelPathReverse(IReadOnlyCollection<Connection> latestConnections, Position2d sourcePos)
        {
            var con = latestConnections.FirstOrDefault(c => c.SourcePos != null && c.SourcePos.EqualPosition(sourcePos));

            var connectionList = new List<Connection> { con };

            while (con.Type == Connection.TypeEnum.Ride || con.Type == Connection.TypeEnum.Transfer || con.Type == Connection.TypeEnum.WalkToStation)
            {
                var newCon = latestConnections.FirstOrDefault(c => c.SourceStation == con.TargetStation);
                if (newCon.Type == Connection.TypeEnum.Transfer || newCon.Type == Connection.TypeEnum.WalkFromStation)
                {
                    // change walking time
                    if (newCon.Type == Connection.TypeEnum.Transfer)
                    {
                        newCon = Connection.CreateTransfer(newCon.SourceStation, con.TargetTime, newCon.TargetStation, newCon.TargetTime - (newCon.SourceTime - con.TargetTime));
                    }
                    else
                    {
                        newCon = Connection.CreateWalkFromStation(newCon.SourceStation, con.TargetTime, newCon.TargetPos, newCon.TargetTime - (newCon.SourceTime - con.TargetTime));
                    }
                    
                }

                connectionList.Add(newCon);
                con = newCon;
                if (connectionList.Count > latestConnections.Count)
                {
                    throw new InvalidOperationException();
                }
            }

            return connectionList;
        }

        protected (Dictionary<StationInfo, WeekTimePoint>, List<Connection>) GetInitialMarkedStations(Position2d position, WeekTimePoint time)
        {
            var markedStations = new Dictionary<StationInfo, WeekTimePoint>();
            var connections = new List<Connection>();
            foreach (var stationInfo in _dataManager.AllStationInfos)
            {
                var walkingTime = TimeSpan.FromSeconds(position.DistanceTo(stationInfo.Station.Position) / _walkingSpeed.MetersPerSecond);
                if (walkingTime > _maxWalkingTime)
                {
                    continue;
                }

                var timeAtStation = time + walkingTime;
                markedStations.Add(stationInfo, timeAtStation);
                connections.Add(Connection.CreateWalkToStation(position, time, stationInfo.Station, timeAtStation));
            }

            return (markedStations, connections);
        }

        protected (Dictionary<StationInfo, WeekTimePoint>, List<Connection>) GetInitialMarkedStationsReverse(Position2d position, WeekTimePoint time)
        {
            var markedStations = new Dictionary<StationInfo, WeekTimePoint>();
            var connections = new List<Connection>();
            foreach (var stationInfo in _dataManager.AllStationInfos)
            {
                var walkingTime = TimeSpan.FromSeconds(position.DistanceTo(stationInfo.Station.Position) / _walkingSpeed.MetersPerSecond);
                if (walkingTime > _maxWalkingTime)
                {
                    continue;
                }

                var timeAtStation = time - walkingTime;
                markedStations.Add(stationInfo, timeAtStation);
                connections.Add(Connection.CreateWalkFromStation(stationInfo.Station, timeAtStation, position, time));
            }

            return (markedStations, connections);
        }
    }
}
