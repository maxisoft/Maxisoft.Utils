using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Maxisoft.Utils.Collections;
using Maxisoft.Utils.Collections.Lists;
using Maxisoft.Utils.Collections.Queues;
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
        
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        public void Test_TryPop(int numElement)
        {
            var array = new ArrayList<int>(numElement);

            var numberGenerator = new TRandom(numElement);
            var shift = numberGenerator.Next();

            for (var i = 0; i < numElement; i++)
            {
                array.Add(i + shift);
            }
            
            var originalArray = array.ToImmutableArray();

            if (numElement > 0)
            {
                {
                    Assert.True(array.TryPop(0, out var tmp));
                    Assert.Equal(0 + shift, tmp);
                    array.Insert(0, tmp);
                    Assert.Equal(originalArray, array);
                }
                

                for (var i = numElement - 1; i >= 0; i--)
                {
                    Assert.True(array.TryPop(i, out var tmp));
                    Assert.Equal(i + shift, tmp);
                    array.Insert(i, tmp);
                    Assert.Equal(originalArray, array);
                }
                
                var q = new Deque<int>();
                for (var i = 0; i < numElement; i++)
                {
                    Assert.True(array.TryPop(out var tmp));
                    q.PushFront(tmp);
                    Assert.Equal(array.Count + shift, tmp);
                }
               
                Assert.Empty(array);
                Assert.False(array.TryPop(out _));
                
                array.AddRange(q);
                Assert.Equal(originalArray, array);
            }
            else
            {
                Assert.False(array.TryPop(0, out _));
                Assert.False(array.TryPop(out _));
            }
            Assert.Equal(originalArray, array);
            
            Assert.False(array.TryPop(-1, out _));
            Assert.False(array.TryPop(-2, out _));
            Assert.False(array.TryPop(numElement + 1, out _));
            Assert.False(array.TryPop(numElement, out _));
        
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        public void Test_Pop(int numElement)
        {
            var array = new ArrayList<int>(numElement);

            var numberGenerator = new TRandom(numElement);
            var shift = numberGenerator.Next();

            for (var i = 0; i < numElement; i++)
            {
                array.Add(i + shift);
            }
            
            var originalArray = array.ToImmutableArray();

            if (numElement > 0)
            {
                {
                    var tmp = array.Pop(0);
                    Assert.Equal(0 + shift, tmp);
                    array.Insert(0, tmp);
                    Assert.Equal(originalArray, array);
                }


                for (var i = numElement - 1; i >= 0; i--)
                {
                    var tmp = array.Pop(i);
                    Assert.Equal(i + shift, tmp);
                    array.Insert(i, tmp);
                    Assert.Equal(originalArray, array);
                }

                var q = new Deque<int>();
                for (var i = 0; i < numElement; i++)
                {
                    var tmp = array.Pop();
                    q.PushFront(tmp);
                    Assert.Equal(array.Count + shift, tmp);
                }

                Assert.Empty(array);
                Assert.Throws<ArgumentOutOfRangeException>(() => array.Pop());

                array.AddRange(q);
                Assert.Equal(originalArray, array);
            }
            else
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => array.Pop(0));
                Assert.Throws<ArgumentOutOfRangeException>(() => array.Pop());
            }

            Assert.Equal(originalArray, array);

            Assert.Throws<ArgumentOutOfRangeException>(() => array.Pop(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => array.Pop(-2));
            Assert.Throws<ArgumentOutOfRangeException>(() => array.Pop(numElement + 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => array.Pop(numElement));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(32)]
        [InlineData(75)]
        [InlineData(97)]
        [InlineData(417)]
        public void Test_AddSorted_List(int numElement)
        {
            var array = new List<int>(numElement);

            var numberGenerator = new TRandom(numElement);

            for (var i = 0; i < numElement; i++)
            {
                array.Add(numberGenerator.Next(numElement));
            }
            
            array.Sort();

            var item = numberGenerator.Next(numElement);
            var index = array.AddSorted(item);
            Assert.Equal(numElement + 1, array.Count);
            Assert.Equal(item, array[index]);
            var copy = array.ToImmutableArray();
            array.Sort();
            Assert.Equal(copy, array);
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(32)]
        [InlineData(75)]
        [InlineData(97)]
        [InlineData(417)]
        public void Test_AddSorted_ArrayList(int numElement)
        {
            var array = new ArrayList<int>(numElement);

            var numberGenerator = new TRandom(numElement);

            for (var i = 0; i < numElement; i++)
            {
                array.Add(numberGenerator.Next(numElement));
            }
            
            array.Sort();

            var item = numberGenerator.Next(numElement);
            var index = array.AddSorted(item);
            Assert.Equal(numElement + 1, array.Count);
            Assert.Equal(item, array[index]);
            var copy = array.ToImmutableArray();
            array.Sort();
            Assert.Equal(copy, array);
        }
    }
}