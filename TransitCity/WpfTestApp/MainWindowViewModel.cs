using System;
using System.Windows.Threading;
using Time;
using Transit.Data;
using WpfDrawing.Objects;
using WpfDrawing.Panel;
using WpfDrawing.Utility;

namespace WpfTestApp
{
    public class MainWindowViewModel
    {
        private readonly Random _rand = new Random();
        private readonly DataManager _dataManager;
        private TimeSpan _time = TimeSpan.Zero;

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

            //var tmr = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
            //tmr.Tick += OnTimerTick;
            //tmr.Start();

            var tmr2 = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            tmr2.Tick += OnTimerTick2;
            tmr2.Start();
        }

        public ObservableNotifiableCollection<PanelObject> PanelObjects { get; } = new ObservableNotifiableCollection<PanelObject>();

        private void OnTimerTick(object sender, EventArgs args)
        {
            foreach (var panelObject in PanelObjects)
            {
                var change = 0.01 * (_rand.NextDouble() - 0.5);

                if (_rand.Next(2) == 0)
                {
                    panelObject.VariableX = Math.Max(0, Math.Min(1, panelObject.VariableX + change));
                }
                else
                {
                    panelObject.VariableY = Math.Max(0, Math.Min(1, panelObject.VariableY + change));
                }
            }
        }

        private void OnTimerTick2(object sender, EventArgs args)
        {
            _time += TimeSpan.FromSeconds(2);
            var activeVehicles = _dataManager.GetActiveVehiclePositionsAndDirections(new WeekTimePoint(DayOfWeek.Monday) + _time);

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
