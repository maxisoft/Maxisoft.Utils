using System;
using System.Collections.Generic;
using System.Linq;
using Maxisoft.Utils.Algorithm;
using Xunit;
using Xunit.Abstractions;

namespace Maxisoft.Utils.Tests.Algorithm
{
    public class FibonacciTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public FibonacciTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory, MemberData(nameof(FiboData))]
        public void TestFibonacciPreComputed(int rank, int expected)
        {
            Assert.Equal(expected, Fibonacci.Fibo(rank));
        }

        [Theory, MemberData(nameof(FiboData))]
        public void TestFibonacci(int rank, int expected)
        {
            Assert.Equal(expected, Fibonacci.Compute(rank));
        }

        [Theory, MemberData(nameof(FiboData))]
        public void TestFibonacciImprecise(int rank, int expected)
        {
            Assert.Equal(expected, Fibonacci.ComputeImprecise(rank));
        }

        [Fact]
        public void TestOverflow()
        {
            Assert.Throws<OverflowException>(() => Fibonacci.Compute(Fibonacci.PreComputed.Length + 1));
        }

        [Fact]
        public void TestComputeEqualsComputeImprecise()
        {
            for (var i = 0; i < Fibonacci.PreComputed.Length; i++)
            {
                var precise = Fibonacci.Compute(i);
                var approx = Fibonacci.ComputeImprecise(i);
                if (precise != (long) approx)
                {
                    _testOutputHelper.WriteLine($"{precise} != {approx}");
                }

                if (precise == 0)
                {
                    Assert.Equal(0, approx);
                }
                else
                {
                    Assert.True(Math.Abs(precise - (long) approx) / (0.5 * (precise + approx)) < 1e-9);
                }
            }
        }

        [Fact]
        public void TestPreComputed()
        {
            for (var i = 0; i < Fibonacci.PreComputed.Length; i++)
            {
                var expected = Fibonacci.Compute(i);
                var precomputed = Fibonacci.PreComputed[i];
                Assert.Equal(expected, precomputed);
            }
        }

        [Fact]
        public void TestSqrt5()
        {
            Assert.Equal(Math.Sqrt(5), Fibonacci.Sqrt5);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<object[]> FiboData => new[]
        {
            0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765
        }.Select((i, pos) => new object[] {pos, i});
    }
}