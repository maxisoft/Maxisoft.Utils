using System;
using System.Diagnostics;

namespace Maxisoft.Utils.Random
{
    public class BaseTimeSpanEntropy : ITimeSpanEntropy
    {
        public BaseTimeSpanEntropy(TimeSpan max) : this(TimeSpan.Zero, max)
        {
        }

        public BaseTimeSpanEntropy(TimeSpan min, TimeSpan max)
        {
            if (max < min) throw new ArgumentOutOfRangeException("", nameof(max));
            Max = max;
            Min = min;
            DoNext();
        }

        public TimeSpan Max { get; }
        public TimeSpan Min { get; }
        public TimeSpan Value { get; protected set; } = TimeSpan.MinValue;

        internal TimeSpan DoNext()
        {
            var ri = Random.NextDouble() * (Max.Ticks - Min.Ticks) + Min.Ticks;

            Value = TimeSpan.FromTicks((long) ri);
            Debug.Assert(Value >= Min);
            Debug.Assert(Value <= Max);
            return Value;
        }

        public virtual TimeSpan Next()
        {
            return DoNext();
        }

        protected static readonly RandomThreadSafe Random = new RandomThreadSafe();

        public static implicit operator TimeSpan(BaseTimeSpanEntropy tse) => tse.Value;

        public static BaseTimeSpanEntropy FromPercent(TimeSpan @base, float percent, bool allowNegative = false)
        {
            if (percent < 0)
            {
                throw new ArgumentOutOfRangeException("", nameof(percent));
            }
            if (@base.Ticks <= 0)
            {
                throw new ArgumentOutOfRangeException("", nameof(@base));
            }
            var min = allowNegative ? @base - TimeSpan.FromTicks((long) (@base.Ticks * percent / 100)) : @base;
            min = TimeSpan.FromTicks(Math.Max(0, min.Ticks));
            var max = @base + TimeSpan.FromTicks((long) (@base.Ticks * percent / 100));
            return new BaseTimeSpanEntropy(min, max);
        }
    }
}