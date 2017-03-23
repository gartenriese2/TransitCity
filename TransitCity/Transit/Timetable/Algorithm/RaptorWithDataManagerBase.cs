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

        public virtual List<Connection<Position2f>> Compute(Position2f startPos, WeekTimePoint startTime, Position2f targetPos)
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

        protected (Dictionary<StationInfo, WeekTimePoint>, List<Connection2f>) GetInitialMarkedStations(Position2f startPos, WeekTimePoint startTime)
        {
            var markedStations = new Dictionary<StationInfo, WeekTimePoint>();
            var earliestConnections = new List<Connection2f>();
            foreach (var stationInfo in _dataManager.AllStationInfos)
            {
                var walkingTime = TimeSpan.FromSeconds(startPos.DistanceTo(stationInfo.Station.Position) / _walkingSpeed);
                if (walkingTime > _maxWalkingTime)
                {
                    continue;
                }

                var arrivalTime = startTime + walkingTime;
                markedStations.Add(stationInfo, arrivalTime);
                earliestConnections.Add(Connection2f.CreateWalkToStation(startPos, startTime, stationInfo.Station, arrivalTime));
            }

            return (markedStations, earliestConnections);
        }
    }
}
