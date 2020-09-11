using System;
using System.Diagnostics;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using Maxisoft.Utils.Time;
using Xunit;
using Xunit.Sdk;

namespace Maxisoft.Utils.Tests.Time
{
    public class PreciseMonotonicTimestampTest
    {
        [Fact]
        public async Task TestMonotonicBasic()
        {
            var sw = Stopwatch.StartNew();
            var m1 = PreciseMonotonicTimestamp.Now;
            await Task.Delay(TimeSpan.FromSeconds(Stopwatch.IsHighResolution ? 0.1 : 0.3));
            var m2 = PreciseMonotonicTimestamp.Now;
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
            Assert.Equal(m2 + m1, m1 + m2);
        }

        [SkippableTheory]
        [InlineData(5, 10, true)]
        [InlineData(50, 5, true)]
        [InlineData(300, 2, false)]
        public async Task TestMonotonic_Precision(double maxTimeDiffMilli, int tryAllowed, bool isSkippable)
        {
            var tp = Thread.CurrentThread.Priority;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            var maxTimeDiff = TimeSpan.FromMilliseconds(maxTimeDiffMilli * (Stopwatch.IsHighResolution ? 1 : 2));
            try
            {
                for (var @try = 0; @try < tryAllowed; @try++)
                {
                    var sw = Stopwatch.StartNew();
                    var m1 = PreciseMonotonicTimestamp.Now;
                    await Task.Delay(TimeSpan.FromMilliseconds(300));
                    var m2 = PreciseMonotonicTimestamp.Now;
                    sw.Stop();
                    try
                    {
                        Assert.True(((m2 - m1 - sw.Elapsed).Duration() <= maxTimeDiff));
                    }
                    catch (XunitException) when (@try < tryAllowed - 1)
                    {
                        continue;
                    }
                }
            }
            catch (XunitException)
            {
                Skip.If(isSkippable);
                throw;
            }
            finally
            {
                Thread.CurrentThread.Priority = tp;
            }
        }
    }
}