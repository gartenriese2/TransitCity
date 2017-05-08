using System.ComponentModel;
using Geometry;
using Time;

namespace Transit.Timetable
{
    public class Connection
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

        public Position2d SourcePos { get; private set; }

        public Station SourceStation { get; private set; }

        public WeekTimePoint SourceTime { get; private set; }

        public Position2d TargetPos { get; private set; }

        public Station TargetStation { get; private set; }

        public WeekTimePoint TargetTime { get; private set; }

        public Line Line { get; private set; }

        public static Connection CreateWalkToStation(Position2d sourcePos, WeekTimePoint sourceTime, Station targetStation, WeekTimePoint targetTime)
        {
            return new Connection
            {
                Type = TypeEnum.WalkToStation,
                SourcePos = sourcePos,
                SourceTime = sourceTime,
                TargetStation = targetStation,
                TargetTime = targetTime
            };
        }

        public static Connection CreateWalkFromStation(Station sourceStation, WeekTimePoint sourceTime, Position2d targetPos, WeekTimePoint targetTime)
        {
            return new Connection
            {
                Type = TypeEnum.WalkFromStation,
                SourceStation = sourceStation,
                SourceTime = sourceTime,
                TargetPos = targetPos,
                TargetTime = targetTime
            };
        }

        public static Connection CreateTransfer(Station sourceStation, WeekTimePoint sourceTime, Station targetStation, WeekTimePoint targetTime)
        {
            return new Connection
            {
                Type = TypeEnum.Transfer,
                SourceStation = sourceStation,
                SourceTime = sourceTime,
                TargetStation = targetStation,
                TargetTime = targetTime
            };
        }

        public static Connection CreateRide(Station sourceStation, WeekTimePoint sourceTime, Station targetStation, WeekTimePoint targetTime, Line line)
        {
            return new Connection
            {
                Type = TypeEnum.Ride,
                SourceStation = sourceStation,
                SourceTime = sourceTime,
                TargetStation = targetStation,
                TargetTime = targetTime,
                Line = line
            };
        }

        public static Connection CreateWalk(Position2d sourcePos, WeekTimePoint sourceTime, Position2d targetPos, WeekTimePoint targetTime)
        {
            return new Connection
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
