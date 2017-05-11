using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using Time;
using Transit.Data;
using Utility.MVVM;
using WpfDrawing.Annotations;
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

        public MainWindowViewModel()
        {
            _dataManager = new TestTransitData().DataManager;
            PlusCommand = new RelayCommand(o => _timeDelta *= 2.0, o => _timeDelta < 20.0);
            MinusCommand = new RelayCommand(o => _timeDelta /= 2.0, o => _timeDelta > 0.001);

            foreach (var station in _dataManager.AllStations)
            {
                var s = new Station
                {
                    X = station.Position.X / 10000,
                    Y = station.Position.Y / 10000
                };
                PanelObjects.Add(s);
            }

            var activeVehicles = _dataManager.GetActiveVehiclePositionsAndDirections(new WeekTimePoint(DayOfWeek.Monday) + _time);
            foreach (var activeVehicle in activeVehicles)
            {
                var v = new Vehicle(activeVehicle.Item1 / 10000, activeVehicle.Item2.Normalize());
                PanelObjects.Add(v);
            }

            var tmr = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
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

        private void OnTimerTick2(object sender, EventArgs args)
        {
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

            
        }
    }
}
