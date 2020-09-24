using System;
using System.Collections.Generic;
using System.Linq;
using Maxisoft.Utils.Collections.LinkedLists;
using Xunit;

namespace Maxisoft.Utils.Tests.Collections.LinkedLists
{
    public class LinkedListExtensionsTests
    {
        [Fact]
        public void TestRemoveAll_ListContains()
        {
            var list = new LinkedList<int>();
            foreach (var i in Enumerable.Range(1, 50))
            {
                list.AddLast(i);
            }

            Assert.Contains(list, i => i == 5);
            Assert.Equal(50, list.Count);

            list.RemoveAll(i => i == 5);

            Assert.Equal(49, list.Count);
            Assert.DoesNotContain(list, i => i == 5);
        }

        [Fact]
        public void TestRemoveAll_ListContainsMultipleTime()
        {
            var list = new LinkedList<int>();
            for (var c = 0; c < 5; c++)
            {
                foreach (var i in Enumerable.Range(1, 50))
                {
                    list.AddLast(i);
                }
            }

            Assert.Contains(list, i => i == 5);
            Assert.Equal(50 * 5, list.Count);

            list.RemoveAll(i => i == 5);

            Assert.Equal(49 * 5, list.Count);
            Assert.DoesNotContain(list, i => i == 5);
        }

        [Fact]
        public void TestRemoveAll_ListDoesntContain()
        {
            var list = new LinkedList<int>();
            foreach (var i in Enumerable.Range(1, 50))
            {
                if (i == 5) continue;
                list.AddLast(i);
            }

            Assert.DoesNotContain(list, i => i == 5);
            Assert.Equal(49, list.Count);

            list.RemoveAll(i => i == 5);

            Assert.Equal(49, list.Count);
            Assert.DoesNotContain(list, i => i == 5);
        }

        [Fact]
        public void TestRemoveAll_EmptyList()
        {
            var list = new LinkedList<int>();

            Assert.DoesNotContain(list, i => i == 5);
            Assert.Empty(list);

            list.RemoveAll(i => i == 5);

            Assert.Empty(list);
            Assert.DoesNotContain(list, i => i == 5);
        }

        [Fact]
        public void TestRemoveAll_LinkedListPredicate()
        {
            var list = new LinkedList<int>();
            foreach (var i in Enumerable.Range(1, 50))
            {
                list.AddLast(i);
            }

            static bool Filter(LinkedListNode<int> node)
            {
                return node.Value == 5;
            }

            list.RemoveAll(Filter);

            Assert.Equal(49, list.Count);
            Assert.DoesNotContain(list, i => i == 5);
        }

        [Fact]
        public void Test_ReversedIterator()
        {
            var list = new LinkedList<int>();
            foreach (var i in Enumerable.Range(1, 50))
            {
                if (i % 5 == 0) continue;
                list.AddLast(i);
            }

            using var it = list.ReversedIterator().GetEnumerator();
            Assert.True(it.MoveNext());
            var c = 1;
            foreach (var i in Enumerable.Range(1, 50).Reverse())
            {
                if (i % 5 == 0) continue;
                Assert.Equal(i, it.Current);
                if (it.MoveNext())
                {
                    c++;
                }
                else
                {
                    break;
                }
            }

            Assert.Equal(list.Count, c);
            Assert.False(it.MoveNext());
        }

        [Fact]
        public void Test_ReversedNodeIterator()
        {
            var list = new LinkedList<int>();
            foreach (var i in Enumerable.Range(1, 50))
            {
                if (i % 5 == 0) continue;
                list.AddLast(i);
            }

            using var it = list.ReversedNodeIterator().GetEnumerator();
            Assert.True(it.MoveNext());
            var c = 1;
            foreach (var i in Enumerable.Range(1, 50).Reverse())
            {
                if (i % 5 == 0) continue;
                Assert.Equal(i, it.Current?.Value);
                if (it.MoveNext())
                {
                    c++;
                }
                else
                {
                    break;
                }
            }

            Assert.Equal(list.Count, c);
            Assert.False(it.MoveNext());
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(64)]
        public void TestAt(int size)
        {
            var l = new LinkedList<int>();
            for (var i = 0; i < size; i++)
            {
                l.AddLast(i);
            }

            Assert.Equal(size, l.Count);

            for (var i = 0; i < size; i++)
            {
                Assert.Equal(i, l.At(i).Value);
            }
        }

        private const int DefaultSize = 8;

        [Theory]
        [InlineData(DefaultSize)]
        [InlineData(DefaultSize + 1)]
        [InlineData(DefaultSize + 2)]
        [InlineData(-1)]
        [InlineData(-2)]
        [InlineData(-DefaultSize)]
        [InlineData(int.MinValue)]
        public void TestAt_OutOfBound(int index)
        {
            var l = new LinkedList<int>();
            for (var i = 0; i < DefaultSize; i++)
            {
                l.AddLast(i);
            }

            Assert.Throws<ArgumentOutOfRangeException>(() => l.At(index));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(64)]
        public void TestInsert(int size)
        {
            for (var i = 0; i <= size; i++)
            {
                var l = LinkedListExtensions.Range(size);
                var adversarial = Enumerable.Range(0, size).ToList();
                Assert.Equal(l, adversarial);

                l.Insert(i, -i);
                adversarial.Insert(i, -i);
                Assert.Equal(l, adversarial);
            }
        }
        
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(64)]
        public void TestRemoveAt(int size)
        {
            for (var i = 0; i < size; i++)
            {
                var l = LinkedListExtensions.Range(size);
                var adversarial = Enumerable.Range(0, size).ToList();
                Assert.Equal(l, adversarial);

                l.RemoveAt(i);
                adversarial.RemoveAt(i);
                Assert.Equal(l, adversarial);
            }
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(64)]
        public void TestIndexOf(int size)
        {
            var l = LinkedListExtensions.Range(size);
            var adversarial = Enumerable.Range(0, size).ToList();
            Assert.Equal(l, adversarial);
            for (var i = -2; i < size + 2; i++)
            {
                Assert.Equal(adversarial.IndexOf(i), l.IndexOf(i));
            }
        }
    }
}