using System;
using System.Windows.Threading;
using WpfDrawing.Objects;
using WpfDrawing.Panel;
using WpfDrawing.Utility;

namespace WpfTestApp
{
    public class MainWindowViewModel
    {
        private readonly Random _rand = new Random();

        public MainWindowViewModel()
        {
            var tmr = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
            tmr.Tick += OnTimerTick;
            tmr.Start();

            var tmr2 = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            tmr2.Tick += OnTimerTick2;
            tmr2.Start();
        }

        public ObservableNotifiableCollection<PanelObject> DataPoints { get; } = new ObservableNotifiableCollection<PanelObject>();

        private void OnTimerTick(object sender, EventArgs args)
        {
            foreach (var panelObject in DataPoints)
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
            var dataPoint = new Station
            {
                VariableX = _rand.NextDouble(),
                VariableY = _rand.NextDouble()
            };
            DataPoints.Add(dataPoint);
        }
    }
}
