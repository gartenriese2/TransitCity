﻿using System;

namespace Time
{
    public class WeekTimeSpan
    {
        public WeekTimeSpan(WeekTimePoint begin, WeekTimePoint end)
        {
            Begin = begin;
            End = end;
        }

        public WeekTimePoint Begin { get; }

        public WeekTimePoint End { get; }

        public TimeSpan GetTimeSpan()
        {
            return WeekTimePoint.GetCorrectedDifference(Begin, End);
        }

        public bool IsInside(WeekTimePoint wtp)
        {
            if (Begin <= End)
            {
                return wtp >= Begin && wtp <= End;
            }

            return wtp >= Begin || wtp <= End;
        }
    }
}
