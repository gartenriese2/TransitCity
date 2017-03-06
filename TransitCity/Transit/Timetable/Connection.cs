using System.ComponentModel;
using Geometry;
using Time;

namespace Transit.Timetable
{
    public class Connection<TPos> where TPos : IPosition
    {
        private Connection() { }

        public enum TypeEnum : byte
        {
            Undefined,
            Ride,
            Walk,
            Transfer,
            WalkToStation,
            WalkFromStation
        }

        public TypeEnum Type { get; private set; } = TypeEnum.Undefined;

        public TPos SourcePos { get; private set; }

        public Station<TPos> SourceStation { get; private set; }

        public WeekTimePoint SourceTime { get; private set; }

        public TPos TargetPos { get; private set; }

        public Station<TPos> TargetStation { get; private set; }

        public WeekTimePoint TargetTime { get; private set; }

        public Line<TPos> Line { get; private set; }

        public static Connection<TPos> CreateWalkToStation(TPos sourcePos, WeekTimePoint sourceTime, Station<TPos> targetStation, WeekTimePoint targetTime)
        {
            return new Connection<TPos>
            {
                Type = TypeEnum.WalkToStation,
                SourcePos = sourcePos,
                SourceTime = sourceTime,
                TargetStation = targetStation,
                TargetTime = targetTime
            };
        }

        public static Connection<TPos> CreateWalkFromStation(Station<TPos> sourceStation, WeekTimePoint sourceTime, TPos targetPos, WeekTimePoint targetTime)
        {
            return new Connection<TPos>
            {
                Type = TypeEnum.WalkFromStation,
                SourceStation = sourceStation,
                SourceTime = sourceTime,
                TargetPos = targetPos,
                TargetTime = targetTime
            };
        }

        public static Connection<TPos> CreateTransfer(Station<TPos> sourceStation, WeekTimePoint sourceTime, Station<TPos> targetStation, WeekTimePoint targetTime)
        {
            return new Connection<TPos>
            {
                Type = TypeEnum.Transfer,
                SourceStation = sourceStation,
                SourceTime = sourceTime,
                TargetStation = targetStation,
                TargetTime = targetTime
            };
        }

        public static Connection<TPos> CreateRide(Station<TPos> sourceStation, WeekTimePoint sourceTime, Station<TPos> targetStation, WeekTimePoint targetTime, Line<TPos> line)
        {
            return new Connection<TPos>
            {
                Type = TypeEnum.Ride,
                SourceStation = sourceStation,
                SourceTime = sourceTime,
                TargetStation = targetStation,
                TargetTime = targetTime,
                Line = line
            };
        }

        public static Connection<TPos> CreateWalk(TPos sourcePos, WeekTimePoint sourceTime, TPos targetPos, WeekTimePoint targetTime)
        {
            return new Connection<TPos>
            {
                Type = TypeEnum.Walk,
                SourcePos = sourcePos,
                SourceTime = sourceTime,
                TargetPos = targetPos,
                TargetTime = targetTime
            };
        }

        public override string ToString()
        {
            switch (Type)
            {
                case TypeEnum.Undefined:
                    return "Undefined";
                case TypeEnum.Ride:
                    return $"Ride with line {Line} from {SourceStation} at {SourceTime} to {TargetStation} at {TargetTime}";
                case TypeEnum.Walk:
                    return $"Walk from {SourcePos} at {SourceTime} to {TargetPos} at {TargetTime}";
                case TypeEnum.WalkToStation:
                    return $"Walk from {SourcePos} at {SourceTime} to {TargetStation} at {TargetTime}";
                case TypeEnum.WalkFromStation:
                    return $"Walk from {SourceStation} at {SourceTime} to {TargetPos} at {TargetTime}";
                case TypeEnum.Transfer:
                    return $"Transfer from {SourceStation} at {SourceTime} to {TargetStation} at {TargetTime}";
                default:
                    throw new InvalidEnumArgumentException(nameof(Type), (int)Type, typeof(TypeEnum));
            }
        }
    }
}
