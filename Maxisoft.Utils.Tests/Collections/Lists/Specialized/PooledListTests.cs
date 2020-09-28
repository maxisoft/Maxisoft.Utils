using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using Maxisoft.Utils.Collections.Lists.Specialized;
using Moq;
using Troschuetz.Random;
using Xunit;
using Range = Moq.Range;

namespace Maxisoft.Utils.Tests.Collections.Lists.Specialized
{
    public class PooledListTests
    {
        [Fact]
        public void Test_Basics()
        {
            var listMock = new Mock<PooledList<int>>(){CallBase = true};
            var list = listMock.Object;

            const int numElements = 32;
            var numberGenerator = new TRandom(numElements);

            var adversarial = new List<int>();
            
            for (var i = 0; i < numElements; i++)
            {
                var n = numberGenerator.Next();
                adversarial.Add(n);
                list.Add(n);
            }
            
            Assert.Equal(adversarial, list);
            
            listMock.Verify(mock => mock.ReAlloc(ref It.Ref<int[]>.IsAny, It.IsAny<int>(), It.IsAny<int>()), Times.AtLeastOnce);
            listMock.Verify(mock => mock.ComputeGrowSize(It.IsAny<int>(), It.IsAny<int>()), Times.AtLeastOnce);
            listMock.Verify(mock => mock.Alloc(It.IsAny<int>()), Times.AtLeastOnce);
            listMock.Verify(mock => mock.Free(It.IsAny<int[]>()), Times.AtLeastOnce);
            listMock.VerifyNoOtherCalls();
            
        }

        [Fact]
        public void Test_With_CustomArrayPool()
        {
            var poolMock = new Mock<CustomArrayPool<int>>(){CallBase = true};
            var pool = poolMock.Object;
            var listMock = new Mock<PooledList<int>>(args: new object[]{0, pool}){CallBase = true};
            var list = listMock.Object;


            // assert that a zero capacity list doesn't allocate
            {
                Assert.Equal(0, list.Capacity);
                poolMock.VerifyNoOtherCalls();
                listMock.VerifyNoOtherCalls();
            }

            const int numElements = 50;
            var data = list.Data();
            var capacity = list.Capacity;
            var numberGenerator = new TRandom(seed: numElements);
            for (var i = 0; i < numElements; i++)
            {
                list.Add(numberGenerator.Next());
                var newData = list.Data();
                var newCapacity = list.Capacity;
                if (newCapacity != capacity)
                {
                    Assert.True(list.Capacity >= i);
                    Assert.NotSame(data, newData);
                    if (!ReferenceEquals(data, PooledList<int>.EmptyArray))
                    {
                        var data1 = data;
                        poolMock.Verify(mock => mock.Return(data1, It.IsAny<bool>()));
                    }

                    var i1 = i;
                    poolMock.Verify(mock => mock.Rent(It.IsInRange(i1 + 1, int.MaxValue, Range.Inclusive)));
                    poolMock.VerifyNoOtherCalls();
                    poolMock.Invocations.Clear();
                }

                data = newData;
                capacity = newCapacity;
            }
            
            poolMock.Invocations.Clear();
            listMock.Invocations.Clear();

            
            // test shrink to fit
            {
                var initialCapacity = list.Capacity;
                var initialCount = list.Count;
                var initialData = list.Data();
                list.ShrinkToFit();
                Assert.True(initialCapacity >= list.Capacity);
                Assert.Equal(list.Count, initialCount);
                if (!ReferenceEquals(initialData, list.Data()))
                {
                    poolMock.Verify(mock => mock.Return(initialData, It.IsAny<bool>()));
                }
            }
            
            poolMock.Invocations.Clear();
            listMock.Invocations.Clear();

            // test resize
            {
                var initialCapacity = list.Capacity;
                var initialCount = list.Count;
                list.Resize(list.Count / 2);
                
                Assert.Equal(initialCount / 2, list.Count);
                Assert.True(list.Capacity <= initialCapacity);
                Assert.True(list.Capacity >= initialCount / 2);
            }
        }

        public class CustomArrayPool<T> : ArrayPool<T>
        {
            public override T[] Rent(int minimumLength)
            {
                return new T[minimumLength + 3];
            }

            public override void Return(T[] array, bool clearArray = false)
            {
                if (clearArray)
                {
                    Array.Clear(array, 0, array.Length);
                }
            }
        }
    }
}