using System;
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

        protected (Dictionary<StationInfo, WeekTimePoint>, List<Connection>, Dictionary<Station, TimeSpan>) GetInitialMarkedStations(Position2d sourcePos, Position2d targetPos, WeekTimePoint time, Speed walkingSpeed)
        {
            var markedStations = new Dictionary<StationInfo, WeekTimePoint>();
            var connections = new List<Connection>();
            var exitTimeSpans = new Dictionary<Station, TimeSpan>();
            foreach (var stationInfo in _dataManager.AllStationInfos)
            {
                var walktingTimeFromExit = TimeSpan.FromSeconds(stationInfo.Station.ExitPosition.DistanceTo(targetPos) / walkingSpeed.MetersPerSecond);
                exitTimeSpans.Add(stationInfo.Station, walktingTimeFromExit);
                //if (walktingTimeFromExit.TotalMilliseconds / 2 > _maxWalkingTime.TotalMilliseconds) // Approximation
                //{
                //    continue;
                //}

                var walkingTimeToEntry = TimeSpan.FromSeconds(sourcePos.DistanceTo(stationInfo.Station.EntryPosition) / walkingSpeed.MetersPerSecond);
                if (walkingTimeToEntry > _maxWalkingTime)
                {
                    continue;
                }

                var timeAtStation = time + walkingTimeToEntry;
                markedStations.Add(stationInfo, timeAtStation);
                connections.Add(Connection.CreateWalkToStation(sourcePos, time, stationInfo.Station, timeAtStation));
            }

            return (markedStations, connections, exitTimeSpans);
        }

        protected (Dictionary<StationInfo, WeekTimePoint>, List<Connection>, Dictionary<Station, TimeSpan>) GetInitialMarkedStationsReverse(Position2d sourcePos, Position2d targetPos, WeekTimePoint time, Speed walkingSpeed)
        {
            var markedStations = new Dictionary<StationInfo, WeekTimePoint>();
            var connections = new List<Connection>();
            var enterTimeSpans = new Dictionary<Station, TimeSpan>();
            foreach (var stationInfo in _dataManager.AllStationInfos)
            {
                var walkingTimeToEntry = TimeSpan.FromSeconds(sourcePos.DistanceTo(stationInfo.Station.EntryPosition) / walkingSpeed.MetersPerSecond);
                enterTimeSpans.Add(stationInfo.Station, walkingTimeToEntry);
                //if (walkingTimeToEntry.TotalMilliseconds / 2 > _maxWalkingTime.TotalMilliseconds) // Approximation
                //{
                //    continue;
                //}

                var walkingTimeFromExit = TimeSpan.FromSeconds(stationInfo.Station.ExitPosition.DistanceTo(targetPos) / walkingSpeed.MetersPerSecond);
                if (walkingTimeFromExit > _maxWalkingTime)
                {
                    continue;
                }

                var timeAtStation = time - walkingTimeFromExit;
                markedStations.Add(stationInfo, timeAtStation);
                connections.Add(Connection.CreateWalkFromStation(stationInfo.Station, timeAtStation, targetPos, time));
            }

            return (markedStations, connections, enterTimeSpans);
        }
    }
}
