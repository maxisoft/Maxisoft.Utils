using System;
using System.Collections;
using System.Collections.Generic;

namespace Maxisoft.Utils.TimeRanges
{
    public struct TimeRange : IEquatable<TimeRange>, IEnumerable<TimePoint>
    {
        public readonly TimePoint From;

        public readonly TimePoint To;

        public TimeRange(TimePoint from, TimePoint to)
        {
            From = from;
            To = to;
        }

        public static bool TryParse(string playload, out TimeRange timePoint) =>
            Parser.TryParse(playload, out timePoint);
        
        public bool IsValid()
        {
            return From <= To;
        }

        public bool IsEmpty()
        {
            return From == To;
        }

        public bool Overlaps(TimePoint timePoint)
        {
            return IsValid() && Overlap(From.TotalSeconds, To.TotalSeconds, timePoint.TotalSeconds,
                       timePoint.TotalSeconds);
        }

        public bool Overlaps(TimeRange timeRange)
        {
            return IsValid() && Overlap(From.TotalSeconds, To.TotalSeconds, timeRange.From.TotalSeconds,
                       timeRange.To.TotalSeconds);
        }

        private static bool Overlap(long s, long e, long s1, long e1) 
        {
            if(s >= s1 && s <= e1)
                return true;
            if(s1 >= s && s1 <= e)
                return true;
            return false;
        }

        public bool Equals(TimeRange other)
        {
            return From.Equals(other.From) && To.Equals(other.To);
        }

        public IEnumerator<TimePoint> GetEnumerator()
        {
            yield return From;
            yield return To;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TimeRange && Equals((TimeRange) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (From.GetHashCode() * 397) ^ To.GetHashCode();
            }
        }

        public static bool operator ==(TimeRange left, TimeRange right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TimeRange left, TimeRange right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"TimeRange({nameof(From)}: {From}, {nameof(To)}: {To})";
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}