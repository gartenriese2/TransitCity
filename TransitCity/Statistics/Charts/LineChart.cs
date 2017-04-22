using System;
using System.Collections.Generic;
using System.Linq;
using Statistics.Data;

namespace Statistics.Charts
{
    public class LineChart
    {
        private readonly RangedData _data;

        public LineChart(RangedData data)
        {
            _data = data;
        }

        public float XMin => _data.XMin;

        public float XMax => _data.XMax;

        public float YMax => GetValues().Max();

        public List<Tuple<float, float>> Ranges => _data.Ranges;

        public IEnumerable<float> GetValues()
        {
            return _data.Ranges.Select(range => _data[range]);
        }
    }
}
