using System;
using System.Linq;
using Maxisoft.Utils.Collections.Queue;
using Xunit;

namespace Maxisoft.Utils.Tests.Collections.Queue
{
    public class CircularQueueTests
    {
        [Fact]
        public void Test_CircularQueue()
        {
            const int maxSize = 16;
            var q = new CircularDeque<int>(maxSize);
            Assert.False(q.IsFull);
            for (var i = 0; i < maxSize; i++)
            {
                q.Add(i);
            }

            Assert.Equal(Enumerable.Range(0, maxSize), q);

            Assert.Equal(maxSize, q.Count);
            Assert.True(q.IsFull);

            q.PushBack(maxSize);
            Assert.Equal(Enumerable.Range(1, maxSize), q);
            q.PushFront(0);
            Assert.Equal(Enumerable.Range(0, maxSize), q);

            {
                Assert.True(q.TryPeekBack(out var tmp));
                Assert.Equal(maxSize - 1, tmp);
                Assert.Equal(q[^1], tmp);
                Assert.Equal(q.ToArray()[^1], tmp);
                var expected = tmp;
                Assert.True(q.TryPopBack(out var actual));
                Assert.Equal(expected, actual);
                Assert.False(q.IsFull);
                q.PushBack(expected);
                Assert.True(q.IsFull);
            }

            {
                Assert.True(q.TryPeekFront(out var tmp));
                Assert.Equal(0, tmp);
                Assert.Equal(q[0], tmp);
                Assert.Equal(q.ToArray()[0], tmp);
                var expected = tmp;
                Assert.True(q.TryPopFront(out var actual));
                Assert.Equal(expected, actual);

                Assert.False(q.IsFull);
                q.PushFront(expected);
                Assert.True(q.IsFull);
            }


            Assert.True(q.IsFull);
            Assert.Equal(maxSize, q.Count);


            {
                for (var i = 0; i < maxSize; i++)
                {
                    Assert.True(q.TryPopBack(out _));
                    Assert.False(q.IsFull);
                }


                Assert.Empty(q);
                Assert.False(q.TryPeekBack(out _));
                Assert.False(q.TryPeekFront(out _));
                Assert.False(q.TryPopBack(out _));
                Assert.False(q.TryPopFront(out _));
                Assert.Throws<InvalidOperationException>(() => q.PopFront());
                Assert.Throws<InvalidOperationException>(() => q.PopBack());
            }


            for (var i = 0; i < maxSize; i++)
            {
                q.Add(i);
            }

            {
                for (var i = 0; i < maxSize; i++)
                {
                    Assert.True(q.TryPopFront(out _));
                    Assert.False(q.IsFull);
                }


                Assert.Empty(q);
                Assert.False(q.TryPeekBack(out _));
                Assert.False(q.TryPeekFront(out _));
                Assert.False(q.TryPopBack(out _));
                Assert.False(q.TryPopFront(out _));
                Assert.Throws<InvalidOperationException>(() => q.PopFront());
                Assert.Throws<InvalidOperationException>(() => q.PopBack());
            }
        }
    }
}