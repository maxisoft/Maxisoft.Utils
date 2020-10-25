using System;
using System.Collections.Generic;
using System.Linq;
using Maxisoft.Utils.Algorithms;
using Maxisoft.Utils.Collections;
using Troschuetz.Random;
using Xunit;

namespace Maxisoft.Utils.Tests.Algorithms
{
    public class SortedTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(16)]
        [InlineData(17)]
        public void Test_IsSorted_IEnumerable(int length)
        {
            var random = new TRandom(length);

            var arr = new int[length];

            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = random.Next();
            }

            var sorted = arr.ToArrayList();
            sorted.Sort();
            
            Assert.True(sorted.IsSorted());
            Assert.Equal(sorted.SequenceEqual(arr), ((IEnumerable<int>) arr).IsSorted());
            Assert.Equal(sorted.SequenceEqual(arr), ((IEnumerable<int>) arr).IsSorted(Comparer<int>.Default));
            
            Array.Sort(arr);
            Assert.True(((IEnumerable<int>) arr).IsSorted());
            Assert.True(((IEnumerable<int>) arr).IsSorted(Comparer<int>.Default));
        }
        
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(16)]
        [InlineData(17)]
        public void Test_IsSorted_List(int length)
        {
            var random = new TRandom(length);

            var arr = new int[length];

            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = random.Next();
            }

            var list = arr.ToList();

            var sorted = list.ToArrayList();
            sorted.Sort();
            
            Assert.True(sorted.IsSorted());
            Assert.Equal(sorted.SequenceEqual(list), list.IsSorted());
            Assert.Equal(sorted.SequenceEqual(list), list.IsSorted(Comparer<int>.Default));
            Assert.Equal(sorted.SequenceEqual(list), ((IList<int>)list).IsSorted());
            Assert.Equal(sorted.SequenceEqual(list), ((IList<int>)list).IsSorted(Comparer<int>.Default));
            
            list.Sort();
            Array.Sort(arr);
            Assert.True(list.IsSorted());
            Assert.True(list.IsSorted(Comparer<int>.Default));
            
            Assert.True(((IList<int>)list).IsSorted());
            Assert.True(((IList<int>)list).IsSorted(Comparer<int>.Default));
        }
        
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(16)]
        [InlineData(17)]
        public void Test_IsSorted_Array(int length)
        {
            var random = new TRandom(length);

            var arr = new int[length];

            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = random.Next();
            }
            

            var sorted = arr.ToArrayList();
            sorted.Sort();
            
            Assert.True(sorted.IsSorted());
            Assert.Equal(sorted.SequenceEqual(arr), arr.IsSorted());
            Assert.Equal(sorted.SequenceEqual(arr), arr.IsSorted(Comparer<int>.Default));
           
            Array.Sort(arr);
            Assert.True(arr.IsSorted());
            Assert.True(arr.IsSorted(Comparer<int>.Default));
        }
        
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(16)]
        [InlineData(17)]
        public void Test_IsSorted_Span(int length)
        {
            var random = new TRandom(length);

            var arr = new int[length];

            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = random.Next();
            }

            Span<int> span = arr;

            var sorted = span.ToArrayList();
            sorted.Sort();
            
            Assert.True(sorted.IsSorted());
            Assert.Equal(sorted.SequenceEqual(arr), span.IsSorted());
            Assert.Equal(sorted.SequenceEqual(arr), span.IsSorted(Comparer<int>.Default));
           
            Array.Sort(arr);
            Assert.True(span.IsSorted());
            Assert.True(span.IsSorted(Comparer<int>.Default));
        }
    }
}
