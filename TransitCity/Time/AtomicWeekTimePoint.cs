using Utility.Threading;

namespace Time
{
    public class AtomicWeekTimePoint : WeekTimePoint, IAtomic<AtomicWeekTimePoint>
    {
        public AtomicWeekTimePoint(WeekTimePoint wtp) : base(wtp.TimePoint)
        {
        }

        public void Replace(AtomicWeekTimePoint other)
        {
            TimePoint = other.TimePoint;
        }
    }
}
