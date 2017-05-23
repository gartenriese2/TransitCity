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
using Geometry;
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

namespace WpfTestApp
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        private readonly DataManager _dataManager;
        private readonly TransitConnectionInfo _transitConnectionInfo;
        private TimeSpan _time = TimeSpan.Zero;
        private string _weektime = string.Empty;
        private double _timeDelta = 5.0;
        private double _simulationTime;
        private Point _center;
        private double _centerX;
        private double _centerY;
        private double _zoom;
        private int _activeConnectionsCount;

        public MainWindowViewModel()
        {
            //var city = CreateCity();
            var city = CreateSmallCity();
            var rnd = new Random();
            var workerScheduleTuples = city.Residents.Where(r => r.HasJob).Select(r => (r, JobSchedule.CreateRandom(rnd))).ToList();
            _dataManager = new TestTransitData().DataManager;
            var raptor = new Raptor(TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(15), _dataManager);
            var workerConnectionsDictionary = new Dictionary<Resident, List<List<Connection>>>();
            foreach (var (worker, schedule) in workerScheduleTuples)
            {
                var workerTaskList = new List<Task<List<Connection>>>();
                foreach (var scheduleWts in schedule.WeekTimeSpans)
                {
                    workerTaskList.Add(Task.Factory.StartNew(() => raptor.ComputeReverse(worker.Position, scheduleWts.Begin, worker.Job.Position, Speed.FromKilometersPerHour(8))));
                    workerTaskList.Add(Task.Factory.StartNew(() => raptor.Compute(worker.Job.Position, scheduleWts.End, worker.Position, Speed.FromKilometersPerHour(8))));
                }

                Task.WaitAll(workerTaskList.ToArray());
                workerConnectionsDictionary.Add(worker, workerTaskList.Select(t => t.Result).ToList());
            }

            _transitConnectionInfo = new TransitConnectionInfo(workerConnectionsDictionary);

            PlusCommand = new RelayCommand(o => _timeDelta *= 2.0, o => _timeDelta < 20.0);
            MinusCommand = new RelayCommand(o => _timeDelta /= 2.0, o => _timeDelta > 0.001);
            CenterX = 0.5;
            CenterY = 0.5;
            Zoom = 1.0;

            foreach (var lineInfo in _dataManager.AllLineInfos)
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

                foreach (var path in lineInfo.RouteInfos.Select(ri => ri.Path))
                {
                    var r = new Route(path, color);
                    PanelObjects.Add(r);
                }
            }

            foreach (var station in _dataManager.AllStations)
            {
                var s = new Station(station.Position);
                PanelObjects.Add(s);
            }

            var activeVehicles = _dataManager.GetActiveVehiclePositionsAndDirections(new WeekTimePoint(DayOfWeek.Monday) + _time);
            foreach (var activeVehicle in activeVehicles)
            {
                var v = new Vehicle(activeVehicle.Item1, activeVehicle.Item2.Normalize());
                PanelObjects.Add(v);
            }

            var tmr = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16.67) };
            tmr.Tick += OnTimerTick2;
            tmr.Start();
        }

        public ObservableNotifiableCollection<PanelObject> PanelObjects { get; } = new ObservableNotifiableCollection<PanelObject>();

        public RelayCommand PlusCommand { get; }

        public RelayCommand MinusCommand { get; }

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

        public double MinZoom { get; } = 0.1;

        public double MaxZoom { get; } = 50.0;

        public Size WorldSize { get; } = new Size(10000, 10000);

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

        private static City CreateCity()
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

        private static City CreateSmallCity()
        {
            var district = new RandomDistrict("City", new Polygon(
                0, 0,
                10000, 0,
                10000, 10000,
                0, 10000
            ), 2000, 2000);

            return new City("SmallCity", new List<IDistrict> { district });
        }

        private void OnTimerTick2(object sender, EventArgs args)
        {
            var sw = new Stopwatch();
            sw.Start();
            _time += TimeSpan.FromSeconds(_timeDelta);
            var wtp = new WeekTimePoint(DayOfWeek.Monday) + _time;
            WeekTime = wtp.ToString();
            
            var activeVehicles = _dataManager.GetActiveVehiclePositionsAndDirections(wtp).ToList();
            for (var i = PanelObjects.Count - 1; i >= 0; --i)
            {
                if (PanelObjects[i] is Vehicle || PanelObjects[i] is ResidentObject)
                {
                    PanelObjects.RemoveAt(i);
                }
            }

            foreach (var activeVehicle in activeVehicles)
            {
                var v = new Vehicle(activeVehicle.Item1, activeVehicle.Item2.Normalize());
                PanelObjects.Add(v);
            }

            var activeConnections = _transitConnectionInfo.GetActiveConnectionsDictionary(wtp).ToList();
            ActiveConnectionsCount = activeConnections.Count;
            var walkingConnections = activeConnections.Where(c => c.Type == Connection.TypeEnum.WalkToStation || c.Type == Connection.TypeEnum.WalkFromStation);
            foreach (var wc in walkingConnections)
            {
                var vec = wc.Type == Connection.TypeEnum.WalkToStation
                    ? wc.TargetStation.EntryPosition - wc.SourcePos
                    : wc.TargetPos - wc.SourceStation.ExitPosition;
                var from = wc.Type == Connection.TypeEnum.WalkToStation ? wc.SourcePos : wc.SourceStation.ExitPosition;
                var to = wc.Type == Connection.TypeEnum.WalkToStation ? wc.TargetStation.EntryPosition : wc.TargetPos;
                var t = (wtp - wc.SourceTime).TotalMilliseconds / (wc.TargetTime - wc.SourceTime).TotalMilliseconds;
                var pos = Position2d.Lerp(t, from, to);
                var v = new ResidentObject(pos, vec);
                PanelObjects.Add(v);
            }

            sw.Stop();
            SimulationTime = sw.ElapsedMilliseconds;
        }
    }
}
