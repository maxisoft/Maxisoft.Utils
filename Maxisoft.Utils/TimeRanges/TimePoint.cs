using System;

namespace Maxisoft.Utils.TimeRanges
{
    public struct TimePoint : IEquatable<TimePoint>, IComparable<TimePoint>, IComparable
    {
        public const int MinuteToSecond = 60;
        public const int HoursToSecond = MinuteToSecond * 24;
        
        public readonly int Hours;
        public readonly int Minutes;
        public readonly int Seconds;

        public TimePoint(int h = 0, int m = 0, int s = 0)
        {
            if (!ValidHour(h)) throw new ArgumentOutOfRangeException(nameof(h), h, "must be in (0,24)");
            Hours = h;
            if (!ValidMinute(m)) throw new ArgumentOutOfRangeException(nameof(m), m, "must be in (0,60)");
            Minutes = m;
            if (!ValidSecond(s)) throw new ArgumentOutOfRangeException(nameof(s), s, "must be in (0,60)");
            Seconds = s;
        }

        public TimePoint(TimeSpan ts) : this((long) ts.TotalSeconds, true)
        {
        }

        internal TimePoint(long second, bool @infer)
        {
            if (second < 0) throw new ArgumentOutOfRangeException(nameof(second), second, "must be > 0");
            if (second >= 24 * HoursToSecond) throw new ArgumentOutOfRangeException(nameof(second), second, "must < " + (24 * HoursToSecond));
            Hours = (int) (second / HoursToSecond);
            Minutes = (int) ((second - Hours * HoursToSecond) / MinuteToSecond);
            Seconds = (int) (second - Hours * HoursToSecond - Minutes * MinuteToSecond);
        }

        public TimePoint(DateTime dateTime) : this(dateTime.Hour, dateTime.Minute, dateTime.Second)
        {
            
        }

        public static TimePoint FromSeconds(double second)
        {
            return FromSeconds((long) second);
        }

        public static bool TryParse(string playload, out TimePoint timePoint) =>
            Parser.TryParse(playload, out timePoint);
        
        public static TimePoint FromSeconds(long second)
        {
            return new TimePoint(second, true);
        }
        
        public static bool ValidSecond(int s)
        {
            return 0 <= s && s < 60;
        }

        public static bool ValidMinute(int m)
        {
            return 0 <= m && m < 60;
        }

        public static bool ValidHour(int h)
        {
            return 0 <= h && h < 24;
        }

        public static readonly TimePoint Zero = default(TimePoint);
        public static readonly TimePoint Unit = new TimePoint(1, true);
        
        public bool Equals(TimePoint other)
        {
            return Hours == other.Hours && Minutes == other.Minutes && Seconds == other.Seconds;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TimePoint && Equals((TimePoint) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Hours;
                hashCode = (hashCode * 397) ^ Minutes;
                hashCode = (hashCode * 397) ^ Seconds;
                return hashCode;
            }
        }

        public static bool operator ==(TimePoint left, TimePoint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TimePoint left, TimePoint right)
        {
            return !left.Equals(right);
        }
        
        public int CompareTo(TimePoint other)
        {
            var hoursComparison = Hours.CompareTo(other.Hours);
            if (hoursComparison != 0) return hoursComparison;
            var minutesComparison = Minutes.CompareTo(other.Minutes);
            if (minutesComparison != 0) return minutesComparison;
            return Seconds.CompareTo(other.Seconds);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            if (!(obj is TimePoint)) throw new ArgumentException($"Object must be of type {nameof(TimePoint)}");
            return CompareTo((TimePoint) obj);
        }

        public static bool operator <(TimePoint left, TimePoint right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(TimePoint left, TimePoint right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(TimePoint left, TimePoint right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(TimePoint left, TimePoint right)
        {
            return left.CompareTo(right) >= 0;
        }
        
        public static TimePoint operator +(TimePoint left, TimePoint right)
        {
            return TimePoint.FromSeconds(left.TotalSeconds + right.TotalSeconds);
        }
        
        public static TimePoint operator -(TimePoint left, TimePoint right)
        {
            return TimePoint.FromSeconds(left.TotalSeconds - right.TotalSeconds);
        }
        
        public static TimePoint operator +(TimePoint left, TimeSpan right)
        {
            return TimePoint.FromSeconds(left.TotalSeconds + right.TotalSeconds);
        }
        
        public static TimePoint operator +(TimePoint left, long right)
        {
            return TimePoint.FromSeconds(left.TotalSeconds + right);
        }
        
        public static TimePoint operator +(TimePoint left, double right)
        {
            return TimePoint.FromSeconds(left.TotalSeconds + right);
        }

        public static explicit operator long(TimePoint tp)
        {
            return tp.TotalSeconds;
        }

        public long TotalSeconds => (long) Hours * HoursToSecond + Minutes * MinuteToSecond + Seconds;

        public static explicit operator TimeSpan(TimePoint tp)
        {
            return TimeSpan.FromSeconds(tp.TotalSeconds);
        }

        public override string ToString()
        {
            return $"TimePoint({nameof(Hours)}: {Hours}, {nameof(Minutes)}: {Minutes}, {nameof(Seconds)}: {Seconds})";
        }
    }
}