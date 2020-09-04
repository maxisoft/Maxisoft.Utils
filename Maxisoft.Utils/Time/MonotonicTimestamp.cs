using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Maxisoft.Utils.Time
{
    public struct MonotonicTimestamp : IComparable<MonotonicTimestamp>, IEquatable<MonotonicTimestamp>
    {
        private readonly ulong _timestamp;

        public static readonly MonotonicTimestamp MinValue = new MonotonicTimestamp(1);
        public static readonly MonotonicTimestamp MaxValue = new MonotonicTimestamp(ulong.MaxValue);
        public static readonly TimeSpan Precision = TimeSpan.FromMilliseconds(64);

        public MonotonicTimestamp(long timestamp) : this((ulong)timestamp)
        {
            
        }
        
        public MonotonicTimestamp(ulong timestamp)
        {
            _timestamp = timestamp;
        }

        public bool IsZero => _timestamp == 0;
        
        public static MonotonicTimestamp Now => new MonotonicTimestamp(Native.GetTickCount64());

        public static MonotonicTimestamp FromLuaTime(double time)
        {
            return new MonotonicTimestamp((ulong) (time * 1000));
        }

        public static explicit operator ulong(MonotonicTimestamp t)
        {
            return t._timestamp;
        }

        public static TimeSpan operator +(MonotonicTimestamp to, MonotonicTimestamp from)
        {
            return TimeSpan.FromMilliseconds((ulong) to + (ulong) from);
        }

        public static TimeSpan operator -(MonotonicTimestamp to, MonotonicTimestamp from)
        {
            return TimeSpan.FromMilliseconds((double) (new BigInteger((ulong) to) - new BigInteger((ulong) from)));
        }

        public static MonotonicTimestamp operator +(MonotonicTimestamp to, TimeSpan ts)
        {
            return new MonotonicTimestamp((ulong) ((ulong) to + ts.TotalMilliseconds));
        }

        public static MonotonicTimestamp operator -(MonotonicTimestamp to, TimeSpan ts)
        {
            return new MonotonicTimestamp((ulong) ((ulong) to - ts.TotalMilliseconds));
        }


        public static bool operator ==(MonotonicTimestamp to, MonotonicTimestamp from)
        {
            return (ulong) to == (ulong) from;
        }

        public static bool operator !=(MonotonicTimestamp to, MonotonicTimestamp from)
        {
            return !(to == from);
        }

        public static bool operator >(MonotonicTimestamp to, MonotonicTimestamp from)
        {
            return (ulong) to > (ulong) from;
        }

        public static bool operator <(MonotonicTimestamp to, MonotonicTimestamp from)
        {
            return (ulong) to < (ulong) from;
        }

        public bool Equals(MonotonicTimestamp other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is MonotonicTimestamp && Equals((MonotonicTimestamp) obj);
        }

        public override int GetHashCode()
        {
            return _timestamp.GetHashCode();
        }

        public int CompareTo(MonotonicTimestamp other)
        {
            return _timestamp.CompareTo(other._timestamp);
        }

        private static class Native
        {
            [DllImport("kernel32.dll")]
            public static extern ulong GetTickCount64();
        }
    }
}