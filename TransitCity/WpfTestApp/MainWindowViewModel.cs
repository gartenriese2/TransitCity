using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using System.Windows.Threading;
using Geometry;
using Time;
using Transit.Data;
using Utility.MVVM;
using WpfDrawing.Objects;
using WpfDrawing.Panel;
using WpfDrawing.Utility;

namespace WpfTestApp
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        private readonly DataManager _dataManager;
        private TimeSpan _time = TimeSpan.Zero;
        private string _weektime = string.Empty;
        private double _timeDelta = 5.0;
        private double _simulationTime;
        private double _zoom = 1.0;

        public MainWindowViewModel()
        {
            _dataManager = new TestTransitData().DataManager;
            PlusCommand = new RelayCommand(o => _timeDelta *= 2.0, o => _timeDelta < 20.0);
            MinusCommand = new RelayCommand(o => _timeDelta /= 2.0, o => _timeDelta > 0.001);
            ZoomInCommand = new RelayCommand(o => ZoomIn());
            ZoomOutCommand = new RelayCommand(o => ZoomOut());

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
                var s = new Station(station.Position / 10000);
                PanelObjects.Add(s);
            }

            var activeVehicles = _dataManager.GetActiveVehiclePositionsAndDirections(new WeekTimePoint(DayOfWeek.Monday) + _time);
            foreach (var activeVehicle in activeVehicles)
            {
                var v = new Vehicle(activeVehicle.Item1 / 10000, activeVehicle.Item2.Normalize());
                PanelObjects.Add(v);
            }

            var tmr = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16.67) };
            tmr.Tick += OnTimerTick2;
            tmr.Start();
        }

        public ObservableNotifiableCollection<PanelObject> PanelObjects { get; } = new ObservableNotifiableCollection<PanelObject>();

        public RelayCommand PlusCommand { get; }

        public RelayCommand MinusCommand { get; }

        public RelayCommand ZoomInCommand { get; }

        public RelayCommand ZoomOutCommand { get; }

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

        public double MinZoom { private get; set; }

        public double MaxZoom { private get; set; }

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
                if (PanelObjects[i] is Vehicle)
                {
                    PanelObjects.RemoveAt(i);
                }
            }

            foreach (var activeVehicle in activeVehicles)
            {
                var v = new Vehicle(activeVehicle.Item1 / 10000, activeVehicle.Item2.Normalize());
                PanelObjects.Add(v);
            }
            sw.Stop();
            SimulationTime = sw.ElapsedMilliseconds;
        }

        private void ZoomIn()
        {
            if (Zoom * 2 <= MaxZoom)
            {
                Zoom *= 2;
            }
        }

        private void ZoomOut()
        {
            if (Zoom / 2 >= MinZoom)
            {
                Zoom /= 2;
            }
        }
    }
}
