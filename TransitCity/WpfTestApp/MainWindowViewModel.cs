using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using Geometry;
using Time;
using Transit.Data;
using WpfDrawing.Annotations;
using WpfDrawing.Objects;
using WpfDrawing.Panel;
using WpfDrawing.Utility;

namespace WpfTestApp
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly Random _rand = new Random();
        private readonly DataManager _dataManager;
        private TimeSpan _time = TimeSpan.Zero;
        private string _weektime = string.Empty;

        public MainWindowViewModel()
        {
            _dataManager = new TestTransitData().DataManager;

            foreach (var station in _dataManager.AllStations)
            {
                var s = new Station
                {
                    VariableX = station.Position.X / 10000,
                    VariableY = station.Position.Y / 10000
                };
                PanelObjects.Add(s);
            }

            var activeVehicles = _dataManager.GetActiveVehiclePositionsAndDirections(new WeekTimePoint(DayOfWeek.Monday) + _time);
            foreach (var activeVehicle in activeVehicles)
            {
                var v = new Vehicle
                {
                    VariableX = activeVehicle.Item1.X / 10000,
                    VariableY = activeVehicle.Item1.Y / 10000
                };
                PanelObjects.Add(v);
            }

            var tmr = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
            tmr.Tick += OnTimerTick2;
            tmr.Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableNotifiableCollection<PanelObject> PanelObjects { get; } = new ObservableNotifiableCollection<PanelObject>();

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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnTimerTick2(object sender, EventArgs args)
        {
            _time += TimeSpan.FromSeconds(5);
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
                var v = new Vehicle
                {
                    VariableX = activeVehicle.Item1.X / 10000,
                    VariableY = activeVehicle.Item1.Y / 10000
                };
                PanelObjects.Add(v);
            }

            
        }
    }
}
