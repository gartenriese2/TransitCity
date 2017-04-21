using System.Collections.Generic;
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

        public T this[string key] => _data[key];

        public int BarCount { get; }

        public T Minimum { get; }

        public T Maximum { get; }

        public IEnumerable<string> Names => _data.Names;

        public IEnumerable<T> GetValues()
        {
            return _data.Names.Select(name => _data[name]);
        }
    }
}
