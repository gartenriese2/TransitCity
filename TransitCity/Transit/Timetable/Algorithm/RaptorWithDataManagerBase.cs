using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Time;
using Transit.Data;

namespace Transit.Timetable.Algorithm
{
    using Connection2f = Connection<Position2f>;

    public class RaptorWithDataManagerBase : IRaptor<Position2f>
    {
        protected const int NumRounds = 5;
        protected readonly float _walkingSpeed;
        protected readonly TimeSpan _maxWalkingTime;
        protected readonly TimeSpan _maxWaitingTime;
        protected readonly DataManager _dataManager;

        protected RaptorWithDataManagerBase(float walkingSpeed, TimeSpan maxWalkingTime, TimeSpan maxWaitingTime, DataManager dataManager)
        {
            _walkingSpeed = walkingSpeed;
            _maxWalkingTime = maxWalkingTime;
            _maxWaitingTime = maxWaitingTime;
            _dataManager = dataManager;
        }

        public virtual List<Connection<Position2f>> Compute(Position2f sourcePos, WeekTimePoint startTime, Position2f targetPos)
        {
            throw new NotImplementedException();
        }

        public virtual List<Connection2f> ComputeReverse(Position2f sourcePos, WeekTimePoint latestArrivalTime, Position2f targetPos)
        {
            throw new NotImplementedException();
        }

        protected static List<Connection2f> GetTravelPath(IReadOnlyCollection<Connection2f> earliestConnections, IPosition targetPos)
        {
            var con = earliestConnections.FirstOrDefault(c => c.TargetPos != null && c.TargetPos.DistanceTo(targetPos) < float.Epsilon);

            var connectionList = new List<Connection2f> { con };

            while (con.Type == Connection2f.TypeEnum.Ride || con.Type == Connection2f.TypeEnum.Transfer || con.Type == Connection2f.TypeEnum.WalkFromStation)
            {
                con = earliestConnections.FirstOrDefault(c => c.TargetStation == con.SourceStation);
                connectionList.Insert(0, con);
            }

            return connectionList;
        }

        protected static List<Connection2f> GetTravelPathReverse(IReadOnlyCollection<Connection2f> latestConnections, Position2f sourcePos)
        {
            var con = latestConnections.FirstOrDefault(c => c.SourcePos != null && c.SourcePos.EqualPosition(sourcePos));

            var connectionList = new List<Connection2f> { con };

            while (con.Type == Connection2f.TypeEnum.Ride || con.Type == Connection2f.TypeEnum.Transfer || con.Type == Connection2f.TypeEnum.WalkToStation)
            {
                var newCon = latestConnections.FirstOrDefault(c => c.SourceStation == con.TargetStation);
                if (newCon.Type == Connection2f.TypeEnum.Transfer || newCon.Type == Connection2f.TypeEnum.WalkFromStation)
                {
                    // change walking time
                    if (newCon.Type == Connection2f.TypeEnum.Transfer)
                    {
                        newCon = Connection2f.CreateTransfer(newCon.SourceStation, con.TargetTime, newCon.TargetStation, newCon.TargetTime - (newCon.SourceTime - con.TargetTime));
                    }
                    else
                    {
                        newCon = Connection2f.CreateWalkFromStation(newCon.SourceStation, con.TargetTime, newCon.TargetPos, newCon.TargetTime - (newCon.SourceTime - con.TargetTime));
                    }
                    
                }

                connectionList.Add(newCon);
                con = newCon;
            }

            return connectionList;
        }

        protected (Dictionary<StationInfo, WeekTimePoint>, List<Connection2f>) GetInitialMarkedStations(Position2f position, WeekTimePoint time)
        {
            var markedStations = new Dictionary<StationInfo, WeekTimePoint>();
            var connections = new List<Connection2f>();
            foreach (var stationInfo in _dataManager.AllStationInfos)
            {
                var walkingTime = TimeSpan.FromSeconds(position.DistanceTo(stationInfo.Station.Position) / _walkingSpeed);
                if (walkingTime > _maxWalkingTime)
                {
                    continue;
                }

                var timeAtStation = time + walkingTime;
                markedStations.Add(stationInfo, timeAtStation);
                connections.Add(Connection2f.CreateWalkToStation(position, time, stationInfo.Station, timeAtStation));
            }

            return (markedStations, connections);
        }

        protected (Dictionary<StationInfo, WeekTimePoint>, List<Connection2f>) GetInitialMarkedStationsReverse(Position2f position, WeekTimePoint time)
        {
            var markedStations = new Dictionary<StationInfo, WeekTimePoint>();
            var connections = new List<Connection2f>();
            foreach (var stationInfo in _dataManager.AllStationInfos)
            {
                var walkingTime = TimeSpan.FromSeconds(position.DistanceTo(stationInfo.Station.Position) / _walkingSpeed);
                if (walkingTime > _maxWalkingTime)
                {
                    continue;
                }

                var timeAtStation = time - walkingTime;
                markedStations.Add(stationInfo, timeAtStation);
                connections.Add(Connection2f.CreateWalkFromStation(stationInfo.Station, timeAtStation, position, time));
            }

            return (markedStations, connections);
        }
    }
}
