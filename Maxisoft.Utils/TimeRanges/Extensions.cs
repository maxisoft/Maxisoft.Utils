using System;

namespace Maxisoft.Utils.TimeRanges
{
    public static class Extensions
    {
        public static TimePoint ToTimePoint(this TimeSpan timeSpan)
        {
            return new TimePoint(timeSpan);
        }
    }
}