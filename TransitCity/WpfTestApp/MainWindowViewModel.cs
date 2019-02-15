using Transit;

namespace WpfTestApp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;

    using CitySimulation;

    using Geometry.Shapes;

    using Time;

    using Transit.Data;
    using Transit.Timetable;
    using Transit.Timetable.Algorithm;

    using Utility.MVVM;
    using Utility.Units;

    using WpfDrawing.Objects;
    using WpfDrawing.Panel;
    using WpfDrawing.Utility;

    public class MainWindowViewModel : PropertyChangedBase
    {
        #region Fields

        private readonly DataManager _dataManager;
        private readonly TransitConnectionInfo _transitConnectionInfo;
        private List<ResidentObject> _residentObjects = new List<ResidentObject>();
        private List<(VehicleObject, VehicleRidersObject)> _vehicleObjects = new List<(VehicleObject, VehicleRidersObject)>();
        private TimeSpan _time = TimeSpan.Zero;
        private string _weektime = string.Empty;
        private double _simulationSpeedFactor = 1.0;
        private double _simulationTime;
        private Point _center;
        private double _centerX;
        private double _centerY;
        private double _zoom;
        private int _activeConnectionsCount;
        private int _activeRiderCount;
        private int _activeVehicleCount;
        private double _percentageLoaded;
        private Visibility _percentageLoadedVisibility = Visibility.Hidden;

        #endregion

        public MainWindowViewModel()
        {
            _dataManager = new TestTransitData().DataManager;
            _transitConnectionInfo = new TransitConnectionInfo(new Dictionary<Resident, List<List<Connection>>>());
            Initialize();

            Speed0Command = new RelayCommand(o => _simulationSpeedFactor = 0.0);
            Speed1Command = new RelayCommand(o => _simulationSpeedFactor = 1.0);
            Speed2Command = new RelayCommand(o => _simulationSpeedFactor = 10.0);
            Speed3Command = new RelayCommand(o => _simulationSpeedFactor = 60.0);

            CenterX = 0.5;
            CenterY = 0.5;
            Zoom = 1.0;

            foreach (var lineInfo in _dataManager.AllLineInfos)
            {
                var color = LineInfoToColor(lineInfo);

                foreach (var path in lineInfo.RouteInfos.Select(ri => ri.Path))
                {
                    var r = new RouteObject(path, color);
                    PanelObjects.Add(r);
                }
            }

            var waitingConnections = _transitConnectionInfo.GetWaitingResidents(new WeekTimePoint(DayOfWeek.Monday) + _time);
            var waitingDictionary = new Dictionary<Station, int>();
            foreach (var connection in waitingConnections)
            {
                if (waitingDictionary.ContainsKey(connection.TargetStation))
                {
                    waitingDictionary[connection.TargetStation]++;
                }
                else
                {
                    waitingDictionary.Add(connection.TargetStation, 1);
                }
            }

            foreach (var lineInfo in _dataManager.AllLineInfos)
            {
                // First routeInfo text will be below
                foreach (var stationInfo in lineInfo.RouteInfos[0].StationInfos)
                {
                    var s = new StationObject(stationInfo.Station.Position);
                    PanelObjects.Add(s);
                    var swo = new StationWaitersObject((waitingDictionary.ContainsKey(stationInfo.Station) ? waitingDictionary[stationInfo.Station] : 0).ToString(), stationInfo.Station, true);
                    PanelObjects.Add(swo);
                }

                // Second routeInfo text will be above
                foreach (var stationInfo in lineInfo.RouteInfos[1].StationInfos)
                {
                    var s = new StationObject(stationInfo.Station.Position);
                    PanelObjects.Add(s);
                    var swo = new StationWaitersObject((waitingDictionary.ContainsKey(stationInfo.Station) ? waitingDictionary[stationInfo.Station] : 0).ToString(), stationInfo.Station, false);
                    PanelObjects.Add(swo);
                }
            }

            var activeVehicles = _dataManager.GetActiveVehiclePositionsAndDirections(new WeekTimePoint(DayOfWeek.Monday) + _time).ToList();
            var ridingConnections = _transitConnectionInfo.GetRidingResidents(new WeekTimePoint(DayOfWeek.Monday) + _time).ToList();
            var ridershipDictionary = new Dictionary<Trip, int>();
            foreach (var connection in ridingConnections)
            {
                var lineInfo = connection.LineInfo;
                var possibleVehicles = activeVehicles.Where(t => t.Item1 == lineInfo);
                foreach (var (_, routeInfo, trip, _, _) in possibleVehicles)
                {
                    if (!routeInfo.StationInfos.Select(si => si.Station).Contains(connection.SourceStation))
                    {
                        continue;
                    }

                    if (trip.DepartureAtStation(connection.SourceStation) == connection.SourceTime && trip.ArrivalAtStation(connection.TargetStation) == connection.TargetTime)
                    {
                        if (ridershipDictionary.ContainsKey(trip))
                        {
                            ridershipDictionary[trip]++;
                        }
                        else
                        {
                            ridershipDictionary.Add(trip, 1);
                        }
                    }
                }
            }

            foreach (var (lineInfo, _, trip, pos, vec) in activeVehicles)
            {
                var v = new VehicleObject(pos, vec.Normalize(), trip);
                PanelObjects.Add(v);
                var color = LineInfoToColor(lineInfo);
                var t = new VehicleRidersObject((ridershipDictionary.ContainsKey(trip) ? ridershipDictionary[trip] : 0).ToString(), pos, color);
                PanelObjects.Add(t);
                _vehicleObjects.Add((v, t));
            }

            var tmr = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16.67) };
            tmr.Tick += MainLoopTick;
            tmr.Start();
        }

        #region Properties

        public ObservableNotifiableCollection<PanelObject> PanelObjects { get; } = new ObservableNotifiableCollection<PanelObject>();

        public RelayCommand Speed0Command { get; }

        public RelayCommand Speed1Command { get; }

        public RelayCommand Speed2Command { get; }

        public RelayCommand Speed3Command { get; }

        public string WeekTime
        {
            get => _weektime;
            set
            {
                if (value != _weektime)
                {
                    _weektime = value;
                    OnPropertyChanged();
                }
            }
        }

        public double SimulationTime
        {
            get => _simulationTime;
            set
            {
                if (Math.Abs(value - _simulationTime) > double.Epsilon)
                {
                    _simulationTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public Point Center
        {
            get => _center;
            private set
            {
                if (_center != value)
                {
                    _center = value;
                    OnPropertyChanged();
                }
            }
        }

        public double CenterX
        {
            get => _centerX;
            set
            {
                if (Math.Abs(value - _centerX) > double.Epsilon)
                {
                    _centerX = value;
                    OnPropertyChanged();
                    Center = new Point(CenterX, CenterY);
                }
            }
        }

        public double CenterY
        {
            get => _centerY;
            set
            {
                if (Math.Abs(value - _centerY) > double.Epsilon)
                {
                    _centerY = value;
                    OnPropertyChanged();
                    Center = new Point(CenterX, CenterY);
                }
            }
        }

        public double Zoom
        {
            get => _zoom;
            set
            {
                if (Math.Abs(value - _zoom) > double.Epsilon)
                {
                    _zoom = value;
                    OnPropertyChanged();
                }
            }
        }

        public int ActiveConnectionsCount
        {
            get => _activeConnectionsCount;
            set
            {
                if (value != _activeConnectionsCount)
                {
                    _activeConnectionsCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public int ActiveRiderCount
        {
            get => _activeRiderCount;
            set
            {
                if (value != _activeRiderCount)
                {
                    _activeRiderCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public int ActiveVehicleCount
        {
            get => _activeVehicleCount;
            set
            {
                if (value != _activeVehicleCount)
                {
                    _activeVehicleCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public double PercentageLoaded
        {
            get => _percentageLoaded;
            set
            {
                if (Math.Abs(value - _percentageLoaded) > double.Epsilon)
                {
                    _percentageLoaded = value;
                    OnPropertyChanged();
                }
            }
        }

        public Visibility PercentageLoadedVisibility
        {
            get => _percentageLoadedVisibility;
            set
            {
                if (value != _percentageLoadedVisibility)
                {
                    _percentageLoadedVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public double MinZoom { get; } = 0.1;

        public double MaxZoom { get; } = 50.0;

        public Size WorldSize { get; } = new Size(10000, 10000);

        #endregion

        public void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var diff = e.Delta / 1000.0 * Zoom;
            Zoom = Math.Max(MinZoom, Math.Min(MaxZoom, Zoom + diff));
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            var delta = 0.1 / Zoom;
            if (e.Key == Key.Left)
            {
                CenterX = Math.Max(0.0, CenterX - delta);
            }
            if (e.Key == Key.Right)
            {
                CenterX = Math.Min(1.0, CenterX + delta);
            }
            if (e.Key == Key.Up)
            {
                CenterY = Math.Min(1.0, CenterY + delta);
            }
            if (e.Key == Key.Down)
            {
                CenterY = Math.Max(0.0, CenterY - delta);
            }
        }

        private static City CreateLondon()
        {
            var balham = new RandomDistrict("Balham", new Polygon(
                5000, 4500,
                5250, 4550,
                5500, 4625,
                5750, 4675,
                6000, 4700,
                6250, 4610,
                6500, 4500,
                6750, 4340,
                7000, 4150,
                7000, 4500,
                6500, 5500,
                5500, 6000,
                5000, 5500
            ), 4500, 16000);

            var buckhurst = new RandomDistrict("Buckhurst", new Polygon(
                2500, 5400,
                2750, 5450,
                3000, 5410,
                3250, 5350,
                3500, 5260,
                3750, 5125,
                4000, 5000,
                4250, 4850,
                4500, 4670,
                4750, 4570,
                5000, 4500,
                5000, 5500,
                5500, 6000,
                4500, 6500,
                2500, 6500
            ), 8000, 3000);

            var epping = new RandomDistrict("Epping", new Polygon(
                0, 3600,
                250, 3620,
                500, 3660,
                750, 3750,
                1000, 4000,
                1850, 5000,
                2000, 5120,
                2250, 5270,
                2500, 5400,
                2500, 6500,
                1000, 7000,
                0, 8000
            ), 6000, 2000);

            var morden = new RandomDistrict("Morden", new Polygon(
                0, 10000,
                0, 8000,
                1000, 7000,
                2500, 6500,
                3000, 6500,
                3000, 8000,
                4500, 10000
            ), 10000, 5000);

            var neasden = new RandomDistrict("Neasden", new Polygon(
                4500, 3500,
                5000, 3000,
                6000, 2500,
                6000, 4270,
                5750, 4300,
                5500, 4260,
                5250, 4150,
                5000, 4120,
                4750, 4160,
                4500, 4250
            ), 5000, 14000);

            var clapham = new RandomDistrict("Clapham", new Polygon(
                6000, 4270,
                6000, 1500,
                7000, 1000,
                7500, 1500,
                7500, 2740,
                7250, 3010,
                7000, 3400,
                6750, 3840,
                6500, 4050,
                6250, 4170
            ), 12000, 6000);

            var stockwell = new RandomDistrict("Stockwell", new Polygon(
                7500, 1500,
                7000, 1000,
                7500, 0,
                10000, 0,
                10000, 2300,
                9750, 2200,
                9500, 2100,
                9250, 2060,
                9000, 2040,
                8750, 2080,
                8500, 2150,
                8250, 2250,
                8000, 2390,
                7750, 2550,
                7500, 2740
            ), 2000, 5000);

            var debden = new RandomDistrict("Debden", new Polygon(
                0, 3000,
                5000, 3000,
                4500, 3500,
                4500, 4250,
                3500, 4750,
                3000, 4950,
                2750, 5000,
                2500, 4950,
                2250, 4800,
                2000, 4630,
                1500, 4000,
                1250, 3700,
                1000, 3490,
                750, 3350,
                500, 3270,
                0, 3270
            ), 8000, 3000);

            var amersham = new RandomDistrict("Amersham", new Polygon(
                3000, 3000,
                4000, 1500,
                6000, 1500,
                6000, 2500,
                5000, 3000
            ), 7500, 4000);

            var chesham = new RandomDistrict("Chesham", new Polygon(
                4000, 1500,
                4000, 0,
                7500, 0,
                7000, 1000,
                6000, 1500
            ), 3000, 2000);

            var stanmore = new RandomDistrict("Stanmore", new Polygon(
                0, 3000,
                0, 0,
                4000, 0,
                4000, 1500,
                3000, 3000
            ), 4500, 1500);

            var tooting = new RandomDistrict("Tooting", new Polygon(
                3000, 6500,
                4500, 6500,
                6500, 5500,
                7500, 7500,
                8000, 10000,
                4500, 10000,
                3000, 8000
            ), 3000, 500);

            var hampstead = new RandomDistrict("Hampstead", new Polygon(
                7000, 6500,
                10000, 6500,
                10000, 10000,
                8000, 10000,
                7500, 7500
            ), 4000, 1000);

            var watfrord = new RandomDistrict("Watford", new Polygon(
                7000, 4500,
                10000, 4500,
                10000, 6500,
                7000, 6500,
                6500, 5500
            ), 8000, 4000);

            var chigwell = new RandomDistrict("Chigwell", new Polygon(
                7000, 4500,
                7000, 4150,
                7150, 4000,
                7500, 3500,
                7750, 3200,
                8000, 3000,
                8500, 2720,
                8750, 2640,
                9000, 2600,
                9500, 2660,
                9750, 2750,
                10000, 2900,
                10000, 4500
            ), 7000, 2500);

            return new City("London", new List<IDistrict> { balham, buckhurst, epping, morden, neasden, clapham, stockwell, debden, amersham, chesham, stanmore, tooting, hampstead, watfrord, chigwell });
        }

        private static City CreateSmallLondon()
        {
            var balham = new RandomDistrict("Balham", new Polygon(
                5000, 4500,
                5250, 4550,
                5500, 4625,
                5750, 4675,
                6000, 4700,
                6250, 4610,
                6500, 4500,
                6750, 4340,
                7000, 4150,
                7000, 4500,
                6500, 5500,
                5500, 6000,
                5000, 5500
            ), 450, 1600);

            var buckhurst = new RandomDistrict("Buckhurst", new Polygon(
                2500, 5400,
                2750, 5450,
                3000, 5410,
                3250, 5350,
                3500, 5260,
                3750, 5125,
                4000, 5000,
                4250, 4850,
                4500, 4670,
                4750, 4570,
                5000, 4500,
                5000, 5500,
                5500, 6000,
                4500, 6500,
                2500, 6500
            ), 800, 300);

            var epping = new RandomDistrict("Epping", new Polygon(
                0, 3600,
                250, 3620,
                500, 3660,
                750, 3750,
                1000, 4000,
                1850, 5000,
                2000, 5120,
                2250, 5270,
                2500, 5400,
                2500, 6500,
                1000, 7000,
                0, 8000
            ), 600, 200);

            var morden = new RandomDistrict("Morden", new Polygon(
                0, 10000,
                0, 8000,
                1000, 7000,
                2500, 6500,
                3000, 6500,
                3000, 8000,
                4500, 10000
            ), 1000, 500);

            var neasden = new RandomDistrict("Neasden", new Polygon(
                4500, 3500,
                5000, 3000,
                6000, 2500,
                6000, 4270,
                5750, 4300,
                5500, 4260,
                5250, 4150,
                5000, 4120,
                4750, 4160,
                4500, 4250
            ), 500, 1400);

            var clapham = new RandomDistrict("Clapham", new Polygon(
                6000, 4270,
                6000, 1500,
                7000, 1000,
                7500, 1500,
                7500, 2740,
                7250, 3010,
                7000, 3400,
                6750, 3840,
                6500, 4050,
                6250, 4170
            ), 1200, 600);

            var stockwell = new RandomDistrict("Stockwell", new Polygon(
                7500, 1500,
                7000, 1000,
                7500, 0,
                10000, 0,
                10000, 2300,
                9750, 2200,
                9500, 2100,
                9250, 2060,
                9000, 2040,
                8750, 2080,
                8500, 2150,
                8250, 2250,
                8000, 2390,
                7750, 2550,
                7500, 2740
            ), 200, 500);

            var debden = new RandomDistrict("Debden", new Polygon(
                0, 3000,
                5000, 3000,
                4500, 3500,
                4500, 4250,
                3500, 4750,
                3000, 4950,
                2750, 5000,
                2500, 4950,
                2250, 4800,
                2000, 4630,
                1500, 4000,
                1250, 3700,
                1000, 3490,
                750, 3350,
                500, 3270,
                0, 3270
            ), 800, 300);

            var amersham = new RandomDistrict("Amersham", new Polygon(
                3000, 3000,
                4000, 1500,
                6000, 1500,
                6000, 2500,
                5000, 3000
            ), 750, 400);

            var chesham = new RandomDistrict("Chesham", new Polygon(
                4000, 1500,
                4000, 0,
                7500, 0,
                7000, 1000,
                6000, 1500
            ), 300, 200);

            var stanmore = new RandomDistrict("Stanmore", new Polygon(
                0, 3000,
                0, 0,
                4000, 0,
                4000, 1500,
                3000, 3000
            ), 450, 150);

            var tooting = new RandomDistrict("Tooting", new Polygon(
                3000, 6500,
                4500, 6500,
                6500, 5500,
                7500, 7500,
                8000, 10000,
                4500, 10000,
                3000, 8000
            ), 300, 50);

            var hampstead = new RandomDistrict("Hampstead", new Polygon(
                7000, 6500,
                10000, 6500,
                10000, 10000,
                8000, 10000,
                7500, 7500
            ), 400, 100);

            var watfrord = new RandomDistrict("Watford", new Polygon(
                7000, 4500,
                10000, 4500,
                10000, 6500,
                7000, 6500,
                6500, 5500
            ), 800, 400);

            var chigwell = new RandomDistrict("Chigwell", new Polygon(
                7000, 4500,
                7000, 4150,
                7150, 4000,
                7500, 3500,
                7750, 3200,
                8000, 3000,
                8500, 2720,
                8750, 2640,
                9000, 2600,
                9500, 2660,
                9750, 2750,
                10000, 2900,
                10000, 4500
            ), 700, 250);

            return new City("London", new List<IDistrict> { balham, buckhurst, epping, morden, neasden, clapham, stockwell, debden, amersham, chesham, stanmore, tooting, hampstead, watfrord, chigwell });
        }

        private static City CreateSmallCity()
        {
            var district = new RandomDistrict("City", new Polygon(
                0, 0,
                10000, 0,
                10000, 10000,
                0, 10000
            ), 10000, 10000);

            return new City("SmallCity", new List<IDistrict> { district });
        }

        private void UpdateStations(WeekTimePoint wtp)
        {
            var waitingConnections = _transitConnectionInfo.GetWaitingResidents(wtp);
            var waitingDictionary = new Dictionary<Station, int>();
            foreach (var connection in waitingConnections)
            {
                if (waitingDictionary.ContainsKey(connection.TargetStation))
                {
                    waitingDictionary[connection.TargetStation]++;
                }
                else
                {
                    waitingDictionary.Add(connection.TargetStation, 1);
                }
            }

            foreach (var swo in PanelObjects.OfType<StationWaitersObject>())
            {
                swo.Text = (waitingDictionary.ContainsKey(swo.Station) ? waitingDictionary[swo.Station] : 0).ToString();
            }
        }

        private void UpdateVehicles(WeekTimePoint wtp)
        {
            var activeVehicles = _dataManager.GetActiveVehiclePositionsAndDirections(wtp).ToList();
            ActiveVehicleCount = activeVehicles.Count;
            var ridingConnections = _transitConnectionInfo.GetRidingResidents(wtp).ToList();
            ActiveRiderCount = ridingConnections.Count;
            var ridershipDictionary = new Dictionary<Trip, int>();
            foreach (var connection in ridingConnections)
            {
                var lineInfo = connection.LineInfo;
                var possibleVehicles = activeVehicles.Where(t => t.Item1 == lineInfo);
                foreach (var (_, routeInfo, trip, _, _) in possibleVehicles)
                {
                    if (!routeInfo.StationInfos.Select(si => si.Station).Contains(connection.SourceStation))
                    {
                        continue;
                    }

                    if (trip.DepartureAtStation(connection.SourceStation) == connection.SourceTime && trip.ArrivalAtStation(connection.TargetStation) == connection.TargetTime)
                    {
                        if (ridershipDictionary.ContainsKey(trip))
                        {
                            ridershipDictionary[trip]++;
                        }
                        else
                        {
                            ridershipDictionary.Add(trip, 1);
                        }
                    }
                }
            }

            if (ridershipDictionary.Sum(p => p.Value) != ridingConnections.Count)
            {
                throw new InvalidOperationException();
            }

            var vehicleObjects = new List<(VehicleObject, VehicleRidersObject)>(activeVehicles.Count);

            // Remove old vehicles
            var toBeRemoved = _vehicleObjects.Where(x => !activeVehicles.Select(r => r.Item3).Contains(x.Item1.Trip)).ToList();
            foreach (var (vo, to) in toBeRemoved)
            {
                PanelObjects.Remove(vo);
                PanelObjects.Remove(to);
            }

            // Update or create active vehicles
            foreach (var (lineInfo, routeInfo, trip, pos, dir) in activeVehicles)
            {
                if (_vehicleObjects.Exists(t => t.Item1.Trip == trip))
                {
                    var (vo, to) = _vehicleObjects.First(t => t.Item1.Trip == trip);
                    vo.Update(pos, dir);
                    to.X = pos.X;
                    to.Y = pos.Y;
                    to.Text = (ridershipDictionary.ContainsKey(trip) ? ridershipDictionary[trip] : 0).ToString();
                    vehicleObjects.Add((vo, to));
                }
                else
                {
                    var vo = new VehicleObject(pos, dir.Normalize(), trip);
                    PanelObjects.Add(vo);
                    var color = LineInfoToColor(lineInfo);
                    var to = new VehicleRidersObject((ridershipDictionary.ContainsKey(trip) ? ridershipDictionary[trip] : 0).ToString(), pos, color);
                    PanelObjects.Add(to);
                    vehicleObjects.Add((vo, to));
                }
            }

            _vehicleObjects = vehicleObjects;

            if (_vehicleObjects.Sum(v => int.Parse(v.Item2.Text)) != ridingConnections.Count)
            {
                throw new InvalidOperationException();
            }

            if (PanelObjects.OfType<VehicleRidersObject>().Sum(t => int.Parse(t.Text)) != ridingConnections.Count)
            {
                throw new InvalidOperationException();
            }
        }

        private void UpdateResidents(WeekTimePoint wtp)
        {
            var activeConnections = _transitConnectionInfo.GetActiveConnections(wtp).ToList();
            ActiveConnectionsCount = activeConnections.Count;

            var activeResidents = _transitConnectionInfo.GetActiveResidents(activeConnections, wtp).ToList();
            var residentObjects = new List<ResidentObject>(activeResidents.Count);

            // Add new residents
            foreach (var (r, p, v) in activeResidents.Where(x => !_residentObjects.Select(r => r.Resident).Contains(x.Item1)))
            {
                var ro = new ResidentObject(p, v, r);
                PanelObjects.Add(ro);
                residentObjects.Add(ro);
            }

            // Remove or Update old residents
            var removedResidents = _residentObjects.Where(x => !activeResidents.Select(r => r.Item1).Contains(x.Resident)).ToList();
            for (var i = PanelObjects.Count - 1; i >= 0; --i)
            {
                var po = PanelObjects[i];
                if (!(po is ResidentObject))
                {
                    continue;
                }

                var ro = (ResidentObject)po;
                if (removedResidents.Exists(x => x.Resident == ro.Resident))
                {
                    PanelObjects.RemoveAt(i);
                }
                else
                {
                    var nr = activeResidents.Find(x => x.Item1 == ro.Resident);
                    ro.Update(nr.Item2, nr.Item3);
                    residentObjects.Add(ro);
                }
            }

            _residentObjects = residentObjects;
        }

        private void MainLoopTick(object sender, EventArgs args)
        {
            var sw = new Stopwatch();
            sw.Start();
            
            _time += TimeSpan.FromSeconds(_simulationSpeedFactor);
            var wtp = new WeekTimePoint(DayOfWeek.Monday) + _time;
            WeekTime = wtp.ToString();

            UpdateStations(wtp);
            UpdateVehicles(wtp);
            UpdateResidents(wtp);

            sw.Stop();
            SimulationTime = sw.ElapsedMilliseconds;
        }

        private async void Initialize()
        {
            PercentageLoadedVisibility = Visibility.Visible;

            var city = CreateSmallLondon();
            var rnd = new Random();
            var workerScheduleTuples = city.Residents.Where(r => r.HasJob).Select(r => (r, JobSchedule.CreateRandom(rnd))).ToList();
            var raptor = new Raptor(TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(15), _dataManager);
            var count = 0;
            foreach (var (worker, schedule) in workerScheduleTuples)
            {
                var walkingSpeed = Speed.FromKilometersPerHour(5 + rnd.NextDouble() * 5);
                var workerTaskList = new List<Task<List<Connection>>>();
                foreach (var scheduleWts in schedule.WeekTimeSpans)
                {
                    workerTaskList.Add(Task.Factory.StartNew(() => raptor.ComputeReverse(worker.Position, scheduleWts.Begin, worker.Job.Position, walkingSpeed)));
                    workerTaskList.Add(Task.Factory.StartNew(() => raptor.Compute(worker.Job.Position, scheduleWts.End, worker.Position, walkingSpeed)));
                }

                await Task.WhenAll(workerTaskList.ToArray());
                var connections = workerTaskList.Select(t => t.Result).ToList();

                // Add random or existing waits after walking to station
                foreach (var connection in connections)
                {
                    if (connection.Any() && connection[0].Type == Connection.TypeEnum.WalkToStation)
                    {
                        var walkToStation = connection[0];
                        var ride = connection[1];
                        var waitTime = WeekTimePoint.GetCorrectedDifference(walkToStation.TargetTime, connection[1].SourceTime);
                        if (waitTime.Ticks == 0)
                        {
                            var (_, _, stationInfo) = _dataManager.GetInfos(walkToStation.TargetStation);
                            var (wtp, _) = stationInfo.GetPreviousDepartureAndTripArrayBinarySearch(walkToStation.TargetTime);
                            var maxDiff = WeekTimePoint.GetCorrectedDifference(wtp, walkToStation.TargetTime);
                            waitTime = TimeSpan.FromSeconds(new Random().Next(Math.Min((int)maxDiff.TotalSeconds, 300)));
                            connection[0] = Connection.CreateWalkToStation(walkToStation.SourcePos, walkToStation.SourceTime - waitTime, walkToStation.TargetStation, walkToStation.TargetTime - waitTime);
                            connection.Insert(1, Connection.CreateWait(ride.SourceStation, ride.LineInfo, connection[0].TargetTime, ride.SourceTime));
                        }
                        else
                        {
                            connection.Insert(1, Connection.CreateWait(ride.SourceStation, ride.LineInfo, walkToStation.TargetTime, ride.SourceTime));
                        }
                    }
                }

                // Add waits at transfers to connections
                foreach (var connection in connections)
                {
                    var transferCount = connection.Count(c => c.Type == Connection.TypeEnum.Transfer);
                    if (transferCount == 0)
                    {
                        continue;
                    }

                    for (var i = 0; i < transferCount; ++i)
                    {
                        var numTransfers = 0;
                        for (var index = 0; index < connection.Count; index++)
                        {
                            var connectionStep = connection[index];
                            if (connectionStep.Type == Connection.TypeEnum.Transfer)
                            {
                                ++numTransfers;
                                if (numTransfers > i)
                                {
                                    var nextConnectionStep = connection[index + 1];
                                    connection.Insert(index + 1, Connection.CreateWait(connectionStep.TargetStation, nextConnectionStep.LineInfo, connectionStep.TargetTime, nextConnectionStep.SourceTime));
                                    break;
                                }
                            }
                        }
                    }
                    
                }

                var dic = new Dictionary<Resident, List<List<Connection>>>
                {
                    [worker] = connections
                };
                _transitConnectionInfo.AddConnections(dic);

                ++count;
                PercentageLoaded = 100.0 * count / workerScheduleTuples.Count;
            }

            PercentageLoadedVisibility = Visibility.Collapsed;
        }

        private Color LineInfoToColor(LineInfo lineInfo)
        {
            Color color;
            switch (lineInfo.Line.Name)
            {
                case "1":
                    color = Colors.Red;
                    break;
                case "2":
                    color = Colors.DarkGreen;
                    break;
                case "3":
                    color = Colors.DarkBlue;
                    break;
                case "4":
                    color = Colors.Orange;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return color;
        }
    }
}
