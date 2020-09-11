using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Maxisoft.Utils.Time
{
    public readonly struct PreciseMonotonicTimestamp : IMonotonicTimestamp, IComparable<PreciseMonotonicTimestamp>, IEquatable<PreciseMonotonicTimestamp>
    {
        public readonly long Ticks;
        public static long Frequency => Stopwatch.Frequency;

        public PreciseMonotonicTimestamp(long timestamp)
        {
            Ticks = timestamp;
        }

        public bool IsZero => Ticks == 0;
        
        public static PreciseMonotonicTimestamp Now => new PreciseMonotonicTimestamp(Stopwatch.GetTimestamp());

        public static explicit operator long(PreciseMonotonicTimestamp t)
        {
            return t.Ticks;
        }
        
        public static explicit operator TimeSpan(PreciseMonotonicTimestamp t)
        {
            return TimeSpan.FromSeconds(t.Ticks / (double) Frequency);
        }

        public static TimeSpan operator +(PreciseMonotonicTimestamp to, PreciseMonotonicTimestamp from)
        {
            return (TimeSpan) to + (TimeSpan) from;
        }

        public static TimeSpan operator -(PreciseMonotonicTimestamp to, PreciseMonotonicTimestamp from)
        {
            return (TimeSpan) to - (TimeSpan) from;
        }

        private static PreciseMonotonicTimestamp TimeSpanToTimestamp(TimeSpan ts)
        {
            return new PreciseMonotonicTimestamp((long) (ts.TotalSeconds * Frequency));
        }

        public static PreciseMonotonicTimestamp operator +(PreciseMonotonicTimestamp to, TimeSpan ts)
        {
            return TimeSpanToTimestamp((TimeSpan) to + ts);
        }

        public static PreciseMonotonicTimestamp operator -(PreciseMonotonicTimestamp to, TimeSpan ts)
        {
            return TimeSpanToTimestamp((TimeSpan) to - ts);
        }


        public static bool operator ==(PreciseMonotonicTimestamp to, PreciseMonotonicTimestamp from)
        {
            return (long) to == (long) from;
        }

        public static bool operator !=(PreciseMonotonicTimestamp to, PreciseMonotonicTimestamp from)
        {
            return !(to == from);
        }

        public static bool operator >(PreciseMonotonicTimestamp to, PreciseMonotonicTimestamp from)
        {
            return (long) to > (long) from;
        }

        public static bool operator <(PreciseMonotonicTimestamp to, PreciseMonotonicTimestamp from)
        {
            return (long) to < (long) from;
        }

        public bool Equals(PreciseMonotonicTimestamp other)
        {
            return this == other;
        }

        public int CompareTo(IMonotonicTimestamp other)
        {
            if (other is PreciseMonotonicTimestamp ts) return CompareTo(ts);
            return ToMilliseconds().CompareTo(other.ToMilliseconds());
        }

        public bool Equals(IMonotonicTimestamp other)
        {
            if (other is PreciseMonotonicTimestamp ts) return Equals(ts);
            return ToMilliseconds().Equals(other.ToMilliseconds());
        }

        public long ToMilliseconds()
        {
            return Ticks / (Frequency * 1000);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is PreciseMonotonicTimestamp timestamp && Equals(timestamp);
        }

        public override int GetHashCode()
        {
            return Ticks.GetHashCode();
        }

        public int CompareTo(PreciseMonotonicTimestamp other)
        {
            return Ticks.CompareTo(other.Ticks);
        }
    }
}