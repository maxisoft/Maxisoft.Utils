using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Maxisoft.Utils.Tests
{
    public class LinkedListExtensions
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
            Assert.Equal(0, list.Count);
            
            list.RemoveAll(i => i == 5);
            
            Assert.Equal(0, list.Count);
            Assert.DoesNotContain(list, i => i == 5);
        }
    }
}