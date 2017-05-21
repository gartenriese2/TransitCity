using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
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
        private Point _center;
        private double _centerX;
        private double _centerY;
        private double _zoom;

        public MainWindowViewModel()
        {
            _dataManager = new TestTransitData().DataManager;
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

        public double MinZoom { get; } = 0.1;

        public double MaxZoom { get; } = 10.0;

        public Size WorldSize { get; } = new Size(10000, 10000);

        public void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var diff = e.Delta / 1000.0 * Zoom;
            Zoom = Math.Max(MinZoom, Math.Min(MaxZoom, Zoom + diff));
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            var delta = 0.1;
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
                var v = new Vehicle(activeVehicle.Item1, activeVehicle.Item2.Normalize());
                PanelObjects.Add(v);
            }
            sw.Stop();
            SimulationTime = sw.ElapsedMilliseconds;
        }
    }
}
