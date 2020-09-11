using System;
using System.Diagnostics;

namespace Maxisoft.Utils.Time
{
    [DebuggerDisplay(nameof(Ticks) + " = {" + nameof(Ticks) + "}," + nameof(Frequency) + " = {" + nameof(Frequency) +
                     "}")]
    public readonly partial struct MonotonicTimestamp : IMonotonicTimestamp, IComparable<MonotonicTimestamp>,
        IEquatable<MonotonicTimestamp>
    {
        public readonly long Ticks;
        public static long Frequency => 1000;

        public MonotonicTimestamp(long timestamp)
        {
            Ticks = timestamp;
        }

        public bool IsZero => Ticks == 0;

        private static TickCountProvider<DefaultEnvironment> _tickCountProvider =
            TickCountProvider<DefaultEnvironment>.Now;

        public static MonotonicTimestamp Now => new MonotonicTimestamp(_tickCountProvider.TickCount());


        public static explicit operator long(MonotonicTimestamp t)
        {
            return t.Ticks;
        }

        public static explicit operator TimeSpan(MonotonicTimestamp t)
        {
            return TimeSpan.FromMilliseconds(t.ToMilliseconds());
        }

        public static TimeSpan operator +(MonotonicTimestamp to, MonotonicTimestamp from)
        {
            return (TimeSpan) to + (TimeSpan) from;
        }

        public static TimeSpan operator -(MonotonicTimestamp to, MonotonicTimestamp from)
        {
            return (TimeSpan) to - (TimeSpan) from;
        }

        private static MonotonicTimestamp TimeSpanToTimestamp(TimeSpan ts)
        {
            return new MonotonicTimestamp((long) (ts.TotalSeconds * Frequency));
        }

        public static MonotonicTimestamp operator +(MonotonicTimestamp to, TimeSpan ts)
        {
            return TimeSpanToTimestamp((TimeSpan) to + ts);
        }

        public static MonotonicTimestamp operator -(MonotonicTimestamp to, TimeSpan ts)
        {
            return TimeSpanToTimestamp((TimeSpan) to - ts);
        }


        public static bool operator ==(MonotonicTimestamp to, MonotonicTimestamp from)
        {
            return (long) to == (long) from;
        }

        public static bool operator !=(MonotonicTimestamp to, MonotonicTimestamp from)
        {
            return !(to == from);
        }

        public static bool operator >(MonotonicTimestamp to, MonotonicTimestamp from)
        {
            return (long) to > (long) from;
        }

        public static bool operator <(MonotonicTimestamp to, MonotonicTimestamp from)
        {
            return (long) to < (long) from;
        }

        public bool Equals(MonotonicTimestamp other)
        {
            return this == other;
        }

        public long ToMilliseconds()
        {
            return Ticks;
        }

        public int CompareTo(IMonotonicTimestamp other)
        {
            if (other is MonotonicTimestamp ts)
            {
                return CompareTo(ts);
            }

            return ToMilliseconds().CompareTo(other.ToMilliseconds());
        }

        public bool Equals(IMonotonicTimestamp other)
        {
            if (other is MonotonicTimestamp ts)
            {
                return Equals(ts);
            }

            return ToMilliseconds().Equals(other.ToMilliseconds());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is MonotonicTimestamp timestamp && Equals(timestamp);
        }

        public override int GetHashCode()
        {
            return typeof(MonotonicTimestamp).GetHashCode() ^ Ticks.GetHashCode();
        }

        public int CompareTo(MonotonicTimestamp other)
        {
            return Ticks.CompareTo(other.Ticks);
        }

        public override string ToString()
        {
            return $"Monotonic({(TimeSpan) this})";
        }
    }
}