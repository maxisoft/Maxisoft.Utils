using System;

namespace Maxisoft.Utils.Random
{
    public class SkipableTimeSpanEntropy : BaseTimeSpanEntropy
    {
        public static readonly TimeSpan SkipValueTimeSpan = TimeSpan.Zero;

        public float SkipChancePercent { get; set; } = 0;

        public SkipableTimeSpanEntropy(TimeSpan max) : base(max)
        {
            DoNext();
        }

        public SkipableTimeSpanEntropy(TimeSpan min, TimeSpan max) : base(min, max)
        {
            DoNext();
        }

        private bool ShouldSkip()
        {
            if (Math.Abs(SkipChancePercent) < 0.001)
            {
                return false;
            }
            if (SkipChancePercent > 100)
            {
                return true;
            }

            return Random.NextDouble() * 100 < SkipChancePercent;
        }

        internal new TimeSpan DoNext()
        {
            if (ShouldSkip())
            {
                Value = SkipValueTimeSpan;
                return Value;
            }
            return base.Next();
        }

        public override TimeSpan Next()
        {
            return DoNext();
        }

        public TimeSpan Next(TimeSpan @default)
        {
            var ret = DoNext();
            return IsSkipTimeStamp ? @default : ret;
        }

        public TimeSpan Next(Func<TimeSpan> @default)
        {
            var ret = DoNext();
            return IsSkipTimeStamp ? @default() : ret;
        }

        public bool IsSkipTimeStamp => Value == SkipValueTimeSpan;

        public new static SkipableTimeSpanEntropy FromPercent(TimeSpan @base, float percent,
            bool allowNegative = false)
        {
            return FromPercent(@base, percent, 0, allowNegative: allowNegative);
        }

        public static SkipableTimeSpanEntropy FromPercent(TimeSpan @base, float percent,
            float skipChancePercent = 0,
            bool allowNegative = false)
        {
            var b = BaseTimeSpanEntropy.FromPercent(@base, percent, allowNegative);
            return new SkipableTimeSpanEntropy(b.Min, b.Max)
            {
                SkipChancePercent = skipChancePercent
            };
        }
    }
}