using System;
using System.Collections.Generic;
using Maxisoft.Utils.Random;
using Xunit;

namespace Maxisoft.Utils.Tests.Random
{
    public class SkipableTimeSpanEntropyTest
    {
        private const int Iteration = 5000;
        
        [Fact]
        public void TestBasicTimeSpanEntropyBasicsWithMaxConstructor()
        {
            var max = TimeSpan.FromSeconds(1);
            var tse = new SkipableTimeSpanEntropy(max);
            
            Assert.Equal(max, tse.Max);
            Assert.Equal(TimeSpan.Zero, tse.Min);
            
            Assert.NotEqual(TimeSpan.MinValue, tse.Value);
            
            for (var i = 0; i < Iteration; i++)
            {
                var value = tse.Next();
                Assert.True(value <= max, "value <= max");
                Assert.True(value >= TimeSpan.Zero , "value >= 0");
                Assert.Equal(value, tse.Value);
                Assert.Equal(value, (TimeSpan) tse);
            }
        }
        
        [Fact]
        public void TestBasicTimeSpanEntropyBasicsWithMaxConstructorAndSkipPercentSetTo30()
        {
            var max = TimeSpan.FromSeconds(1);
            var tse = new SkipableTimeSpanEntropy(max){SkipChancePercent = 30};
            
            Assert.Equal(max, tse.Max);
            Assert.Equal(TimeSpan.Zero, tse.Min);
            
            Assert.NotEqual(TimeSpan.MinValue, tse.Value);

            var skipCount = 0;
            
            for (var i = 0; i < Iteration; i++)
            {
                var value = tse.Next();
                if (tse.IsSkipTimeStamp)
                {
                    Assert.Equal(SkipableTimeSpanEntropy.SkipValueTimeSpan, value);
                    skipCount++;
                    continue;
                }
                Assert.True(value <= max, "value <= max");
                Assert.True(value >= TimeSpan.Zero , "value >= 0");
            }
            
            Assert.True(Math.Abs(skipCount - Iteration * 30 / 100) < Iteration * 30 / 100);
        }
        
        
        [Fact]
        public void TestBasicTimeSpanEntropyBasicsWithMaxConstructorAndSkipPercentSetTo500()
        {
            const int percent = 500;
            var max = TimeSpan.FromSeconds(1);
            var tse = new SkipableTimeSpanEntropy(max){SkipChancePercent = percent};
            
            Assert.Equal(max, tse.Max);
            Assert.Equal(TimeSpan.Zero, tse.Min);
            
            Assert.NotEqual(TimeSpan.MinValue, tse.Value);

            var skipCount = 0;
            
            for (var i = 0; i < Iteration; i++)
            {
                var value = tse.Next();
                if (tse.IsSkipTimeStamp)
                {
                    Assert.Equal(SkipableTimeSpanEntropy.SkipValueTimeSpan, value);
                    skipCount++;
                    continue;
                }
                Assert.True(value <= max, "value <= max");
                Assert.True(value >= TimeSpan.Zero , "value >= 0");
            }
            
            Assert.True(Math.Abs(skipCount - Iteration * percent / 100) < Iteration * percent / 100);
        }
        
        
        [Fact]
        public void TestBasicTimeSpanEntropyBasicsWithFromPercentTrueNegative()
        {
            var @base = TimeSpan.FromSeconds(1);
            var tse = SkipableTimeSpanEntropy.FromPercent(@base, 30, allowNegative: true);
            
            Assert.True(Math.Abs((TimeSpan.FromSeconds(1 + 0.3) - tse.Max).TotalSeconds) < 0.001);
            Assert.True(Math.Abs((TimeSpan.FromSeconds(1 - 0.3) - tse.Min).TotalSeconds) < 0.001);
            
            Assert.NotEqual(TimeSpan.MinValue, tse.Value);
        }
        
        [Fact]
        public void TestBasicTimeSpanEntropyBasicsWithFromPercent500PercentTrueNegative()
        {
            var @base = TimeSpan.FromSeconds(1);
            var tse = SkipableTimeSpanEntropy.FromPercent(@base, 500, allowNegative: true);
            
            Assert.True(Math.Abs((TimeSpan.FromSeconds(1 + 5) - tse.Max).TotalSeconds) < 0.001);
            Assert.Equal(TimeSpan.Zero, tse.Min);
            
            Assert.NotEqual(TimeSpan.MinValue, tse.Value);
            
            for (var i = 0; i < Iteration; i++)
            {
                var value = tse.Next();
                if (tse.IsSkipTimeStamp)
                {
                    continue;
                }
                Assert.True(value <= tse.Max, "value <= max");
                Assert.True(value >= TimeSpan.Zero , "value >= 0");
            }
        }
        
        [Fact]
        public void TestBasicTimeSpanEntropyBasicsWithFromPercent500PercentAndSkipPercentSetTo50()
        {
            var @base = TimeSpan.FromSeconds(1);
            var tse = SkipableTimeSpanEntropy.FromPercent(@base, 500, 50, true);
            
            Assert.True(Math.Abs((TimeSpan.FromSeconds(1 + 5) - tse.Max).TotalSeconds) < 0.001);
            Assert.Equal(TimeSpan.Zero, tse.Min);
            
            Assert.NotEqual(TimeSpan.MinValue, tse.Value);
            var @default = TimeSpan.FromTicks(85121);
            
            for (var i = 0; i < Iteration; i++)
            {
                var value = tse.Next(@default);
                if (tse.IsSkipTimeStamp)
                {
                    Assert.Equal(@default, value);
                    continue;
                }
                Assert.True(value <= tse.Max, "value <= max");
                Assert.True(value >= TimeSpan.Zero , "value >= 0");
            }
        }
        
        [Fact]
        public void TestBuggyCaseWithBigNumber()
        {
            TimeSpan p = TimeSpan.FromMinutes(100);
            var tse = SkipableTimeSpanEntropy.FromPercent(p, 200, 40);
            
            for (var i = 0; i < Iteration; i++)
            {
                var value = tse.Next();
                if (tse.IsSkipTimeStamp)
                {
                    continue;
                }
                Assert.True(value <= tse.Max, "value <= max");
                Assert.True(value >= TimeSpan.Zero , "value >= 0");
            }
        }
    }
}