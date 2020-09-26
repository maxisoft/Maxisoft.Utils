using System.Linq;
using Maxisoft.Utils.Collections.Lists;
using Moq;
using Xunit;

namespace Maxisoft.Utils.Tests.Collections.Lists
{
    public class ArrayListTests
    {
        [Fact]
        public void Test_Default_Constructor_Doesnt_Allocate()
        {
            var listMock = new Mock<ArrayList<int>> {CallBase = true};
            var list = listMock.Object;
            listMock.VerifyNoOtherCalls(); // as Alloc / Realloc methods are virtual the mock should catch them 
            Assert.Same(ArrayList<int>.EmptyArray, list.Data());
            Assert.Equal(0, list.Capacity);
            Assert.Empty(list);
        }

        [Fact]
        public void Test_Constructor_With_Array()
        {
            var arr = Enumerable.Range(0, 5).ToArray();
            var listMock = new Mock<ArrayList<int>>(arr) {CallBase = true};
            var list = listMock.Object;
            listMock.VerifyNoOtherCalls(); // => no allocation
            Assert.Same(arr, list.Data());
            Assert.Equal(arr.Length, list.Count);
            Assert.Equal(arr.Length, list.Capacity);
            Assert.Equal(Enumerable.Range(0, 5), list);

            list.ShrinkToFit();
            Assert.Same(arr, list.Data());

            //one can remove element within the arr via the list
            {
                list.RemoveAt(0);
                list.RemoveAt(list.Count - 1);
                var expected = Enumerable.Range(1, 3).ToArray();
                Assert.Equal(expected, list);
                Assert.Equal(expected.Length, list.Count);
                Assert.Equal(arr.Length, list.Capacity);
                Assert.Same(arr, list.Data());
                Assert.Equal(new[] {1, 2, 3, 4, 4}, arr);
            }

            list.Resize(arr.Length);
            Assert.Same(arr, list.Data());

            // any changes made to the arr are directly reflected in the list
            {
                arr[^1] = 5;
                Assert.Equal(5, list.Back());
                Assert.Equal(5, list[^1]);
                Assert.Equal(5, list.At(list.Count - 1));

                for (var i = 0; i < arr.Length; i++)
                {
                    Assert.Equal(arr[i], list[i]);
                    Assert.Equal(arr[i], list.At(i));
                }
            }

            listMock.VerifyNoOtherCalls(); // => no allocation

            // as soon as one add more elements than the list capacity could handle the array is detached from the list
            {
                list.Add(6);
                Assert.NotSame(arr, list.Data());
                Assert.Equal(new[] {1, 2, 3, 4, 5, 6}, list);
                listMock.Verify(mock => mock.ReAlloc(ref It.Ref<int[]>.IsAny, It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                listMock.Verify(mock => mock.ComputeGrowSize(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
                listMock.VerifyNoOtherCalls();
            }
        }
        
        [Fact]
        public void Test_Constructor_Capacity()
        {
            const int capacity = 10;
            var listMock = new Mock<ArrayList<int>>(capacity) {CallBase = true};
            var list = listMock.Object;
            
            listMock.Verify(mock => mock.ReAlloc(ref It.Ref<int[]>.IsAny, It.IsAny<int>(), capacity), Times.Once);
            listMock.Verify(mock => mock.Alloc(capacity), Times.Once);
            listMock.VerifyNoOtherCalls();
            Assert.Equal(capacity, list.Capacity);
            Assert.Empty(list);
            var arr = list.Data();

            // assert no more allocation as long as one don't push more element than capacity 
            {
                for (var i = 0; i < capacity; i++)
                {
                    list.Add(i);
                    Assert.Equal(i, list[i]);
                    Assert.Equal(i, list.At(i));
                    Assert.Equal(i, list.IndexOf(i));
                    Assert.Same(arr, list.Data());
                }
                Assert.Equal(capacity, list.Count);
                listMock.VerifyNoOtherCalls();
            }
            
        }

        [Fact]
        public void Test_Basics()
        {
            var listMock = new Mock<ArrayList<int>>() {CallBase = true};
            var list = listMock.Object;
            
            listMock.VerifyNoOtherCalls(); // => no allocation
            
            list.Add(99);
            Assert.True(list.Capacity >= 1);
            Assert.Single(list);
            {
                var expected = new int [list.Capacity];
                expected[0] = 99;
                Assert.Equal(new int[] {99}, list);
                Assert.Equal(expected, list.Data());
            }
            
            
            Assert.Equal(99, list[0]);
            Assert.Equal(99, list.At(0));
            Assert.Equal(99, list.Front());
            Assert.Equal(99, list.Back());

            // one can edit list using ref methods
            {
                list.At(0) = 2;
                Assert.Equal(2, list[0]);
                list.Front() = 3;
                Assert.Equal(3, list[0]);
                list.Back() = 3;
                Assert.Equal(3, list[0]);
                list.Data()[0] = 4;
                Assert.Equal(4, list[0]);
            }

        }
    }
}