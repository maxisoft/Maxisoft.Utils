using System;
using System.Collections.Generic;
using Maxisoft.Utils.Random;
using Xunit;

namespace Maxisoft.Utils.Tests.Random
{
    public class BaseTimeSpanEntropyTest
    {
        private const int Iteration = 5000;
        
        [Fact]
        public void TestBasicTimeSpanEntropyBasicsWithMaxConstructor()
        {
            var max = TimeSpan.FromSeconds(1);
            var tse = new BaseTimeSpanEntropy(max);
            
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
        public void TestBasicTimeSpanEntropyBasicsWithMaxAndMinConstructor()
        {
            var max = TimeSpan.FromSeconds(1);
            var min = TimeSpan.FromSeconds(0.3);
            var tse = new BaseTimeSpanEntropy(min, max);
            
            Assert.Equal(max, tse.Max);
            Assert.Equal(min, tse.Min);
            
            Assert.NotEqual(TimeSpan.MinValue, tse.Value);
            
            for (var i = 0; i < Iteration; i++)
            {
                var value = tse.Next();
                Assert.True(value <= max, "value <= max");
                Assert.True(value >= min , "value >= min");
            }
        }
        
        [Fact]
        public void TestBasicTimeSpanEntropyBasicsWithFromPercent()
        {
            var @base = TimeSpan.FromSeconds(1);
            var tse = BaseTimeSpanEntropy.FromPercent(@base, 30);
            
            Assert.True(Math.Abs((TimeSpan.FromSeconds(1 + 0.3) - tse.Max).TotalSeconds) < 0.001);
            Assert.Equal(@base, tse.Min);
            
            Assert.NotEqual(TimeSpan.MinValue, tse.Value);
        }
        
        [Fact]
        public void TestBasicTimeSpanEntropyBasicsWithFromPercentTrueNegative()
        {
            var @base = TimeSpan.FromSeconds(1);
            var tse = BaseTimeSpanEntropy.FromPercent(@base, 30, true);
            
            Assert.True(Math.Abs((TimeSpan.FromSeconds(1 + 0.3) - tse.Max).TotalSeconds) < 0.001);
            Assert.True(Math.Abs((TimeSpan.FromSeconds(1 - 0.3) - tse.Min).TotalSeconds) < 0.001);
            
            Assert.NotEqual(TimeSpan.MinValue, tse.Value);
        }
    }
}