﻿using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Time;
using Utility.Units;

namespace Transit.Data
{
    public class DataManager
    {
        private readonly Dictionary<Station<Position2f>, TransferStation<Position2f>> _stationToTransferStationDictionary = new Dictionary<Station<Position2f>, TransferStation<Position2f>>();

        private readonly Dictionary<string, TransferStation<Position2f>> _transferStationDictionary = new Dictionary<string, TransferStation<Position2f>>();

        private readonly Dictionary<Station<Position2f>, (LineInfo, RouteInfo, StationInfo)> _stationToRouteInfoDictionary = new Dictionary<Station<Position2f>, (LineInfo, RouteInfo, StationInfo)>();

        private readonly List<Line<Position2f>> _lines = new List<Line<Position2f>>();

        private readonly List<LineInfo> _lineInfos = new List<LineInfo>();

        public IEnumerable<Station<Position2f>> AllStations => _stationToTransferStationDictionary.Keys;

        public IEnumerable<TransferStation<Position2f>> AllTransferStations => _transferStationDictionary.Values;

        public IEnumerable<StationInfo> AllStationInfos => _lineInfos.SelectMany(li => li.RouteInfos).SelectMany(ri => ri.StationInfos);

        public IEnumerable<LineInfo> AllLineInfos => _lineInfos;

        public IEnumerable<Trip<Position2f>> GetActiveTrips(WeekTimePoint wtp) => AllLineInfos.SelectMany(li => li.GetActiveTrips(wtp));

        public IEnumerable<Position2f> GetActiveVehiclePositions(WeekTimePoint wtp) => AllLineInfos.SelectMany(li => li.GetActiveVehiclePositions(wtp));

        public IEnumerable<(Position2f, Vector2f)> GetActiveVehiclePositionsAndDirections(WeekTimePoint wtp) => AllLineInfos.SelectMany(li => li.GetActiveVehiclePositionsAndDirections(wtp));

        public void AddSubwayLine(Dictionary<Position2f, string> route, string name, WeekTimeCollection initialOutwardDepartures, WeekTimeCollection initialInwardDepartures, Duration waitingTime)
        {
            var numStations = route.Count;
            var ((outwardRun, outwardPath), (inwardRun, inwardPath)) = CreateSubwayRoutes(route, _transferStationDictionary);

            var (outwardArrivals, outwardDepartures, outwardTripList) = CreateRouteArrivalsAndDepartures(outwardRun, outwardPath, initialOutwardDepartures, waitingTime, SubwayTravelTimeFunc);
            var (inwardArrivals, inwardDepartures, inwardTripList) = CreateRouteArrivalsAndDepartures(inwardRun, inwardPath, initialInwardDepartures, waitingTime, SubwayTravelTimeFunc);

            outwardTripList = outwardTripList.OrderBy(trip => trip.DepartureAtStation(trip.Stations.First())).ToList();
            inwardTripList = inwardTripList.OrderBy(trip => trip.DepartureAtStation(trip.Stations.First())).ToList();

            var outwardStationInfos = new List<StationInfo>();
            var inwardStationInfos = new List<StationInfo>();
            for (var i = 0; i < numStations; ++i)
            {
                var outwardStation = outwardRun.Stations.ElementAt(i);
                var inwardStation = inwardRun.Stations.ElementAt(i);
                var outwardArrivalCollection = i > 0 ? outwardArrivals[i - 1] : new WeekTimeCollection();
                var inwardArrivalCollection = i > 0 ? inwardArrivals[i - 1] : new WeekTimeCollection();
                var outwardDepartureCollection = i < numStations - 1 ? outwardDepartures[i] : new WeekTimeCollection();
                var inwardDepartureCollection = i < numStations - 1 ? inwardDepartures[i] : new WeekTimeCollection();
                var outwardStationInfo = new StationInfo(outwardStation, outwardArrivalCollection, outwardDepartureCollection, outwardTripList);
                var inwardStationInfo = new StationInfo(inwardStation, inwardArrivalCollection, inwardDepartureCollection, inwardTripList);
                outwardStationInfos.Add(outwardStationInfo);
                inwardStationInfos.Add(inwardStationInfo);
            }

            var line = new Line<Position2f>(name, outwardRun, inwardRun);
            var routeInfoOutward = new RouteInfo(outwardRun, outwardPath, outwardStationInfos, outwardTripList);
            var routeInfoInward = new RouteInfo(inwardRun, inwardPath, inwardStationInfos, inwardTripList);

            var lineInfo = new LineInfo(line, new List<RouteInfo>{routeInfoOutward, routeInfoInward});
            _lineInfos.Add(lineInfo);
            _lines.Add(line);
            foreach (var stationInfo in routeInfoOutward.StationInfos)
            {
                _stationToRouteInfoDictionary.Add(stationInfo.Station, (lineInfo, routeInfoOutward, stationInfo));
            }

            foreach (var stationInfo in routeInfoInward.StationInfos)
            {
                _stationToRouteInfoDictionary.Add(stationInfo.Station, (lineInfo, routeInfoInward, stationInfo));
            }
        }

