using System;
using System.Collections;
using System.Collections.Generic;
using Maxisoft.Utils.Collections;
using Maxisoft.Utils.Collections.Lists;
using Maxisoft.Utils.Collections.Lists.Specialized;
using Troschuetz.Random;
using Xunit;

namespace Maxisoft.Utils.Tests.Collections
{
    public class WrappedIndexTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(17)]
        public void Test_Basics(int size)
        {
            
            var numberGenerator = new TRandom(size);
            
            {
                var arr = new List<int>(size);
                for (var i = 0; i < size; i++)
                {
                    arr.Add(numberGenerator.Next());
                }

                for (var i = 0; i < size; i++)
                {
                    var index = new WrappedIndex(i);
                    Assert.Equal(i, index.Resolve((ICollection) arr));
                    Assert.Equal(i, index.Resolve<int, IList<int>>(arr));
                    Assert.Equal(i, index.Resolve(in arr));
                    Assert.Equal(i, index.Resolve(arr.Count));
                    Assert.Equal(arr[i], arr.At(index));
                }
                
                for (var i = 1; i <= size; i++)
                {
                    var index = new WrappedIndex(-i);
                    Assert.Equal(arr.Count - i, index.Resolve((ICollection) arr));
                    Assert.Equal(arr.Count - i, index.Resolve<int, IList<int>>(arr));
                    Assert.Equal(arr.Count - i, index.Resolve(in arr));
                    Assert.Equal(arr.Count - i, index.Resolve(arr.Count));
                    Assert.Equal(arr[^i], arr.At(index));
                }
            }
            
            {
                var arr = new int[size];
                for (var i = 0; i < size; i++)
                {
                    arr[i] = numberGenerator.Next();
                }

                for (var i = 0; i < size; i++)
                {
                    var index = new WrappedIndex(i);
                    Assert.Equal(i, index.Resolve((ICollection) arr));
                    Assert.Equal(i, index.Resolve<int, IList<int>>(arr));
                    Assert.Equal(i, index.Resolve(in arr));
                    Assert.Equal(i, index.Resolve((Array) arr));
                    Assert.Equal(i, index.Resolve(arr.Length));
                    Assert.Equal(arr[i], arr.At(index));
                }
                
                for (var i = 1; i <= size; i++)
                {
                    var index = new WrappedIndex(-i);
                    Assert.Equal(arr.Length - i, index.Resolve((ICollection) arr));
                    Assert.Equal(arr.Length - i, index.Resolve<int, IList<int>>(arr));
                    Assert.Equal(arr.Length - i, index.Resolve(in arr));
                    Assert.Equal(arr.Length - i, index.Resolve((Array) arr));
                    Assert.Equal(arr.Length - i, index.Resolve(arr.Length));
                    Assert.Equal(arr[^i], arr.At(index));
                }
            }
        }
    }
}