using System;
using System.Collections.Generic;
using System.Linq;

namespace Statistics.Data
{
    public class RangedData : Data
    {
        public RangedData(float xMin, float delta, int steps)
        {
            if (steps <= 1)
            {
                throw new InvalidOperationException();
            }

            if (delta <= 0f)
            {
                throw new InvalidOperationException();
            }

            XMin = xMin;
            XMax = xMin + steps * delta;
            Delta = delta;
            Steps = steps;

            for (var i = 0; i < steps; ++i)
            {
                Ranges.Add(new Tuple<float, float>(xMin + i * delta, xMin + (i + 1) * delta));
            }
        }

        public float this[Tuple<float, float> key] => DatapointCollection.Where(dp => ((FloatDatapoint)dp).X >= key.Item1 && ((FloatDatapoint)dp).X <= key.Item2).Aggregate(0f, (sum, dp) => sum + ((FloatDatapoint)dp).Y);

        public float XMin { get; }

        public float XMax { get; }

        public float Delta { get; }

        public int Steps { get; }

        public List<Tuple<float, float>> Ranges { get; } = new List<Tuple<float,float>>();

        public void AddDatapoint(FloatDatapoint dp)
        {
            if (dp.X < XMin || dp.X > XMax)
            {
                throw new ArgumentOutOfRangeException();
            }

            base.AddDatapoint(dp);
        }
    }
}
