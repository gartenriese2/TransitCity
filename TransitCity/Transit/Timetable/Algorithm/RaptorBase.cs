using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Time;

namespace Transit.Timetable.Algorithm
{
    public class RaptorBase<TPos> : IRaptor<TPos> where TPos : IPosition
    {
        protected const int NumRounds = 5;
        protected readonly float _walkingSpeed;
        private readonly TimeSpan _maxWalkingTime;
        private readonly IReadOnlyCollection<TransferStation<TPos>> _transferStations;

        protected RaptorBase(float walkingSpeed, TimeSpan maxWalkingTime, IReadOnlyCollection<TransferStation<TPos>> transferStations)
        {
            _walkingSpeed = walkingSpeed;
            _maxWalkingTime = maxWalkingTime;
            _transferStations = transferStations;
        }

        public virtual List<Connection<TPos>> Compute(TPos startPos, WeekTimePoint startTime, TPos targetPos)
        {
            throw new NotImplementedException();
        }

        protected TransferStation<TPos> GetTransferStation(Station<TPos> station)
        {
            var collection = _transferStations.Where(ts => ts.Stations.Any(s => s == station)).ToList();
            if (collection.Count != 1)
            {
                throw new InvalidOperationException();
            }

            return collection[0];
        }

        protected (Dictionary<Station<TPos>, WeekTimePoint>, List<Connection<TPos>>) GetInitialMarkedStations(TPos startPos, WeekTimePoint startTime)
        {
            var markedStations = new Dictionary<Station<TPos>, WeekTimePoint>();
            var earliestConnections = new List<Connection<TPos>>();
            foreach (var enterStation in _transferStations.SelectMany(ts => ts.Stations))
            {
                var walkingTime = TimeSpan.FromSeconds(startPos.DistanceTo(enterStation.Position) / _walkingSpeed);
                if (walkingTime > _maxWalkingTime)
                {
                    continue;
                }

                var arrivalTime = startTime + walkingTime;
                markedStations.Add(enterStation, arrivalTime);
                earliestConnections.Add(Connection<TPos>.CreateWalkToStation(startPos, startTime, enterStation, arrivalTime));
            }

            return (markedStations, earliestConnections);
        }

        protected List<Connection<TPos>> GetTravelPath(IReadOnlyCollection<Connection<TPos>> earliestConnections, TPos targetPos)
        {
            var con = earliestConnections.FirstOrDefault(c => c.TargetPos != null && c.TargetPos.DistanceTo(targetPos) < float.Epsilon);

            var connectionList = new List<Connection<TPos>> { con };

            while (con.Type == Connection<TPos>.TypeEnum.Ride || con.Type == Connection<TPos>.TypeEnum.Transfer || con.Type == Connection<TPos>.TypeEnum.WalkFromStation)
            {
                con = earliestConnections.FirstOrDefault(c => c.TargetStation == con.SourceStation);
                connectionList.Insert(0, con);
            }

            return connectionList;
        }
    }
}
