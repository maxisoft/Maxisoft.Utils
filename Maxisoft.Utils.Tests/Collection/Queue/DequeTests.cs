using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Maxisoft.Utils.Collection.Queue;
using Maxisoft.Utils.Random;
using Xunit;
using Xunit.Abstractions;

namespace Maxisoft.Utils.Tests.Collection.Queue
{
    public class DequeTests
    {
        private const long DefaultChunkSize = 7;
        private const long DefaultSize = DefaultChunkSize;
        private readonly RandomThreadSafe _randomThreadSafe = new RandomThreadSafe();
        private readonly ITestOutputHelper _testOutputHelper;


        public DequeTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }


        [Fact]
        public void TestDefaultConstructor()
        {
            var q = new Deque<int>();
            Assert.True(q.IsEmpty);
            // ReSharper disable xUnit2013
            Assert.Equal(0, q.Count);
            // ReSharper restore xUnit2013
            Assert.Empty(q);
            Assert.Equal(new int[0], q);
            Assert.DoesNotContain(0, q);
            Assert.DoesNotContain(-1, q);
            Assert.DoesNotContain(1, q);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-2)]
        [InlineData(int.MinValue)]
        [InlineData(sbyte.MinValue)]
        [InlineData(short.MinValue)]
        [InlineData(long.MinValue)]
        public void TestConstructor_WithNegativeChunkSize(long chunk)
        {
            Assert.Throws<ArgumentException>(() => new Deque<int>(chunk));
        }


        [Theory]
        [ClassData(typeof(DataGenDifferentSizesAndChunkSize))]
        public void TestPushBack(long size, long chunkSize)
        {
            var q = new Deque<int>(chunkSize);
            for (var i = 0; i < size; i++)
            {
                Assert.Equal(i, q.LongLength);
                q.PushBack(i);
                Assert.Equal(i + 1, q.LongLength);
                Assert.Equal(Enumerable.Range(0, i + 1), q);
            }

            //construct an array with the expected same elements
            var arr = size > 0 ? Enumerable.Range(0, (int) size).ToArray() : new int[0];

            Assert.Equal(size, q.LongLength);
            Assert.Equal(size, q.Length);
            Assert.Equal(arr, q.ToArray());
        }

        [Theory]
        [ClassData(typeof(DataGenDifferentSizesAndChunkSize))]
        public void TestPushFront(long size, long chunkSize)
        {
            var q = new Deque<int>(chunkSize);
            for (var i = 0; i < size; i++)
            {
                Assert.Equal(i, q.LongLength);
                q.PushFront(i);
                Assert.Equal(i + 1, q.LongLength);
                Assert.Equal(Enumerable.Range(0, i + 1).Reverse(), q);
            }

            //construct an array with the expected same elements
            var arr = size > 0 ? Enumerable.Range(0, (int) size).Reverse().ToArray() : new int[0];

            Assert.Equal(size, q.LongLength);
            Assert.Equal(size, q.Length);
            Assert.Equal(arr, q.ToArray());
        }


        [Fact]
        public void TestPushBack_LegacyCase()
        {
            var q = new Deque<int>(DefaultChunkSize);
            const long upper = DefaultChunkSize * 8;
            for (var i = 0; i < upper; i++)
            {
                q.PushBack(i);
            }

            var arr = q.ToArray();

            Assert.Equal(upper, q.LongLength);
            Assert.Equal(upper, q.Length);
            Assert.Equal(arr, q);
            Assert.Equal(arr, Enumerable.Range(0, (int) upper));
        }


        [Fact]
        public void TestRemove_NonExisting()
        {
            var q = new Deque<long> {1L, 8L};
            Assert.False(q.Remove(-1));
            Assert.False(q.Remove(0));
            Assert.False(q.Remove(2));
            Assert.False(q.Remove(7));
            Assert.False(q.Remove(9));
            Assert.False(q.Remove(long.MinValue));
            Assert.False(q.Remove(long.MaxValue));
        }

        [Fact]
        public void TestRemove_MultipleTime()
        {
            var q = new Deque<long> {1L, 8L};
            Assert.True(q.Remove(1));
            for (var i = 0; i < 3; i++)
            {
                Assert.False(q.Remove(1));
            }

            Assert.Equal(new long[] {8}, q);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-2)]
        [InlineData(int.MinValue)]
        [InlineData(sbyte.MinValue)]
        [InlineData(short.MinValue)]
        [InlineData(DefaultSize)]
        [InlineData(DefaultSize + 1)]
        public void TestRemoveAt_OutOfBound(int index)
        {
            var q = new Deque<int>(DefaultChunkSize);
            for (var i = 0; i < DefaultSize; i++)
            {
                q.PushBack(i);
            }

            Assert.Throws<ArgumentOutOfRangeException>(() => q.RemoveAt(index));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(DefaultSize - 1)]
        [InlineData(DefaultSize - 2)]
        // ReSharper disable once xUnit1025
        [InlineData(DefaultChunkSize - 1)]
        public void TestRemoveAt_EdgeCases(int index)
        {
            var q = new Deque<int>(DefaultChunkSize);
            for (var i = 0; i < DefaultSize; i++)
            {
                q.PushBack(i);
            }

            q.RemoveAt(index);


            var expected = Enumerable.Range(0, (int) DefaultSize).ToList();
            expected.RemoveAt(index);

            Assert.Equal(expected, q);
        }


        [Theory]
        [InlineData(13, 1, 10)]
        [InlineData(14, 3, 10)]
        [InlineData(14, 6, 9)]
        [InlineData(15, 4, 8)]
        public void TestRemoveAt_Regression1(long size, long chunkSize, long index)
        {
            var q = new Deque<int>(chunkSize);
            for (var i = 0; i < size; i++)
            {
                q.PushBack(i);
            }

            //construct an array with the expected same elements
            var arr = size > 0 ? Enumerable.Range(0, (int) size).ToArray() : new int[0];

            Assert.Equal(size, q.LongLength);
            Assert.Equal(arr, q.ToArray());

            var expected = arr.ToList();
            expected.RemoveAt((int) index);

            q.RemoveAt((int) index);
            Assert.Equal(expected, q);
        }

        [Fact]
        public void TestRemove_DuplicateValues_And_MultipleTimes()
        {
            var q = new Deque<long> {1L, 2L, 8L, 8L, 8L};
            Assert.True(q.Remove(1));
            for (var i = 0; i < 3; i++)
            {
                Assert.True(q.Remove(8));
            }

            Assert.Equal(new long[] {2}, q);
            Assert.False(q.Remove(1));
            Assert.False(q.Remove(8));

            Assert.True(q.Remove(2));
        }

        [Fact]
        public void TestRemove_LegacyCase()
        {
            var q = new Deque<int>(DefaultChunkSize);
            const long upper = DefaultChunkSize * 8;
            for (var i = 0; i < upper; i++)
            {
                q.PushBack(i);
            }

            var arr = q.ToArray();

            q.Remove(6);
            var expected = arr.ToList();
            expected.Remove(6);
            Assert.Equal(expected, q);
        }


        [Fact]
        public void TestInsert_LegacyCase()
        {
            var q = new Deque<int>(DefaultChunkSize);
            const long upper = DefaultChunkSize * 8;
            for (var i = 0; i < upper; i++)
            {
                q.PushBack(i);
            }

            var arr = q.ToArray();

            q.Insert(3, 777);
            var expected = arr.ToList();
            expected.Insert(3, 777);
            Assert.Equal(expected, q);
        }

        [Fact]
        public void TestInsert_Right()
        {
            var q = new Deque<int>(DefaultChunkSize);
            const long upper = DefaultChunkSize * 8;
            for (var i = 0; i < upper; i++)
            {
                q.PushBack(i);
            }

            var arr = q.ToArray();

            q.Insert(50, 777);
            var expected = arr.ToList();
            expected.Insert(50, 777);
            var actual = q.ToArray();
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void TestInsert_MultipleTimes()
        {
            var q = new Deque<long>(DefaultChunkSize) {1L, 2L, 4L, 8L};
            q.Insert(0, 0);
            q.Insert(3, 3);
            q.Insert(5, 5);
            q.Insert(6, 7);
            q.Insert(6, 6);
            q.Insert(9, 9);


            Assert.Equal(Enumerable.Range(0, 10).Select(i => (long) i), q);
        }

        [Theory]
        [InlineData(0, 16, 0)]
        [InlineData(14, 3, 10)]
        [InlineData(14, 6, 9)]
        [InlineData(15, 4, 8)]
        public void TestInsert_Regression1(long size, long chunkSize, long index)
        {
            var q = new Deque<int>(chunkSize);
            for (var i = 0; i < size; i++)
            {
                q.PushBack(i);
            }

            //construct an array with the expected same elements
            var arr = size > 0 ? Enumerable.Range(0, (int) size).ToArray() : new int[0];

            Assert.Equal(size, q.LongLength);
            Assert.Equal(arr, q.ToArray());

            var element = _randomThreadSafe.Next() * -1;

            q.Insert((int) index, element);
            var expected = arr.ToList();
            expected.Insert((int) index, element);

            var actual = q.ToArray();

            Assert.Equal(expected, q);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(DefaultSize)]
        [InlineData(DefaultChunkSize + 1)]
        // ReSharper disable once xUnit1025
        [InlineData(DefaultChunkSize)]
        [InlineData(DefaultChunkSize - 1)]
        [InlineData(0)]
        public void TestClear(long size)
        {
            var q = new Deque<int>(DefaultChunkSize);
            for (var i = 0; i < size; i++)
            {
                q.PushBack(i);
            }

            Assert.Equal(size, q.Count);

            q.Clear();

            Assert.Empty(q);
            Assert.Empty(q.ToArray());
        }

        [Fact]
        public void TestBack()
        {
            var expected = new object();
            var q = new Deque<object> {new object(), expected};
            Assert.Same(expected, q.Back());

            q = new Deque<object> {expected};
            Assert.Same(expected, q.Back());
        }

        [Fact]
        public void TestBack_IsAssignable()
        {
            var initial = new object();
            var replacement = new object();
            Assert.NotSame(initial, replacement);
            var q = new Deque<object> {new object(), initial};
            Assert.Same(initial, q.Back());

            q.Back() = replacement; // assign front to a new obj
            Assert.Same(replacement, q.Back());
            Assert.NotSame(initial, q.Back());
        }

        [Fact]
        public void TestBack_WithEmptyDequeShouldThrow()
        {
            var q = new Deque<object>();
            Assert.Throws<InvalidOperationException>(() => q.Back());
        }


        [Fact]
        public void TestFront()
        {
            var expected = new object();
            var q = new Deque<object> {expected, new object()};
            Assert.Same(expected, q.Front());

            q = new Deque<object> {expected};
            Assert.Same(expected, q.Front());
        }

        [Fact]
        public void TestFront_IsAssignable()
        {
            var initial = new object();
            var replacement = new object();
            Assert.NotSame(initial, replacement);
            var q = new Deque<object> {initial, new object()};
            Assert.Same(initial, q.Front());

            q.Front() = replacement; // assign front to a new obj
            Assert.Same(replacement, q.Front());
            Assert.NotSame(initial, q.Front());
        }

        [Fact]
        public void TestFront_WithEmptyDequeShouldThrow()
        {
            var q = new Deque<object>();
            Assert.Throws<InvalidOperationException>(() => q.Front());
        }

        [Fact]
        public void TestTryPeekFront()
        {
            var expected = new object();
            var q = new Deque<object> {expected, new object()};
            Assert.True(q.TryPeekFront(out var actual));
            Assert.Same(expected, actual);

            q = new Deque<object>();
            Assert.Empty(q);
            Assert.False(q.TryPeekFront(out _));
        }

        [Fact]
        public void TestTryPeekBack()
        {
            var expected = new object();
            var q = new Deque<object> {new object(), expected};
            Assert.True(q.TryPeekBack(out var actual));
            Assert.Same(expected, actual);

            q = new Deque<object>();
            Assert.Empty(q);
            Assert.False(q.TryPeekBack(out _));
        }

        [Fact]
        public void TestPopBack()
        {
            var expected = new object();
            var q = new Deque<object> {new object(), expected};
            Assert.Same(expected, q.PopBack());
            Assert.Equal(1, q.LongLength);
        }


        [Fact]
        public void TestPopFront()
        {
            var expected = new object();
            var q = new Deque<object> {expected, new object()};
            Assert.Same(expected, q.PopFront());
            Assert.Equal(1, q.LongLength);
        }


        internal class DataGenDifferentSizes : IEnumerable<object[]>
        {
            protected virtual long[] DequeSize => new[]
            {
                0, 1, 2, 3, DefaultChunkSize / 2, DefaultChunkSize, DefaultChunkSize * 2, DefaultChunkSize * 3,
                DefaultChunkSize * 4, DefaultChunkSize * DefaultChunkSize
            };


            public virtual IEnumerator<object[]> GetEnumerator()
            {
                var dejaVu = new HashSet<long>();

                foreach (var size in DequeSize)
                {
                    for (var i = -1; i <= 1; i++)
                    {
                        var res = size + i;
                        if (dejaVu.Add(res))
                        {
                            yield return new object[] {res};
                        }
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        internal class DataGenDifferentSizesAndChunkSize : DataGenDifferentSizes
        {
            protected virtual long[] ChunkSizes => new[]
            {
                1, 2, 3, 5, 7, 11, 13, 17, DefaultChunkSize / 2, DefaultChunkSize, DefaultChunkSize * 2,
                DefaultChunkSize * 3, DefaultChunkSize * 4, DefaultChunkSize * DefaultChunkSize
            };

            public override IEnumerator<object[]> GetEnumerator()
            {
                var dejaVu = new HashSet<(long, long)>();

                foreach (var size in DequeSize)
                {
                    for (var i = -1; i <= 1; i++)
                    {
                        var rsize = Math.Max(size + i, 0);
                        foreach (var chunk in ChunkSizes)
                        {
                            for (var j = -1; j <= 1; j++)
                            {
                                var rchunk = Math.Max(chunk + j, 1);
                                if (dejaVu.Add((rsize, rchunk)))
                                {
                                    yield return new object[] {rsize, rchunk};
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}