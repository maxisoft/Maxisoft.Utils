using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using Maxisoft.Utils.Random;
using Maxisoft.Utils.Time;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Maxisoft.Utils.Tests.Time
{
    public class MonotonicTimestampTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public MonotonicTimestampTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task TestMonotonicBasic()
        {
            var sw = Stopwatch.StartNew();
            var m1 = MonotonicTimestamp.Now;
            await Task.Delay(TimeSpan.FromSeconds(Stopwatch.IsHighResolution ? 0.1 : 0.3));
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
                    var m1 = MonotonicTimestamp.Now;
                    await Task.Delay(TimeSpan.FromMilliseconds(300));
                    var m2 = MonotonicTimestamp.Now;
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

        internal struct MockedEnviron : MonotonicTimestamp.IEnvironment
        {
            internal static SemaphoreSlim Semaphore { get; } = new SemaphoreSlim(1, 1);

            internal static int TickCountValue
            {
                get => _tickCountValue.Value;
                set => _tickCountValue.Value = value;
            }

            private static readonly ThreadLocal<int> _tickCountValue = new ThreadLocal<int>();
            public int TickCount => TickCountValue;
        }

        [Fact]
        public void TestTickCount_DefaultEnviron()
        {
            var now = MonotonicTimestamp.TickCountProvider<MonotonicTimestamp.DefaultEnvironment>.Now;
            var tc = Environment.TickCount;
            Assert.True(Math.Abs(now.TickCount() - tc) < 20);
        }

        [Fact]
        public async Task TestTickCount_MockEnviron()
        {
            await MockedEnviron.Semaphore.WaitAsync();
            try
            {
                MockedEnviron.TickCountValue = 0;
                var now = MonotonicTimestamp.TickCountProvider<MockedEnviron>.Now;
                Assert.Equal(0L, now.TickCount());

                MockedEnviron.TickCountValue = 0;

                for (var i = 0; i < 16; i++)
                {
                    now.Overflow = i;
                    Assert.Equal((long) i << 32, now.TickCount());
                }


                for (var i = 0; i < 16; i++)
                {
                    now.Overflow = i;
                    Assert.Equal((long) i << 32, now.TickCount());
                }

                for (var i = 0; i < 16; i++)
                {
                    now.Overflow = i;
                    Assert.Equal((long) i << 32, now.TickCount());
                }

                MockedEnviron.TickCountValue = -1;
                now.Overflow = 0;
                Assert.Equal((1L << 32) - 1, now.TickCount());
            }
            finally
            {
                MockedEnviron.Semaphore.Release();
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData(-2)]
        [InlineData(int.MinValue)]
        [InlineData(int.MinValue + 1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MaxValue - 1)]
        public async Task TestTickCount_MockEnviron_Fuzz(int initialTick)
        {
            var random = new RandomThreadSafe();
            await MockedEnviron.Semaphore.WaitAsync();
            try
            {
                MockedEnviron.TickCountValue = initialTick;
                var provider = MonotonicTimestamp.TickCountProvider<MockedEnviron>.Now;
                var initialTickCount = provider.TickCount();
                Assert.Equal((uint) initialTick, initialTickCount);
                long prevTickCount = provider.TickCount();
                var numOperations = random.Next(2048) + 1;
                var history = new List<TimeSpan>();
                var overflowCounter = 0;
                for (var i = 0; i < numOperations; i++)
                {
                    var add = random.Next();

                    var addTimeSpan = TimeSpan.FromMilliseconds(add);
                    history.Add(addTimeSpan);
                    unchecked
                    {
                        var old = MockedEnviron.TickCountValue;
                        Assert.True(add >= 0);
                        MockedEnviron.TickCountValue += add;
                        if (MockedEnviron.TickCountValue < old)
                        {
                            overflowCounter += 1;
                        }
                    }
                    var actual = provider.TickCount();
                    Assert.True(actual >= initialTickCount);
                    try
                    {
                        Assert.True(actual >= prevTickCount);
                    }
                    catch (XunitException)
                    {
                        _testOutputHelper.WriteLine(history.Count.ToString());
                        _testOutputHelper.WriteLine(string.Join(", ", history.ToArray()));
                        _testOutputHelper.WriteLine(overflowCounter.ToString());
                        throw;
                    }

                    Assert.Equal(actual - prevTickCount, add);
                    prevTickCount = actual;
                }
            }
            finally
            {
                MockedEnviron.Semaphore.Release();
            }
        }
    }
}