using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Maxisoft.Utils.Algorithms;
using Maxisoft.Utils.Collections;
using Xunit;

namespace Maxisoft.Utils.Tests.Collections
{
    public class EnumerableExtensionsTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(64)]
        public void Test_ToDeque(int count)
        {
            var enumerable = Enumerable.Range(0, count).Select(Fibonacci.Fibo).ToImmutableList();
            var res = enumerable.ToDeque();
            Assert.Equal(enumerable, res);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(64)]
        public void Test_ToLinkedList(int count)
        {
            var enumerable = Enumerable.Range(0, count).Select(Fibonacci.Fibo).ToImmutableList();
            var res = enumerable.ToLinkedList();
            Assert.Equal(enumerable, res);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(64)]
        public void Test_ToArrayList(int count)
        {
            var enumerable = Enumerable.Range(0, count).Select(Fibonacci.Fibo).ToImmutableList();
            Assert.Equal(enumerable, ((IReadOnlyCollection<long>) enumerable).ToArrayList());
            Assert.Equal(enumerable, ((ICollection<long>) enumerable).ToArrayList());
            Assert.Equal(enumerable, ((IEnumerable<long>) enumerable).ToArrayList());
            var arr = enumerable.ToArray();
            Assert.Equal(enumerable, arr.ToArrayList());
            if (count > 0)
            {
                Assert.Same(arr, arr.ToArrayList(copy: false).Data());
                Assert.NotSame(arr, arr.ToArrayList(copy: true).Data());
            }

            Assert.Equal(enumerable, enumerable.ToArray().ToArrayList());
        }
    }
}