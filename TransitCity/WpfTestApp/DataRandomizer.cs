// DataRandomizer.cs by Charles Petzold, December 2008

using System;
using System.Collections.Generic;
using System.Windows.Threading;
using WpfDrawing.Objects;

namespace WpfTestApp
{
    public class DataRandomizer<T> where T : DataPoint, new()
    {
        private readonly IList<T> _collection;
        private readonly Random _rand = new Random();

        public DataRandomizer(IList<T> collection, int numPoints)
        {
            _collection = collection;

            for (var i = 0; i < numPoints; i++)
            {
                var dataPoint = new T
                {
                    Id = FabricateIdString(),
                    Type = _rand.Next(6)
                };
                switch (dataPoint.Type)
                {
                    case 0:
                        dataPoint.VariableX = _rand.NextDouble();
                        dataPoint.VariableY = 0.25 * _rand.NextDouble() + 0.17 - 0.125;
                        break;

                    case 1:
                        dataPoint.VariableX = _rand.NextDouble();
                        dataPoint.VariableY = 0.25 * _rand.NextDouble() + 0.50 - 0.125;
                        break;

                    case 2:
                        dataPoint.VariableX = _rand.NextDouble();
                        dataPoint.VariableY = 0.25 * _rand.NextDouble() + 0.83 - 0.125;
                        break;

                    case 3:
                        dataPoint.VariableX = _rand.NextDouble();
                        dataPoint.VariableY = 0.75 * dataPoint.VariableX + 0.25 * _rand.NextDouble();
                        break;

                    case 4:
                        dataPoint.VariableX = _rand.NextDouble();
                        dataPoint.VariableY = 0.75 * (1 - dataPoint.VariableX) + 0.25 * _rand.NextDouble();
                        break;

                    case 5:
                        dataPoint.VariableX = 0.5 * _rand.NextDouble() + 0.25;
                        dataPoint.VariableY = _rand.NextDouble();
                        break;
                }

                collection.Add(dataPoint);
            }

            var tmr = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(10)};
            tmr.Tick += OnTimerTick;
            tmr.Start();
        }

        string FabricateIdString()
        {
            var str = "";
            var num = _rand.Next(3, 11);
            char[] vowels = { 'A', 'E', 'I', 'O', 'U' };
            char[] cons = { 'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'L',
                'M', 'N', 'P', 'R', 'S', 'T', 'V', 'W', 'Z' };

            for (var i = 0; i < num; i++)
            {
                var ch = i % 2 == 0 ? cons[_rand.Next(cons.Length)] :
                    vowels[_rand.Next(vowels.Length)];
                str += i == 0 ? ch : char.ToLower(ch);
            }
            return str;
        }

        void OnTimerTick(object sender, EventArgs args)
        {
            for (var i = 0; i < _collection.Count; i++)
            {
                var dataPoint = _collection[i];
                var change = 0.01 * (_rand.NextDouble() - 0.5);

                if (_rand.Next(2) == 0)
                    dataPoint.VariableX =
                        Math.Max(0, Math.Min(1, dataPoint.VariableX + change));
                else
                    dataPoint.VariableY =
                        Math.Max(0, Math.Min(1, dataPoint.VariableY + change));
            }
        }
    }

    public class DataRandomizer2<T> where T : PanelObject, new()
    {
        private readonly IList<T> _collection;
        private readonly Random _rand = new Random();

        public DataRandomizer2(IList<T> collection, int numPoints)
        {
            _collection = collection;

            for (var i = 0; i < numPoints; i++)
            {
                var dataPoint = new T
                {
                    X = _rand.NextDouble(),
                    Y = _rand.NextDouble()
                };

                collection.Add(dataPoint);
            }

            var tmr = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
            tmr.Tick += OnTimerTick;
            tmr.Start();
        }

        void OnTimerTick(object sender, EventArgs args)
        {
            foreach (var dataPoint in _collection)
            {
                var change = 0.01 * (_rand.NextDouble() - 0.5);

                if (_rand.Next(2) == 0)
                    dataPoint.X =
                        Math.Max(0, Math.Min(1, dataPoint.X + change));
                else
                    dataPoint.Y =
                        Math.Max(0, Math.Min(1, dataPoint.Y + change));
            }
        }
    }
}