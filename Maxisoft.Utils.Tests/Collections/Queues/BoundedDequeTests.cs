using System;
using System.Linq;
using Maxisoft.Utils.Collections.Queues;
using Xunit;

namespace Maxisoft.Utils.Tests.Collections.Queues
{
    public class BoundedDequeTests
    {
        [Fact]
        public void Test_BoundedQueue()
        {
            const int maxSize = 16;
            var q = new BoundedDeque<int>(maxSize);
            Assert.False(q.IsFull);
            for (var i = 0; i < maxSize; i++)
            {
                q.Add(i);
            }

            Assert.Equal(Enumerable.Range(0, maxSize), q);

            Assert.Equal(maxSize, q.Count);
            Assert.True(q.IsFull);

            Assert.Throws<InvalidOperationException>(() => q.Add(0));
            Assert.Throws<InvalidOperationException>(() => q.PushBack(0));
            Assert.Throws<InvalidOperationException>(() => q.PushFront(0));
            Assert.False(q.TryPushBack(0));
            Assert.False(q.TryPushFront(0));


            {
                Assert.True(q.TryPeekBack(out var tmp));
                Assert.Equal(maxSize - 1, tmp);
                Assert.Equal(q[^1], tmp);
                Assert.Equal(q.ToArray()[^1], tmp);
                var expected = tmp;
                Assert.True(q.TryPopBack(out var actual));
                Assert.Equal(expected, actual);
                Assert.False(q.IsFull);
                Assert.True(q.TryPushBack(expected));
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
                Assert.True(q.TryPushFront(expected));
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