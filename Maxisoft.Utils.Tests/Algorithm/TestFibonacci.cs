using System.Collections.Generic;
using System.Linq;
using Maxisoft.Utils.Algorithm;
using Xunit;

namespace Maxisoft.Utils.Tests.Algorithm
{
    public class TestFibonacci
    {
        [Theory, MemberData(nameof(SplitCountData))]
        public void TestFibonnaci(int rank, int expected)
        {
            Assert.Equal(expected, Fibonacci.Fibo(rank));
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<object[]> SplitCountData => new[]
        {
            0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765
        }.Select((i, pos) => new object[] {pos, i});
    }
}