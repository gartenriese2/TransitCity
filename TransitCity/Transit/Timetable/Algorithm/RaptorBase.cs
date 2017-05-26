﻿using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Time;
using Transit.Data;
using Utility.Units;

namespace Transit.Timetable.Algorithm
{
    public class RaptorBase : IRaptor
    {
        protected const int NumRounds = 5;
        protected readonly TimeSpan _maxWalkingTime;
        protected readonly TimeSpan _maxWaitingTime;
        protected readonly DataManager _dataManager;

        protected RaptorBase(TimeSpan maxWalkingTime, TimeSpan maxWaitingTime, DataManager dataManager)
        {
            _maxWalkingTime = maxWalkingTime;
            _maxWaitingTime = maxWaitingTime;
            _dataManager = dataManager;
        }

        public virtual List<Connection> Compute(Position2d sourcePos, WeekTimePoint startTime, Position2d targetPos, Speed walkingSpeed)
        {
            throw new NotImplementedException();
        }

        public virtual List<Connection> ComputeReverse(Position2d sourcePos, WeekTimePoint latestArrivalTime, Position2d targetPos, Speed walkingSpeed)
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

        protected (Dictionary<StationInfo, WeekTimePoint>, List<Connection>, Dictionary<Station, TimeSpan>) GetInitialMarkedStations(Position2d position, WeekTimePoint time, Speed walkingSpeed)
        {
            var markedStations = new Dictionary<StationInfo, WeekTimePoint>();
            var connections = new List<Connection>();
            var exitTimeSpans = new Dictionary<Station, TimeSpan>();
            foreach (var stationInfo in _dataManager.AllStationInfos)
            {
                var walktingTimeFromExit = TimeSpan.FromSeconds(stationInfo.Station.ExitPosition.DistanceTo(position) / walkingSpeed.MetersPerSecond);
                exitTimeSpans.Add(stationInfo.Station, walktingTimeFromExit);
                if (walktingTimeFromExit.TotalMilliseconds / 2 > _maxWalkingTime.TotalMilliseconds) // Approximation
                {
                    continue;
                }

                var walkingTime = TimeSpan.FromSeconds(position.DistanceTo(stationInfo.Station.EntryPosition) / walkingSpeed.MetersPerSecond);
                if (walkingTime > _maxWalkingTime)
                {
                    continue;
                }

                var timeAtStation = time + walkingTime;
                markedStations.Add(stationInfo, timeAtStation);
                connections.Add(Connection.CreateWalkToStation(position, time, stationInfo.Station, timeAtStation));
            }

            return (markedStations, connections, exitTimeSpans);
        }

        protected (Dictionary<StationInfo, WeekTimePoint>, List<Connection>, Dictionary<Station, TimeSpan>) GetInitialMarkedStationsReverse(Position2d position, WeekTimePoint time, Speed walkingSpeed)
        {
            var markedStations = new Dictionary<StationInfo, WeekTimePoint>();
            var connections = new List<Connection>();
            var enterTimeSpans = new Dictionary<Station, TimeSpan>();
            foreach (var stationInfo in _dataManager.AllStationInfos)
            {
                var walktingTimeToEntry = TimeSpan.FromSeconds(stationInfo.Station.EntryPosition.DistanceTo(position) / walkingSpeed.MetersPerSecond);
                enterTimeSpans.Add(stationInfo.Station, walktingTimeToEntry);
                if (walktingTimeToEntry.TotalMilliseconds / 2 > _maxWalkingTime.TotalMilliseconds) // Approximation
                {
                    continue;
                }

                var walkingTime = TimeSpan.FromSeconds(position.DistanceTo(stationInfo.Station.Position) / walkingSpeed.MetersPerSecond);
                if (walkingTime > _maxWalkingTime)
                {
                    continue;
                }

                var timeAtStation = time - walkingTime;
                markedStations.Add(stationInfo, timeAtStation);
                connections.Add(Connection.CreateWalkFromStation(stationInfo.Station, timeAtStation, position, time));
            }

            return (markedStations, connections, enterTimeSpans);
        }
    }
}
