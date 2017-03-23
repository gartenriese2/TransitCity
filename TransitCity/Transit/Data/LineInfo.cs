using System;
using System.Collections.Generic;
using Geometry;

namespace Transit.Data
{
    public class LineInfo
    {
        public LineInfo(Line<Position2f> line, List<RouteInfo> routeInfos)
        {
            Line = line ?? throw new ArgumentNullException(nameof(line));
            RouteInfos = routeInfos ?? throw new ArgumentNullException(nameof(routeInfos));
        }

        public Line<Position2f> Line { get; }

        public List<RouteInfo> RouteInfos { get; }
    }
}
