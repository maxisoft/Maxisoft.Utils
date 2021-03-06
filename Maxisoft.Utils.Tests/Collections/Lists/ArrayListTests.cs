﻿using System;
using System.Linq;
using Maxisoft.Utils.Collections.Allocators;
using Maxisoft.Utils.Collections.Lists;
using Moq;
using Troschuetz.Random;
using Xunit;
using Xunit.Sdk;

namespace Maxisoft.Utils.Tests.Collections.Lists
{
    public class ArrayListTests
    {
        [Fact]
        public void Test_Default_Constructor_Doesnt_Allocate()
        {
            {
                var listMock = new Mock<ArrayList<int>> {CallBase = true};
                var list = listMock.Object;
                listMock.VerifyNoOtherCalls();
                Assert.Same(ArrayList<int>.EmptyArray, list.Data());
                Assert.Equal(0, list.Capacity);
                Assert.Empty(list);
            }


            {
                var mockAllocator = MockAllocator<int>.Create();
                var list = new TestableArrayList<int>(mockAllocator.Object);
                Assert.Same(ArrayList<int>.EmptyArray, list.Data());
                mockAllocator.VerifyNoOtherCalls();
            }
        }

        [Fact]
        public void Test_Constructor_With_Array()
        {
            var arr = Enumerable.Range(0, 5).ToArray();
            var mockAllocator = MockAllocator<int>.Create();
            var listMock = new Mock<TestableArrayList<int>>(arr, arr.Length, mockAllocator.Object) {CallBase = true};
            var list = listMock.Object;
            mockAllocator.VerifyNoOtherCalls(); // => no allocation
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

            mockAllocator.VerifyNoOtherCalls(); // => no allocation

            // as soon as one add more elements than the list capacity could handle the array is detached from the list
            {
                list.Add(6);
                Assert.NotSame(arr, list.Data());
                Assert.Equal(new[] {1, 2, 3, 4, 5, 6}, list);
                mockAllocator.Verify(mock => mock.ReAlloc(ref It.Ref<int[]>.IsAny, ref It.Ref<int>.IsAny),
                    Times.Once);
                listMock.Verify(mock => mock.ComputeGrowSize(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                mockAllocator.Verify(mock => mock.Free(ref It.Ref<int[]>.IsAny), Times.AtMostOnce);
                mockAllocator.VerifyNoOtherCalls();
            }

            // test constructor with size
            {
                Assert.Equal(new ArrayList<int>(arr).GetRange(0, 3), new ArrayList<int>(arr, 3));
                Assert.Equal(new ArrayList<int>(arr, arr.Length), new ArrayList<int>(arr));
            }

            // throws on negative size
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayList<int>(arr, -1));
                Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayList<int>(arr, int.MinValue));
            }

