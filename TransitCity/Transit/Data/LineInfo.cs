using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Time;

namespace Transit.Data
{
    public class LineInfo
    {
        public LineInfo(Line line, List<RouteInfo> routeInfos)
        {
            Line = line ?? throw new ArgumentNullException(nameof(line));
            RouteInfos = routeInfos ?? throw new ArgumentNullException(nameof(routeInfos));
        }

        public Line Line { get; }

        public List<RouteInfo> RouteInfos { get; }

        public IEnumerable<Trip> GetActiveTrips(WeekTimePoint wtp) => RouteInfos.SelectMany(ri => ri.GetActiveTrips(wtp));

        public IEnumerable<Position2d> GetActiveVehiclePositions(WeekTimePoint wtp) => RouteInfos.SelectMany(ri => ri.GetActiveVehiclePositions(wtp));

        public IEnumerable<(Position2d, Vector2d)> GetActiveVehiclePositionsAndDirections(WeekTimePoint wtp) => RouteInfos.SelectMany(ri => ri.GetActiveVehiclePositionsAndDirections(wtp));
    }
}