        public TransferStation<Position2f> GetTransferStation(Station<Position2f> station)
        {
            if (station == null || !_stationToTransferStationDictionary.ContainsKey(station))
            {
                throw new ArgumentException();
            }

            return _stationToTransferStationDictionary[station];
        }

        public (LineInfo lineInfo, RouteInfo routeInfo, StationInfo stationInfo) GetInfos(Station<Position2f> station)
        {
            if (station == null || !_stationToRouteInfoDictionary.ContainsKey(station))
            {
                throw new ArgumentException();
            }

            return _stationToRouteInfoDictionary[station];
        }

        private ((Route<Position2f>, Path), (Route<Position2f>, Path)) CreateSubwayRoutes(Dictionary<Position2f, string> dic, IDictionary<string, TransferStation<Position2f>> tsd)
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

            var routeA = new Route<Position2f>(routeAList);
            routeBList.Reverse();
            var routeB = new Route<Position2f>(routeBList);

            var offsetSplines = spline.OffsetPaths(4f);

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

        private static (List<WeekTimeCollection>, List<WeekTimeCollection>, List<Trip<Position2f>>) CreateRouteArrivalsAndDepartures(Route<Position2f> route, Path path, WeekTimeCollection initialDepartures, IReadOnlyList<Duration> waitingTimes, Func<Distance, Duration> travelFunc)
        {
            if (waitingTimes.Count != route.Stations.Count() - 2)
            {
                throw new ArgumentException();
            }

            var numTrips = initialDepartures.Count;
            var tripPrepareList = new List<List<(WeekTimePoint, WeekTimePoint)>>(numTrips);
            for (var i = 0; i < numTrips; ++i)
            {
                tripPrepareList.Add(new List<(WeekTimePoint, WeekTimePoint)>(route.Stations.Count())
                {
                    (null, initialDepartures.UnsortedWeekTimePoints.ElementAt(i))
                });
            }
            var arrivals = new List<WeekTimeCollection>();
            var departures = new List<WeekTimeCollection> { initialDepartures };

            for (var i = 1; i < route.Stations.Count(); ++i)
            {
                var stationPosition = route.Stations.ElementAt(i).Position;
                var stationPositionBefore = route.Stations.ElementAt(i - 1).Position;
                var startIndex = path.IndexOf(stationPositionBefore);
                var endIndex = path.IndexOf(stationPosition);
                var subPath = path.Subpath(startIndex, endIndex - startIndex);
                var distance = Distance.FromMeters(subPath.Length());
                var duration = travelFunc(distance);
                var arrivalList = (from WeekTimePoint departure in departures.Last().UnsortedWeekTimePoints select departure + duration).ToList();
                arrivals.Add(new WeekTimeCollection(arrivalList));
                if (i < route.Stations.Count() - 1)
                {
                    var departureList = (from WeekTimePoint arrival in arrivalList select arrival + waitingTimes[i - 1]).ToList();
                    departures.Add(new WeekTimeCollection(departureList));

                    if (arrivalList.Count != departureList.Count)
                    {
                        throw new InvalidOperationException();
                    }

                    for (var j = 0; j < numTrips; ++j)
                    {
                        tripPrepareList[j].Add((arrivalList[j], departureList[j]));
                    }
                }
                else
                {
                    for (var j = 0; j < numTrips; ++j)
                    {
                        tripPrepareList[j].Add((arrivalList[j], null));
                    }
                }
            }

            var tripList = new List<Trip<Position2f>>();
            foreach (var c in tripPrepareList)
            {
                var trip = new Trip<Position2f>(c.Select((t, idx) => (route.Stations.ElementAt(idx), t)).ToList());
                tripList.Add(trip);
            }

            return (arrivals, departures, tripList);
        }