            // throws on size larger than array
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayList<int>(arr, arr.Length + 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayList<int>(arr, int.MaxValue));
            }
        }

        [Fact]
        public void Test_Constructor_Capacity()
        {
            const int capacity = 10;
            var listMock = new Mock<ArrayList<int>>(capacity) {CallBase = true};
            var list = listMock.Object;
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

            // throws on negative capacity
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayList<int>(-1));
                Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayList<int>(int.MinValue));
                // but not on zero capacity
                Assert.Equal(0, new ArrayList<int>(0).Capacity);
            }
        }

        [Fact]
        public void Test_Basics()
        {
            var allocatorMock = MockAllocator<int>.Create();
            var listMock = new Mock<TestableArrayList<int>>(allocatorMock.Object) {CallBase = true};
            var list = listMock.Object;

            allocatorMock.VerifyNoOtherCalls();
            listMock.VerifyNoOtherCalls();

            // ReSharper disable once xUnit2013
            Assert.Equal(0, list.Count);
            Assert.Equal(0, list.Capacity);
            Assert.False(list.IsReadOnly);
            Assert.Equal(Enumerable.Empty<int>(), list);

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
            var allocatorMock = MockAllocator<int>.Create();
            var listMock = new Mock<TestableArrayList<int>>(allocatorMock.Object) {CallBase = true};
            var list = listMock.Object;
            const int numElement = 5;
            listMock.VerifyNoOtherCalls(); // => no allocation

            list.EnsureCapacity(numElement);
            allocatorMock.Verify(mock => mock.ReAlloc(ref It.Ref<int[]>.IsAny, ref It.Ref<int>.IsAny), Times.Once);
            allocatorMock.Verify(mock => mock.Alloc(ref It.Ref<int>.IsAny), Times.AtMostOnce);
            listMock.VerifyNoOtherCalls();
            listMock.Invocations.Clear();
            allocatorMock.Invocations.Clear();

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
                allocatorMock.Verify(mock => mock.ReAlloc(ref It.Ref<int[]>.IsAny, ref It.Ref<int>.IsAny),
                    Times.Once);
                allocatorMock.Verify(mock => mock.Free(ref It.Ref<int[]>.IsAny), Times.AtMostOnce);
                listMock.VerifyNoOtherCalls();

                Assert.Equal(numElement * 2, list.Capacity);

                Assert.NotSame(dataRef, list.Data()); // the data array has changed
                Assert.Equal(Enumerable.Range(0, numElement), list); // => data copied into the new array
            }
        }


        [Fact]
        public void Test_ShrinkToFit()
        {
            var allocatorMock = MockAllocator<int>.Create();
            var listMock = new Mock<TestableArrayList<int>>(allocatorMock.Object) {CallBase = true};
            var list = listMock.Object;
            const int numElement = 5;

            for (var i = 0; i < numElement; i++)
            {
                list.Add(i);
                Assert.Equal(i, list[i]);
            }

            Assert.True(list.Capacity >= list.Count);
            listMock.Invocations.Clear();
            allocatorMock.Invocations.Clear();

            list.ShrinkToFit();
            Assert.True(list.Capacity == list.Count);
            Assert.True(list.Capacity > 0);
            listMock.Invocations.Clear();
            allocatorMock.Invocations.Clear();

            // call a 2nd time then
            // check data is not changed and there's no new array alloced
            {
                var data = list.Data();
                Assert.Equal(Enumerable.Range(0, numElement), list);
                list.ShrinkToFit();
                Assert.True(list.Capacity == list.Count);
                listMock.VerifyNoOtherCalls();
                allocatorMock.VerifyNoOtherCalls();
                Assert.Same(data, list.Data());
                Assert.Equal(Enumerable.Range(0, numElement), list);
            }

            // grow (x2) the list's capacity, shrink and check that data's still the same
            {
                list.Capacity *= 2;
                allocatorMock.Verify(mock => mock.ReAlloc(ref It.Ref<int[]>.IsAny, ref It.Ref<int>.IsAny),
                    Times.Once);
                allocatorMock.Verify(mock => mock.Free(ref It.Ref<int[]>.IsAny), Times.AtMostOnce);
                listMock.VerifyNoOtherCalls();
                allocatorMock.VerifyNoOtherCalls();
                allocatorMock.Invocations.Clear();
                listMock.Invocations.Clear();

                Assert.True(list.Capacity >= list.Count);

                list.ShrinkToFit();
                Assert.True(list.Capacity == list.Count);
                allocatorMock.Verify(mock => mock.ReAlloc(ref It.Ref<int[]>.IsAny, ref It.Ref<int>.IsAny), Times.Once);
                allocatorMock.Verify(mock => mock.Free(ref It.Ref<int[]>.IsAny), Times.AtMostOnce);
                listMock.VerifyNoOtherCalls();
                allocatorMock.VerifyNoOtherCalls();

                Assert.Equal(Enumerable.Range(0, numElement), list);
            }

            // test edge case with 0 size, 1 capacity to 0 capacity
            {
                list.Clear();
                allocatorMock.Invocations.Clear();
                listMock.Invocations.Clear();
                Assert.Empty(list);
                list.ShrinkToFit();
                Assert.Equal(0, list.Capacity);
                listMock.VerifyNoOtherCalls();
                list.Capacity = 1;
                allocatorMock.Verify(mock => mock.Alloc(ref It.Ref<int>.IsAny), Times.AtMostOnce);
                allocatorMock.Verify(mock => mock.ReAlloc(ref It.Ref<int[]>.IsAny, ref It.Ref<int>.IsAny), Times.Once);
                allocatorMock.VerifyNoOtherCalls();
                listMock.VerifyNoOtherCalls();
                list.ShrinkToFit();
                Assert.Equal(0, list.Capacity);
                allocatorMock.Verify(mock => mock.ReAlloc(ref It.Ref<int[]>.IsAny, ref It.Ref<int>.IsAny),
                    Times.Exactly(2));
                allocatorMock.Verify(mock => mock.Free(ref It.Ref<int[]>.IsAny), Times.AtMost(2));
                allocatorMock.VerifyNoOtherCalls();
                listMock.VerifyNoOtherCalls(); // note that a zero size array is not alloced with Alloc
            }
        }

        [Fact]
        public void Test_Clear()
        {
            var allocatorMock = MockAllocator<int>.Create();
            var listMock = new Mock<TestableArrayList<int>>(allocatorMock.Object) {CallBase = true};
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
            allocatorMock.Invocations.Clear();

            // clear and check
            {
                list.Clear();
                Assert.Empty(list);
                Assert.Equal(0, list.Capacity);
                Assert.NotSame(data, list.Data());
                allocatorMock.Verify(mock => mock.Free(ref It.Ref<int[]>.IsAny));
                allocatorMock.VerifyNoOtherCalls(); // note that a zero size array is not alloced with Alloc
                listMock.VerifyNoOtherCalls(); // note that a zero size array is not alloced with Alloc
            }
            allocatorMock.Invocations.Clear();
            listMock.Invocations.Clear();

            // do a 2nd time clear as a edge case check
            {
                list.Clear();
                Assert.Empty(list);
                Assert.Equal(0, list.Capacity);
                allocatorMock.Verify(mock => mock.Free(ref It.Ref<int[]>.IsAny));
                allocatorMock.VerifyNoOtherCalls();
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
            var allocatorMock = MockAllocator<int>.Create();
            var listMock = new Mock<TestableArrayList<int>>(allocatorMock.Object) {CallBase = true};
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
            allocatorMock.Invocations.Clear();

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
                allocatorMock.VerifyNoOtherCalls();
                return;
            }

            if (newSize > numElement)
            {
                Assert.True(list.Capacity >= newSize);
                allocatorMock.Verify(mock => mock.ReAlloc(ref It.Ref<int[]>.IsAny, ref It.Ref<int>.IsAny));
                allocatorMock.Verify(mock => mock.Free(ref It.Ref<int[]>.IsAny), Times.AtMostOnce);
                allocatorMock.VerifyNoOtherCalls();
                listMock.VerifyNoOtherCalls();
            }
            else
            {
                Assert.Same(data, list.Data()); // same data array ref
                listMock.VerifyNoOtherCalls(); // no realloc
                allocatorMock.VerifyNoOtherCalls();
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

        [Fact]
        public void Test_GrowIfNeeded_EdgeCase()
        {
            var listMock = new Mock<ArrayList<int>> {CallBase = true};
            var list = listMock.Object;

            Assert.Equal(0, list.Capacity);
            Assert.Empty(list);
            var data = list.Data();

            list.GrowIfNeeded();
            Assert.True(list.Capacity > 0);
            Assert.NotSame(data, list.Data());
            Assert.Empty(list);
        }

        [Fact]
        public void Test_Realloc_EdgeCase()
        {
            var allocatorMock = MockAllocator<int>.Create();
            var listMock = new Mock<TestableArrayList<int>>(allocatorMock.Object) {CallBase = true};
            var list = listMock.Object;

            Assert.Equal(0, list.Capacity);
            var data = list.Data();
            var capacity = 0;
            allocatorMock.Object.ReAlloc(ref data, ref capacity);
            Assert.Equal(0, list.Capacity);
            Assert.Same(data, list.Data());
            allocatorMock.Verify(mock => mock.ReAlloc(ref data, ref It.Ref<int>.IsAny), Times.Once);
            allocatorMock.VerifyNoOtherCalls();
            listMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(5)]
        [InlineData(0)]
        public void Test_Convert_ToSpan(int numElement)
        {
            var listMock = new Mock<ArrayList<int>> {CallBase = true};
            var list = listMock.Object;
            var numberGen = new TRandom(numElement.GetHashCode());

            for (var i = 0; i < numElement; i++)
            {
                var n = numberGen.Next();
                list.Add(n);
                Assert.Equal(n, list[i]);
            }

            // test AsSpan
            {
                var span = list.AsSpan();
                Assert.Equal(numElement, span.Length);
                for (var i = 0; i < numElement; i++)
                {
                    Assert.Equal(span[i], list[i]);
                }

                if (numElement > 0)
                {
                    // test change a span's value
                    // read the list changed value
                    {
                        var index = numberGen.Next(span.Length);
                        span[index] = numberGen.Next();
                        Assert.Equal(span[index], list[index]);
                    }

                    // test change a list's value
                    // read the span changed value
                    {
                        var index = numberGen.Next(span.Length);
                        list[index] = numberGen.Next();
                        Assert.Equal(list[index], span[index]);
                    }
                }
            }


            // test implicit cast
            {
                Span<int> span = list;
                Assert.Equal(numElement, span.Length);
                for (var i = 0; i < numElement; i++)
                {
                    Assert.Equal(span[i], list[i]);
                }
            }

            // test implicit readonly cast
            {
                ReadOnlySpan<int> span = list;
                Assert.Equal(numElement, span.Length);
                for (var i = 0; i < numElement; i++)
                {
                    Assert.Equal(span[i], list[i]);
                }
            }
        }

        #region Test Classes

        public class MockAllocator<T> : IAllocator<T>
        {
            internal readonly IAllocator<T> AllocatorImplementation;

            public MockAllocator(IAllocator<T> allocatorImplementation)
            {
                AllocatorImplementation = allocatorImplementation;
            }

            public virtual T[] Alloc(ref int capacity)
            {
                return AllocatorImplementation.Alloc(ref capacity);
            }

            public virtual void Free(ref T[] array)
            {
                AllocatorImplementation.Free(ref array);
            }

            public virtual void ReAlloc(ref T[] array, ref int capacity)
            {
                AllocatorImplementation.ReAlloc(ref array, ref capacity);
            }

            public static Mock<MockAllocator<T>> Create()
            {
                return new Mock<MockAllocator<T>>(new DefaultAllocator<T>()) {CallBase = true};
            }
        }

        public class MockAllocator : IAllocator<int>
        {
            internal readonly IAllocator<int> AllocatorImplementation;

            public MockAllocator(IAllocator<int> allocatorImplementation)
            {
                AllocatorImplementation = allocatorImplementation;
            }

            public virtual int[] Alloc(ref int capacity)
            {
                return AllocatorImplementation.Alloc(ref capacity);
            }

            public virtual void Free(ref int[] array)
            {
                AllocatorImplementation.Free(ref array);
            }

            public virtual void ReAlloc(ref int[] array, ref int capacity)
            {
                AllocatorImplementation.ReAlloc(ref array, ref capacity);
            }

            public static Mock<MockAllocator<int>> Create()
            {
                return new Mock<MockAllocator<int>>(new DefaultAllocator<int>()) {CallBase = true};
            }
        }

        public class TestableArrayList<T> : ArrayList<T, IAllocator<T>>
        {
            public TestableArrayList() : this(MockAllocator<T>.Create().Object)
            {
            }

            public TestableArrayList(IAllocator<T> allocator) : this(0, allocator)
            {
            }

            public TestableArrayList(int capacity, IAllocator<T> allocator) : base(capacity, in allocator)
            {
            }

            public TestableArrayList(T[] initialArray, int size, IAllocator<T> allocator) : base(initialArray, size,
                in allocator)
            {
            }
        }

        #endregion
    }
}