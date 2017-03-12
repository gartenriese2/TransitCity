using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;

namespace Transit.Data
{
    public class DataManager
    {
        private readonly Dictionary<Station<Position2f>, TransferStation<Position2f>> _stationToTransferStationDictionary = new Dictionary<Station<Position2f>, TransferStation<Position2f>>();

        private readonly Dictionary<string, TransferStation<Position2f>> _transferStationDictionary = new Dictionary<string, TransferStation<Position2f>>();

        private readonly Dictionary<Route<Position2f>, Path<Position2f>> _routePathDictionary = new Dictionary<Route<Position2f>, Path<Position2f>>();

        private readonly List<Line<Position2f>> _subwayLines = new List<Line<Position2f>>();

        public void AddSubwayLine(Dictionary<Position2f, string> route, float frequency, string name)
        {
            var ((outwardRun, p1), (returnRun, p2)) = CreateSubwayRoutes(route, frequency, _transferStationDictionary);
            _routePathDictionary[outwardRun] = p1;
            _routePathDictionary[returnRun] = p2;
            _subwayLines.Add(new Line<Position2f>(name, outwardRun, returnRun));
        }

        public TransferStation<Position2f> GetTransferStation(Station<Position2f> station)
        {
            if (station == null || !_stationToTransferStationDictionary.ContainsKey(station))
            {
                throw new ArgumentException();
            }

            return _stationToTransferStationDictionary[station];
        }

        private ((Route<Position2f>, Path<Position2f>), (Route<Position2f>, Path<Position2f>)) CreateSubwayRoutes(Dictionary<Position2f, string> dic, float frequency, IDictionary<string, TransferStation<Position2f>> tsd)
        {
            var positions = dic.Keys.ToList();
            var ccrSpline = new CentripetalCatmullRomSpline(positions);
            var spline = ccrSpline.CalculateSegments(10);

            var routeAList = new List<Station<Position2f>>();
            var routeBList = new List<Station<Position2f>>();
            foreach (var b in positions)
            {
                var idx = spline.FindIndex(p => p == b);
                var a = idx == 0 ? b : spline[idx - 1];
                var c = idx == spline.Count - 1 ? b : spline[idx + 1];
                var (p1, p2) = b.GetOffsetPoints(c - a, 4f);

                var stationA = CreateStation(p1, dic[b], tsd);
                var stationB = CreateStation(p2, dic[b], tsd);
                routeAList.Add(stationA);
                routeBList.Add(stationB);
            }

            var routeA = new Route<Position2f>(routeAList, frequency);
            routeBList.Reverse();
            var routeB = new Route<Position2f>(routeBList, frequency);

            var offsetSplines = Path<Position2f>.GetOffsetPaths(spline, 4f);

            return ((routeA, offsetSplines.Item1), (routeB, offsetSplines.Item2));
        }

        private Station<Position2f> CreateStation(Position2f pos, string name, IDictionary<string, TransferStation<Position2f>> tsd)
        {
            var station = new Station<Position2f>(pos);
            if (!tsd.ContainsKey(name))
            {
                tsd[name] = new TransferStation<Position2f>(name);
            }

            tsd[name].AddStation(station);
            _stationToTransferStationDictionary[station] = tsd[name];
            return station;
        }
    }
}
