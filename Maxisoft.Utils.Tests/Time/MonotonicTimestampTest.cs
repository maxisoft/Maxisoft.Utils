using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Maxisoft.Utils.Time;
using Xunit;

namespace Maxisoft.Utils.Tests.Time
{
    public class MonotonicTimestampTest
    {
        [Fact]
        [Trait("Category", "Win32")]
        public async Task TestMonotonicBasic()
        {
            var sw = Stopwatch.StartNew();
            var m1 = MonotonicTimestamp.Now;
            await Task.Delay(TimeSpan.FromSeconds(1));
            var m2 = MonotonicTimestamp.Now;
            sw.Stop();

            Assert.NotStrictEqual(m1, m2);
            Assert.StrictEqual(m2, m2);

            Assert.NotEqual(m1, m2);
            Assert.Equal(m1, m1);

            Assert.True(m1 < m2);
            Assert.True(m2 > m1);

            Assert.True(m2.CompareTo(m1) > 0);
            Assert.True(m1.CompareTo(m2) < 0);
            Assert.True(m1.CompareTo(m1) == 0);

            Assert.NotEqual(m2.GetHashCode(), m1.GetHashCode());
            Assert.Equal(m2.GetHashCode(), m2.GetHashCode());

            Assert.True(((m1 + sw.Elapsed) - m2).Duration() <=
                        TimeSpan.FromTicks(MonotonicTimestamp.Precision.Ticks * 3));
            Assert.True(((m2 - sw.Elapsed) - m1).Duration() <=
                        TimeSpan.FromTicks(MonotonicTimestamp.Precision.Ticks * 3));
            Assert.Equal(m2 + m1, m1 + m2);
        }

        [Fact]
        [Trait("Category", "Win32")]
        public void TestMonotonicBasicDefaultContructor_MustThrow()
        {
            var m1 = new MonotonicTimestamp();
            Assert.Throws<ArgumentException>(() => m1 == m1);
            Assert.Throws<ArgumentException>(() => m1 > m1);
            Assert.Throws<ArgumentException>(() => m1 < m1);
            Assert.Throws<ArgumentException>(() => m1 != m1);
            Assert.Throws<ArgumentException>(() => m1 + TimeSpan.FromMilliseconds(1));
        }
    }
}