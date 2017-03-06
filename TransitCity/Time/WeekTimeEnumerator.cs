using System;
using System.Collections;
using System.Collections.Generic;

namespace Time
{
    public class WeekTimeEnumerator : IEnumerator
    {
        private readonly List<WeekTimePoint> _weekTimePoints;

        private int _position = -1;

        public WeekTimeEnumerator(List<WeekTimePoint> weekTimePoints)
        {
            _weekTimePoints = weekTimePoints;
        }

        public bool MoveNext()
        {
            ++_position;
            return _position < _weekTimePoints.Count;
        }

        public void Reset()
        {
            _position = -1;
        }

        public WeekTimePoint Current
        {
            get
            {
                try
                {
                    return _weekTimePoints[_position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        object IEnumerator.Current => Current;
    }
}
