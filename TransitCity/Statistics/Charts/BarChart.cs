using System.Linq;
using MiscUtil;
using Statistics.Data;

namespace Statistics.Charts
{
    public class BarChart<T> where T : struct
    {
        private readonly NamedData<T> _data;

        public BarChart(NamedData<T> data)
        {
            _data = data;
            BarCount = _data.Names.Count();
            Minimum = Operator<T>.Zero;
            Maximum = _data.Maximum;
        }

        public BarChart(NamedData<T> data, T max) : this(data)
        {
            Maximum = max;
        }

        public int BarCount { get; }

        public T Minimum { get; }

        public T Maximum { get; }
    }
}
