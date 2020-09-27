using System;
using System.Linq;
using Maxisoft.Utils.Collections.Lists;
using Moq;
using Xunit;
using Xunit.Sdk;
using Range = Moq.Range;

namespace Maxisoft.Utils.Tests.Collections.Lists
{
    public class ArrayListTests
    {
        [Fact]
        public void Test_Default_Constructor_Doesnt_Allocate()
        {
            var listMock = new Mock<ArrayList<int>> {CallBase = true};
            var list = listMock.Object;
            listMock.VerifyNoOtherCalls(); // as Alloc / Realloc methods are virtual the mock should catch them 
            Assert.Same(ArrayList<int>.EmptyArray, list.Data());
            Assert.Equal(0, list.Capacity);
            Assert.Empty(list);
        }

        [Fact]
        public void Test_Constructor_With_Array()
        {
            var arr = Enumerable.Range(0, 5).ToArray();
            var listMock = new Mock<ArrayList<int>>(arr) {CallBase = true};
            var list = listMock.Object;
            listMock.VerifyNoOtherCalls(); // => no allocation
            Assert.Same(arr, list.Data());
            Assert.Equal(arr.Length, list.Count);
            Assert.Equal(arr.Length, list.Capacity);
            Assert.Equal(Enumerable.Range(0, 5), list);

            list.ShrinkToFit();
            Assert.Same(arr, list.Data());

            //one can remove element within the arr via the list
            {
                list.RemoveAt(0);
                list.RemoveAt(list.Count - 1);
                var expected = Enumerable.Range(1, 3).ToArray();
                Assert.Equal(expected, list);
                Assert.Equal(expected.Length, list.Count);
                Assert.Equal(arr.Length, list.Capacity);
                Assert.Same(arr, list.Data());
                Assert.Equal(new[] {1, 2, 3, 4, 4}, arr);
            }

            list.Resize(arr.Length);
            Assert.Same(arr, list.Data());

            // any changes made to the arr are directly reflected in the list
            {
                arr[^1] = 5;
                Assert.Equal(5, list.Back());
                Assert.Equal(5, list[^1]);
                Assert.Equal(5, list.At(list.Count - 1));

                for (var i = 0; i < arr.Length; i++)
                {
                    Assert.Equal(arr[i], list[i]);
                    Assert.Equal(arr[i], list.At(i));
                }
            }

            listMock.VerifyNoOtherCalls(); // => no allocation

            // as soon as one add more elements than the list capacity could handle the array is detached from the list
            {
                list.Add(6);
                Assert.NotSame(arr, list.Data());
                Assert.Equal(new[] {1, 2, 3, 4, 5, 6}, list);
                listMock.Verify(mock => mock.ReAlloc(ref It.Ref<int[]>.IsAny, It.IsAny<int>(), It.IsAny<int>()),
                    Times.Once);
                listMock.Verify(mock => mock.ComputeGrowSize(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                listMock.Verify(mock => mock.Free(It.IsAny<int[]>()), Times.Once);
                listMock.VerifyNoOtherCalls();
            }
        }

        [Fact]
        public void Test_Constructor_Capacity()
        {
            const int capacity = 10;
            var listMock = new Mock<ArrayList<int>>(capacity) {CallBase = true};
            var list = listMock.Object;

            listMock.Verify(mock => mock.ReAlloc(ref It.Ref<int[]>.IsAny, It.IsAny<int>(), capacity), Times.Once);
            listMock.Verify(mock => mock.Alloc(capacity), Times.Once);
            listMock.VerifyNoOtherCalls();
            Assert.Equal(capacity, list.Capacity);
            Assert.Empty(list);
            var arr = list.Data();

            // assert no more allocation as long as one don't push more element than capacity 
            {
                for (var i = 0; i < capacity; i++)
                {
                    list.Add(i);
                    Assert.Equal(i, list[i]);
                    Assert.Equal(i, list.At(i));
                    Assert.Equal(i, list.IndexOf(i));
                    Assert.Same(arr, list.Data());
                }

                Assert.Equal(capacity, list.Count);
                listMock.VerifyNoOtherCalls();
            }
        }

        [Fact]
        public void Test_Basics()
        {
            var listMock = new Mock<ArrayList<int>> {CallBase = true};
            var list = listMock.Object;

            listMock.VerifyNoOtherCalls(); // => no allocation

            list.Add(99);
            Assert.True(list.Capacity >= 1);
            Assert.Single(list);
            {
                var expected = new int [list.Capacity];
                expected[0] = 99;
                Assert.Equal(new[] {99}, list);
                Assert.Equal(expected, list.Data());
            }


            Assert.Equal(99, list[0]);
            Assert.Equal(99, list.At(0));
            Assert.Equal(99, list.Front());
            Assert.Equal(99, list.Back());

            // one can edit list using ref methods
            {
                list.At(0) = 2;
                Assert.Equal(2, list[0]);
                list.Front() = 3;
                Assert.Equal(3, list[0]);
                list.Back() = 3;
                Assert.Equal(3, list[0]);
                list.Data()[0] = 4;
                Assert.Equal(4, list[0]);
            }
        }

        [Fact]
        public void Test_EnsureCapacity()
        {
            var listMock = new Mock<ArrayList<int>> {CallBase = true};
            var list = listMock.Object;
            const int numElement = 5;
            listMock.VerifyNoOtherCalls(); // => no allocation

            list.EnsureCapacity(numElement);
            listMock.Verify(mock => mock.ReAlloc(ref It.Ref<int[]>.IsAny, It.IsAny<int>(), numElement), Times.Once);
            listMock.Verify(mock => mock.Alloc(numElement), Times.Once);
            listMock.VerifyNoOtherCalls();
            listMock.Invocations.Clear();

            var dataRef = list.Data();

            Assert.Equal(numElement, list.Capacity);

            for (var i = 0; i < numElement; i++)
            {
                list.Insert(i, i);
                Assert.Equal(i, list[i]);
            }

            listMock.VerifyNoOtherCalls();
            Assert.Same(dataRef, list.Data());

            // try to ensure lower capacity
            {
                list.EnsureCapacity(numElement - 1);
                Assert.Equal(numElement, list.Capacity);
                listMock.VerifyNoOtherCalls();
                list.EnsureCapacity(numElement);
                listMock.VerifyNoOtherCalls();
                Assert.Equal(numElement, list.Capacity);

                list.EnsureCapacity(0);
                listMock.VerifyNoOtherCalls();
                Assert.Equal(numElement, list.Capacity);

                Assert.Same(dataRef, list.Data());
            }

            // ensure bigger capacity
            {
                list.EnsureCapacity(numElement * 2);
                listMock.Verify(mock => mock.ReAlloc(ref It.Ref<int[]>.IsAny, It.IsAny<int>(), numElement * 2),
                    Times.Once);
                listMock.Verify(mock => mock.Free(It.IsAny<int[]>()), Times.Once);
                listMock.VerifyNoOtherCalls();

                Assert.Equal(numElement * 2, list.Capacity);

                Assert.NotSame(dataRef, list.Data()); // the data array has changed
                Assert.Equal(Enumerable.Range(0, numElement), list); // => data copied into the new array
            }
        }


        [Fact]
        public void Test_ShrinkToFit()
        {
            var listMock = new Mock<ArrayList<int>> {CallBase = true};
            var list = listMock.Object;
            const int numElement = 5;

            for (var i = 0; i < numElement; i++)
            {
                list.Add(i);
                Assert.Equal(i, list[i]);
            }

            Assert.True(list.Capacity >= list.Count);
            listMock.Invocations.Clear();

            list.ShrinkToFit();
            Assert.True(list.Capacity == list.Count);
            Assert.True(list.Capacity > 0);
            listMock.Invocations.Clear();

            // call a 2nd time then
            // check data is not changed and there's no new array alloced
            {
                var data = list.Data();
                Assert.Equal(Enumerable.Range(0, numElement), list);
                list.ShrinkToFit();
                Assert.True(list.Capacity == list.Count);
                listMock.VerifyNoOtherCalls();
                Assert.Same(data, list.Data());
                Assert.Equal(Enumerable.Range(0, numElement), list);
            }

            // grow (x2) the list's capacity, shrink and check that data's still the same
            {
                list.Capacity *= 2;
                listMock.Verify(mock => mock.ReAlloc(ref It.Ref<int[]>.IsAny, It.IsAny<int>(), numElement * 2),
                    Times.Once);
                listMock.Verify(mock => mock.Free(It.IsAny<int[]>()), Times.Once);
                listMock.VerifyNoOtherCalls();
                listMock.Invocations.Clear();

                Assert.True(list.Capacity >= list.Count);

                list.ShrinkToFit();
                Assert.True(list.Capacity == list.Count);
                listMock.Verify(mock => mock.ReAlloc(ref It.Ref<int[]>.IsAny, It.IsAny<int>(), numElement), Times.Once);
                listMock.Verify(mock => mock.Free(It.IsAny<int[]>()), Times.Once);
                listMock.VerifyNoOtherCalls();

                Assert.Equal(Enumerable.Range(0, numElement), list);
            }

            // test edge case with 0 size, 1 capacity to 0 capacity
            {
                list.Clear();
                listMock.Invocations.Clear();
                Assert.Empty(list);
                list.ShrinkToFit();
                Assert.Equal(0, list.Capacity);
                listMock.VerifyNoOtherCalls();
                list.Capacity = 1;
                listMock.Verify(mock => mock.Alloc(1), Times.Once);
                listMock.Verify(mock => mock.ReAlloc(ref It.Ref<int[]>.IsAny, It.IsAny<int>(), 1), Times.Once);
                listMock.VerifyNoOtherCalls();
                list.ShrinkToFit();
                Assert.Equal(0, list.Capacity);
                listMock.Verify(mock => mock.ReAlloc(ref It.Ref<int[]>.IsAny, It.IsAny<int>(), 0), Times.Once);
                listMock.Verify(mock => mock.Free(It.IsAny<int[]>()), Times.Once);
                listMock.VerifyNoOtherCalls(); // note that a zero size array is not alloced with Alloc
            }
        }

        [Fact]
        public void Test_Clear()
        {
            var listMock = new Mock<ArrayList<int>> {CallBase = true};
            var list = listMock.Object;
            const int numElement = 5;

            for (var i = 0; i < numElement; i++)
            {
                list.Add(i);
                Assert.Equal(i, list[i]);
            }

            Assert.Equal(numElement, list.Count);
            var data = list.Data();
            listMock.Invocations.Clear();

            // clear and check
            {
                list.Clear();
                Assert.Empty(list);
                Assert.Equal(0, list.Capacity);
                Assert.NotSame(data, list.Data());
                listMock.Verify(mock => mock.Free(It.IsAny<int[]>()));
                listMock.VerifyNoOtherCalls(); // note that a zero size array is not alloced with Alloc
            }

            listMock.Invocations.Clear();

            // do a 2nd time clear as a edge case check
            {
                list.Clear();
                Assert.Empty(list);
                Assert.Equal(0, list.Capacity);
                listMock.Verify(mock => mock.Free(It.IsAny<int[]>()));
                listMock.VerifyNoOtherCalls();
            }

            // ensure the list is still usable after 2 clears
            {
                for (var i = 0; i < numElement; i++)
                {
                    list.Add(i);
                    Assert.Equal(i, list[i]);
                }

                Assert.Equal(Enumerable.Range(0, numElement), list);
            }
        }

        [Theory]
        [InlineData(1, true, null)]
        [InlineData(0, true, null)]
        [InlineData(5, true, null)]
        [InlineData(10, true, null)]
        [InlineData(0, false, null)]
        [InlineData(1, false, null)]
        [InlineData(5, false, null)]
        [InlineData(10, false, null)]
        [InlineData(-1, false, typeof(ArgumentException))]
        [InlineData(-2, false, typeof(ArgumentException))]
        public void Test_Resize(int newSize, bool clear, Type exception)
        {
            var listMock = new Mock<ArrayList<int>> {CallBase = true};
            var list = listMock.Object;
            const int numElement = 5;

            for (var i = 0; i < numElement; i++)
            {
                list.Add(i);
                Assert.Equal(i, list[i]);
            }

            Assert.Equal(numElement, list.Count);
            var data = list.Data();
            listMock.Invocations.Clear();

            try
            {
                list.Resize(newSize, clear);
            }
            catch (Exception e) when (!(e is XunitException))
            {
                Assert.NotNull(exception);
                Assert.IsAssignableFrom(exception, e);
                Assert.Equal(Enumerable.Range(0, numElement), list); // no changes
                listMock.VerifyNoOtherCalls();
                return;
            }

            if (newSize > numElement)
            {
                Assert.True(list.Capacity >= newSize);
                listMock.Verify(mock => mock.ReAlloc(ref It.Ref<int[]>.IsAny, numElement,
                    It.IsInRange(newSize, int.MaxValue, Range.Inclusive)));
                listMock.Verify(mock => mock.Free(It.IsAny<int[]>()), Times.Once);
                listMock.VerifyNoOtherCalls();
            }
            else
            {
                Assert.Same(data, list.Data()); // same data array ref
                listMock.VerifyNoOtherCalls(); // no realloc
            }

            Assert.Equal(newSize, list.Count);
            var expected = Enumerable.Range(0, newSize).ToList();
            if (newSize > numElement)
            {
                expected.RemoveRange(numElement, newSize - numElement);
                expected.AddRange(Enumerable.Repeat(default(int), newSize - expected.Count));
            }

            Assert.Equal(expected, list.AsSpan().ToArray());
            if (clear)
            {
                for (var i = newSize; i < numElement; i++)
                {
                    Assert.Equal(default, list.Data()[i]);
                }
            }
            else
            {
                for (var i = newSize; i < numElement; i++)
                {
                    Assert.Equal(i, list.Data()[i]);
                }

                for (var i = numElement; i < newSize; i++)
                {
                    Assert.Equal(default, list.Data()[i]);
                }
            }
        }
    }
}