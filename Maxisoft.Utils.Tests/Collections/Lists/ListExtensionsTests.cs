using System;
using System.Collections.Generic;
using Maxisoft.Utils.Collections.Lists;
using Moq;
using Troschuetz.Random;
using Xunit;

namespace Maxisoft.Utils.Tests.Collections.Lists
{
    public class ListExtensionsTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        public void Test_At_Array(int numElement)
        {
            var arr = new int[numElement];
            var numberGen = new TRandom(numElement);
            for (var i = 0; i < numElement; i++)
            {
                arr[i] = numberGen.Next();
                Assert.Equal(arr[i], arr.At(i));
            }

            // test that at use wrapped index
            // ie python like index where -1 points to the last element
            {
                for (int i = -1, r = 1; i >= -numElement; i--, r++)
                {
                    Assert.Equal(arr[^r], arr.At(i));
                }
            }

            // test out of bounds
            {
                Assert.Throws<IndexOutOfRangeException>(() => arr.At(-numElement - 1));
                Assert.Throws<IndexOutOfRangeException>(() => arr.At(-numElement - 2));
                Assert.Throws<IndexOutOfRangeException>(() => arr.At(numElement + 1));
                Assert.Throws<IndexOutOfRangeException>(() => arr.At(numElement + 2));
            }

            if (numElement <= 0)
            {
                return;
            }

            // at return a ref so one can edit the value
            {
                var index = numberGen.Next(numElement);
                var expected = numberGen.Next();
                arr.At(index) = expected;
                Assert.Equal(arr[index], arr.At(index));
                Assert.Equal(expected, arr[index]);
            }

            // same as above but with last element
            {
                var expected = numberGen.Next();
                arr.At(-1) = expected;
                Assert.Equal(arr[^1], arr.At(-1));
                Assert.Equal(expected, arr[^1]);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        public void Test_At_IList(int numElement)
        {
            var listMock = new Mock<List<int>>(numElement) {CallBase = true};
            IList<int> list = listMock.Object;

            var numberGen = new TRandom(numElement);
            for (var i = 0; i < numElement; i++)
            {
                list.Add(numberGen.Next());
                Assert.Equal(list[i], list.At(i));
            }

            // test that at use wrapped index
            // ie python like index where -1 points to the last element
            {
                for (int i = -1, r = 1; i >= -numElement; i--, r++)
                {
                    Assert.Equal(list[^r], list.At(i));
                }
            }

            // test out of bounds
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => list.At(-numElement - 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => list.At(-numElement - 2));
                Assert.Throws<ArgumentOutOfRangeException>(() => list.At(numElement + 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => list.At(numElement + 2));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        public void Test_WrapAsList(int numElement)
        {
            var array = new int[numElement];
            var list = array.WrapAsList();
            var numberGenerator = new TRandom(numElement);

            Assert.Equal(array, list);
            Assert.Same(array, list.Data());
            Assert.Equal(numElement, list.Capacity);
            Assert.Equal(numElement, list.Count);

            for (var i = 0; i < numElement; i++)
            {
                Assert.Equal(array[i], list[i]);
            }

            if (numElement <= 0)
            {
                return;
            }

            array[0] = numberGenerator.Next();
            Assert.Equal(array[0], list[0]);

            array[^1] = numberGenerator.Next();
            Assert.Equal(array[^1], list[^1]);
            Assert.Equal(array, list);

            for (var i = 0; i < numElement; i++)
            {
                list[i] = numberGenerator.Next();
                Assert.Equal(array[i], list[i]);
            }
        }
    }
}