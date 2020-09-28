using System;
using System.Linq;
using Maxisoft.Utils.Collections.Lists;
using Moq;
using Troschuetz.Random;
using Xunit;

namespace Maxisoft.Utils.Tests.Collections.Lists
{
    public class ArrayListWrapperTests
    {
        
        [Fact]
        public void Test_Default_Constructor()
        {
            var mockList = new Mock<ArrayListWrapper<int>>() {CallBase = true};
            var list = mockList.Object;
            Assert.Empty(list);
            Assert.Equal(0, list.Capacity);
            mockList.VerifyNoOtherCalls();

            // can't add anything into the list
            Assert.Throws<InvalidOperationException>(() => list.Add(0));
            
            Assert.Equal(Enumerable.Empty<int>(), list);
            Assert.Same(ArrayListWrapper<int>.EmptyArray, list.Data());
        }
        
        [Fact]
        public void Test_Basics()
        {
            const int numElements = 32;
            var underlying = new int[numElements];
            var numberGenerator = new TRandom(numElements);
            var mockList = new Mock<ArrayListWrapper<int>>(underlying, numElements) {CallBase = true};
            var list = mockList.Object;
            
            mockList.VerifyNoOtherCalls();
            
            for (var i = 0; i < numElements; i++)
            {
                var n = numberGenerator.Next();
                list[i] = n;
                Assert.Equal(n, list[i]);
                Assert.Equal(n, underlying[i]);
            }
            mockList.VerifyNoOtherCalls();
            
            for (var i = 0; i < numElements; i++)
            {
                var n = numberGenerator.Next();
                underlying[i] = n;
                Assert.Equal(n, underlying[i]);
                Assert.Equal(n, list[i]);
            }
            mockList.VerifyNoOtherCalls();
            
            
            
            // now test allocation methods
            {
                list.Free(underlying);
                mockList.Verify(mock => mock.Free(underlying));
                mockList.VerifyNoOtherCalls();
                mockList.Invocations.Clear();
                
                Assert.Throws<InvalidOperationException>(() => list.Alloc(1));
                Assert.Same(list.Data(), list.Alloc(numElements));
                mockList.Verify(mock => mock.Alloc(It.IsAny<int>()), Times.Exactly(2));
                mockList.VerifyNoOtherCalls();
                mockList.Invocations.Clear();
                
                var copy = underlying;
                list.ReAlloc(ref copy, numElements, numElements);
                Assert.Same(underlying, copy);
                Assert.Throws<InvalidOperationException>(() => list.ReAlloc(ref copy, numElements, numElements * 2));
                mockList.Verify(mock => mock.ReAlloc(ref It.Ref<int[]>.IsAny, It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
                mockList.VerifyNoOtherCalls();
            }
        }
    }
}