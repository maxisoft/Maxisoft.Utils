using System;
using System.Linq;
using Maxisoft.Utils.Collections.Queues;
using Maxisoft.Utils.Collections.Queues.Specialized;
using Xunit;

namespace Maxisoft.Utils.Tests.Collections.Queues.Specialized
{
    public class CircularDequeTests
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
            

            {
                q.Clear();
                Assert.Empty(q);
            }

            {
                for (var i = 0; i < maxSize; i++)
                {
                    q.PushBack(i);
                }
                Assert.Equal(Enumerable.Range(0, maxSize), q);

                Assert.True(q.Remove(0));
                Assert.Equal(Enumerable.Range(1, maxSize - 1), q);
                Assert.Equal(1, q.At(0));
                Assert.Equal(1, q.Front());
                foreach (var i in Enumerable.Range(1, maxSize - 1))
                {
                    Assert.Contains(i, q);
                }
                Assert.Equal(maxSize - 1, q.Back());
                
                Assert.Equal(6, q.IndexOf(7));
                q.Insert(0, 0);
                Assert.Equal(Enumerable.Range(0, maxSize), q);
                q.RemoveAt(0);
                Assert.Equal(Enumerable.Range(1, maxSize - 1), q);
                q.Insert(q.Count, int.MaxValue);
                q[^1] = q.Count;
                Assert.Equal(Enumerable.Range(1, maxSize), q);
            }

            {
                Assert.False(q.IsReadOnly);
            }
        }

        [Fact]
        public void Test_Constructor_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CircularDeque<int>(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new CircularDeque<int>(int.MinValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => new CircularDeque<int>(new Deque<int>(){0, 1, 2}, 2));
            
        }
    }
}