        private static (List<WeekTimeCollection>, List<WeekTimeCollection>, List<Trip<Position2f>>) CreateRouteArrivalsAndDepartures(Route<Position2f> route, Path path, WeekTimeCollection initialDepartures, Duration waitingTime, Func<Distance, Duration> travelFunc)
        {
            var numTrips = initialDepartures.Count;
            var tripPrepareList = new List<List<(WeekTimePoint, WeekTimePoint)>>(numTrips);
            for (var i = 0; i < numTrips; ++i)
            {
                tripPrepareList.Add(new List<(WeekTimePoint, WeekTimePoint)>(route.Stations.Count())
                {
                    (null, initialDepartures.UnsortedWeekTimePoints.ElementAt(i))
                });
            }
            var arrivals = new List<WeekTimeCollection>();
            var departures = new List<WeekTimeCollection> { initialDepartures };

            for (var i = 1; i < route.Stations.Count(); ++i)
            {
                var stationPosition = route.Stations.ElementAt(i).Position;
                var stationPositionBefore = route.Stations.ElementAt(i - 1).Position;
                var startIndex = path.FindIndex(p => p.EqualPosition(stationPositionBefore));
                var endIndex = path.FindIndex(p => p.EqualPosition(stationPosition));
                var subPath = path.Subpath(startIndex, endIndex - startIndex + 1);
                var distance = Distance.FromMeters(subPath.Length());
                var duration = travelFunc(distance);
                var arrivalList = (from WeekTimePoint departure in departures.Last().UnsortedWeekTimePoints select departure + duration).ToList();
                arrivals.Add(new WeekTimeCollection(arrivalList));
                if (i < route.Stations.Count() - 1)
                {
                    var departureList = (from WeekTimePoint arrival in arrivalList select arrival + waitingTime).ToList();
                    departures.Add(new WeekTimeCollection(departureList));

                    if (arrivalList.Count != departureList.Count)
                    {
                        throw new InvalidOperationException();
                    }

                    for (var j = 0; j < numTrips; ++j)
                    {
                        tripPrepareList[j].Add((arrivalList[j], departureList[j]));
                    }
                }
                else
                {
                    for (var j = 0; j < numTrips; ++j)
                    {
                        tripPrepareList[j].Add((arrivalList[j], null));
                    }
                }
            }

            var tripList = new List<Trip<Position2f>>();
            foreach (var c in tripPrepareList)
            {
                var trip = new Trip<Position2f>(c.Select((t, idx) => (route.Stations.ElementAt(idx), t)).ToList());
                tripList.Add(trip);
            }

            return (arrivals, departures, tripList);
        }

        private static Duration SubwayTravelTimeFunc(Distance distance)
        {
            var meanAcceleration = Acceleration.FromMetersPerSecondSquared(0.6f);
            var maximalSpeed = Speed.FromKilometersPerHour(70f);
            var timeToReachMaximalSpeed = maximalSpeed / meanAcceleration;
            var neededDistanceToReachMaximalSpeed = meanAcceleration / 2 * timeToReachMaximalSpeed * timeToReachMaximalSpeed;
            if (distance < 2 * neededDistanceToReachMaximalSpeed) // distance is too small to reach maximalSpeed
            {
                return 2 * (distance / meanAcceleration).SquareRoot;
            }

            var remainingDistance = distance - 2 * neededDistanceToReachMaximalSpeed;
            return 2 * timeToReachMaximalSpeed + remainingDistance / maximalSpeed;
        }
    }
}
