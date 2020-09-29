using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Maxisoft.Utils.Collections.Lists;
using Maxisoft.Utils.Objects;
using Maxisoft.Utils.Random;
using Moq;
using Troschuetz.Random;
using Xunit;
using Xunit.Sdk;

namespace Maxisoft.Utils.Tests.Collections.Lists
{
    public class ArrayList_ListMethodsTests
    {
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_BinarySearch(int seed)
        {
            const int maxElements = 32;
            var random = new TRandom(seed: seed);
            var numElements = random.Next(maxElements);
            var mockList = new Mock<ArrayList<int>>(args: new object[] {numElements}) {CallBase = true};
            var list = mockList.Object;
            mockList.Invocations.Clear();
            
            var adversarial = new List<int>(numElements);

            for (var i = 0; i < numElements; i++)
            {
                list.Add(i);
                adversarial.Add(i);
            }
            
            Assert.Equal(adversarial.ToImmutableSortedSet(), list);

            var lim = random.Next(maxElements);
            for (var i = 0; i < lim; i++)
            {
                IComparer<int> comparer = null;
                if (random.NextBoolean())
                {
                    comparer = Comparer<int>.Default;
                }

                Exception error = null;
                int? expected = null;
                var element =  random.Next(-2, maxElements + 2);
                try
                {
                    expected = adversarial.BinarySearch(element, comparer);
                }
                catch (Exception e)
                {
                    error = e;
                }

                
                int? res = null;
                try
                {
                    res = list.BinarySearch(element, comparer);
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    Assert.NotNull(error);
                    Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                }

                if (error is null)
                {
                    Assert.Equal(expected, res);
                    if (expected >= 0)
                    {
                        Assert.Equal(adversarial[expected.Value], list[expected.Value]);
                    }
                    
                }


                Assert.Equal(adversarial, list);
                mockList.VerifyNoOtherCalls();
            }
        }

        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_ConvertAll(int seed)
        {
            const int maxElements = 1024;
            var random = new TRandom(seed: seed);
            var numElements = random.Next(maxElements);
            var mockList = new Mock<ArrayList<int>>(args: new object[] {numElements}) {CallBase = true};
            var list = mockList.Object;
            mockList.Invocations.Clear();
        
            var adversarial = new List<int>(numElements);

            for (var i = 0; i < numElements; i++)
            {
                list.Add(i);
                adversarial.Add(i);
            }

            static string Convert(int value)
            {
                return $"{value:X}";
            }

            var expected = adversarial.ConvertAll(Convert);
            var res = list.ConvertAll(Convert);
            
            Assert.Equal(expected, res);
        }


        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_CopyTo(int seed)
        {
            const int maxElements = 32;
            var random = new TRandom(seed: seed);
            var numElements = random.Next(maxElements);
            var mockList = new Mock<ArrayList<int>>(args: new object[] {numElements}) {CallBase = true};
            var list = mockList.Object;
            mockList.Invocations.Clear();

            var adversarial = new List<int>(numElements);

            for (var i = 0; i < numElements; i++)
            {
                list.Add(i);
                adversarial.Add(i);
            }
            
            var adversarialBuff = new int[random.Next(maxElements * 2)];
            var buff = new int[adversarialBuff.Length];
            
            //fill up the buffers with random data
            void RandomFill()
            {
                for (var i = 0; i < buff.Length; i++)
                {
                    var n = random.Next();
                    adversarialBuff[i] = n;
                    buff[i] = n;
                }
            }
            
            var times = random.Next(maxElements);
            for (var _ = 0; _ < times; _++)
            {
                RandomFill();
                Exception error = null;
                var index =  random.Next(-2, maxElements + 2);
                var arrayIndex = random.Next(-2, adversarialBuff.Length + 2);
                var count = random.Next(-2, adversarialBuff.Length + 2);
                try
                {
                    adversarial.CopyTo(index, adversarialBuff, arrayIndex, count);
                }
                catch (Exception e)
                {
                    error = e;
                }
                
                try
                {
                    list.CopyTo(index, buff, arrayIndex, count);
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    try
                    {
                        Assert.NotNull(error);
                        Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                    }
                    catch (XunitException exception)
                    {
                        throw new AggregateException(e, exception);
                    }
                }

                if (error is null)
                {
                    Assert.Equal(adversarialBuff, buff);
                }


                Assert.Equal(adversarial, list);
                mockList.VerifyNoOtherCalls();
            }
        }
        
        
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_CopyTo_Span(int seed)
        {
            const int maxElements = 32;
            var random = new TRandom(seed: seed);
            var numElements = random.Next(maxElements);
            var mockList = new Mock<ArrayList<int>>(args: new object[] {numElements}) {CallBase = true};
            var list = mockList.Object;
            mockList.Invocations.Clear();

            Span<int> adversarial = new int[numElements];

            for (var i = 0; i < numElements; i++)
            {
                list.Add(i);
                adversarial[i] = i;
            }
            
            var adversarialBuff = new int[random.Next(maxElements * 2)];
            var buff = new int[adversarialBuff.Length];
            
            //fill up the buffers with random data
            void RandomFill()
            {
                for (var i = 0; i < buff.Length; i++)
                {
                    var n = random.Next();
                    adversarialBuff[i] = n;
                    buff[i] = n;
                }
            }
            
            var times = random.Next(maxElements);
            for (var _ = 0; _ < times; _++)
            {
                RandomFill();
                Exception error = null;
                try
                {
                    adversarial.CopyTo(adversarialBuff);
                }
                catch (Exception e)
                {
                    error = e;
                }
                
                try
                {
                    list.CopyTo((Span<int>) buff);
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    try
                    {
                        Assert.NotNull(error);
                        Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                    }
                    catch (XunitException exception)
                    {
                        throw new AggregateException(e, exception);
                    }
                }

                if (error is null)
                {
                    Assert.Equal(adversarialBuff, buff);
                }


                Assert.Equal(adversarial.ToArray(), list);
                mockList.VerifyNoOtherCalls();
            }
            
            
        }
        

        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_Exists(int seed)
        {
            const int maxElements = 32;
            var random = new TRandom(seed: seed);
            var numElements = random.Next(maxElements);
            var mockList = new Mock<ArrayList<int>>(args: new object[] {numElements}) {CallBase = true};
            var list = mockList.Object;
            mockList.Invocations.Clear();

            var adversarial = new List<int>(numElements);

            for (var i = 0; i < numElements; i++)
            {
                list.Add(i);
                adversarial.Add(i);
            }
            
            var adversarialBuff = new int[random.Next(maxElements * 2)];
            var buff = new int[adversarialBuff.Length];
            
            //fill up the buffers with random data
            void RandomFill()
            {
                for (var i = 0; i < buff.Length; i++)
                {
                    var n = random.Next(maxElements);
                    adversarialBuff[i] = n;
                    buff[i] = n;
                }
            }
            
            var element = new Boxed<int>(random.Next(maxElements));

            bool Predicate(int value)
            {
                return value == element;
            }
            
            var times = random.Next(maxElements);
            for (var _ = 0; _ < times; _++)
            {
                element.Value = random.Next(-2, maxElements + 2);
                RandomFill();
                Exception error = null;
                bool? expected = null;
                try
                {
                    expected = adversarial.Exists(Predicate);
                }
                catch (Exception e)
                {
                    error = e;
                }

                bool? res = null;
                try
                {
                    res = list.Exists(Predicate);
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    try
                    {
                        Assert.NotNull(error);
                        Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                    }
                    catch (XunitException exception)
                    {
                        throw new AggregateException(e, exception);
                    }
                }

                if (error is null)
                {
                    Assert.Equal(adversarialBuff, buff);
                }
                Assert.Equal(expected, res);

                Assert.Equal(adversarial, list);
                mockList.VerifyNoOtherCalls();
            }
        }
        
        
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_Find(int seed)
        {
            const int maxElements = 32;
            var random = new TRandom(seed: seed);
            var numElements = random.Next(maxElements);
            var mockList = new Mock<ArrayList<int>>(args: new object[] {numElements}) {CallBase = true};
            var list = mockList.Object;
            mockList.Invocations.Clear();

            var adversarial = new List<int>(numElements);

            for (var i = 0; i < numElements; i++)
            {
                list.Add(i);
                adversarial.Add(i);
            }
            
            var adversarialBuff = new int[random.Next(maxElements * 2)];
            var buff = new int[adversarialBuff.Length];
            
            //fill up the buffers with random data
            void RandomFill()
            {
                for (var i = 0; i < buff.Length; i++)
                {
                    var n = random.Next(maxElements);
                    adversarialBuff[i] = n;
                    buff[i] = n;
                }
            }
            
            var element = new Boxed<int>(random.Next(maxElements));

            bool Predicate(int value)
            {
                return value == element;
            }
            
            var times = random.Next(maxElements);
            for (var _ = 0; _ < times; _++)
            {
                element.Value = random.Next(-2, maxElements + 2);
                RandomFill();
                Exception error = null;
                int? expected = null;
                try
                {
                    expected = adversarial.Find(Predicate);
                }
                catch (Exception e)
                {
                    error = e;
                }

                int? res = null;
                try
                {
                    res = list.Find(Predicate);
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    try
                    {
                        Assert.NotNull(error);
                        Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                    }
                    catch (XunitException exception)
                    {
                        throw new AggregateException(e, exception);
                    }
                }

                if (error is null)
                {
                    Assert.Equal(adversarialBuff, buff);
                }
                Assert.Equal(expected, res);

                Assert.Equal(adversarial, list);
                mockList.VerifyNoOtherCalls();
            }
        }
        
        
        
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_FindIndex_With_StartIndex_And_Count(int seed)
        {
            const int maxElements = 32;
            var random = new TRandom(seed: seed);
            var numElements = random.Next(maxElements);
            var mockList = new Mock<ArrayList<int>>(args: new object[] {numElements}) {CallBase = true};
            var list = mockList.Object;
            mockList.Invocations.Clear();

            var adversarial = new List<int>(numElements);

            for (var i = 0; i < numElements; i++)
            {
                list.Add(i);
                adversarial.Add(i);
            }
            
            var adversarialBuff = new int[random.Next(maxElements * 2)];
            var buff = new int[adversarialBuff.Length];
            
            //fill up the buffers with random data
            void RandomFill()
            {
                for (var i = 0; i < buff.Length; i++)
                {
                    var n = random.Next(maxElements);
                    adversarialBuff[i] = n;
                    buff[i] = n;
                }
            }
            
            var element = new Boxed<int>(random.Next(maxElements));

            bool Predicate(int value)
            {
                return value == element;
            }
            
            var times = random.Next(maxElements);
            for (var _ = 0; _ < times; _++)
            {
                element.Value = random.Next(-2, maxElements + 2);
                RandomFill();
                var startIndex =  random.Next(-2, maxElements + 2);
                var count = random.Next(-2, adversarialBuff.Length + 2);
                
                Exception error = null;
                int? expected = null;
                try
                {
                    expected = adversarial.FindIndex(startIndex, count, Predicate);
                }
                catch (Exception e)
                {
                    error = e;
                }

                int? res = null;
                try
                {
                    res = list.FindIndex(startIndex, count, Predicate);
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    try
                    {
                        Assert.NotNull(error);
                        Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                    }
                    catch (XunitException exception)
                    {
                        throw new AggregateException(e, exception);
                    }
                }

                if (error is null)
                {
                    Assert.Equal(adversarialBuff, buff);
                }
                Assert.Equal(expected, res);

                Assert.Equal(adversarial, list);
                mockList.VerifyNoOtherCalls();
            }
        }
        
        
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_FindIndex_NoCount(int seed)
        {
            const int maxElements = 32;
            var random = new TRandom(seed: seed);
            var numElements = random.Next(maxElements);
            var mockList = new Mock<ArrayList<int>>(args: new object[] {numElements}) {CallBase = true};
            var list = mockList.Object;
            mockList.Invocations.Clear();

            var adversarial = new List<int>(numElements);

            for (var i = 0; i < numElements; i++)
            {
                list.Add(i);
                adversarial.Add(i);
            }
            
            var adversarialBuff = new int[random.Next(maxElements * 2)];
            var buff = new int[adversarialBuff.Length];
            
            //fill up the buffers with random data
            void RandomFill()
            {
                for (var i = 0; i < buff.Length; i++)
                {
                    var n = random.Next(maxElements);
                    adversarialBuff[i] = n;
                    buff[i] = n;
                }
            }
            
            var element = new Boxed<int>(random.Next(maxElements));

            bool Predicate(int value)
            {
                return value == element;
            }
            
            var times = random.Next(maxElements);
            for (var _ = 0; _ < times; _++)
            {
                element.Value = random.Next(-2, maxElements + 2);
                RandomFill();
                var startIndex =  random.Next(-2, maxElements + 2);

                Exception error = null;
                int? expected = null;
                try
                {
                    expected = adversarial.FindIndex(startIndex, Predicate);
                }
                catch (Exception e)
                {
                    error = e;
                }

                int? res = null;
                try
                {
                    res = list.FindIndex(startIndex, Predicate);
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    try
                    {
                        Assert.NotNull(error);
                        Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                    }
                    catch (XunitException exception)
                    {
                        throw new AggregateException(e, exception);
                    }
                }

                if (error is null)
                {
                    Assert.Equal(adversarialBuff, buff);
                }
                Assert.Equal(expected, res);

                Assert.Equal(adversarial, list);
                mockList.VerifyNoOtherCalls();
            }
        }
        
        
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_FindIndex(int seed)
        {
            const int maxElements = 32;
            var random = new TRandom(seed: seed);
            var numElements = random.Next(maxElements);
            var mockList = new Mock<ArrayList<int>>(args: new object[] {numElements}) {CallBase = true};
            var list = mockList.Object;
            mockList.Invocations.Clear();

            var adversarial = new List<int>(numElements);

            for (var i = 0; i < numElements; i++)
            {
                list.Add(i);
                adversarial.Add(i);
            }
            
            var adversarialBuff = new int[random.Next(maxElements * 2)];
            var buff = new int[adversarialBuff.Length];
            
            //fill up the buffers with random data
            void RandomFill()
            {
                for (var i = 0; i < buff.Length; i++)
                {
                    var n = random.Next(maxElements);
                    adversarialBuff[i] = n;
                    buff[i] = n;
                }
            }
            
            var element = new Boxed<int>(random.Next(maxElements));

            bool Predicate(int value)
            {
                return value == element;
            }
            
            var times = random.Next(maxElements);
            for (var _ = 0; _ < times; _++)
            {
                element.Value = random.Next(-2, maxElements + 2);
                RandomFill();

                Exception error = null;
                int? expected = null;
                try
                {
                    expected = adversarial.FindIndex(Predicate);
                }
                catch (Exception e)
                {
                    error = e;
                }

                int? res = null;
                try
                {
                    res = list.FindIndex(Predicate);
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    try
                    {
                        Assert.NotNull(error);
                        Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                    }
                    catch (XunitException exception)
                    {
                        throw new AggregateException(e, exception);
                    }
                }

                if (error is null)
                {
                    Assert.Equal(adversarialBuff, buff);
                }
                Assert.Equal(expected, res);

                Assert.Equal(adversarial, list);
                mockList.VerifyNoOtherCalls();
            }
        }
        
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_FindLast(int seed)
        {
            const int maxElements = 32;
            var random = new TRandom(seed: seed);
            var numElements = random.Next(maxElements);
            var mockList = new Mock<ArrayList<int>>(args: new object[] {numElements}) {CallBase = true};
            var list = mockList.Object;
            mockList.Invocations.Clear();

            var adversarial = new List<int>(numElements);

            for (var i = 0; i < numElements; i++)
            {
                list.Add(i);
                adversarial.Add(i);
            }
            
            var adversarialBuff = new int[random.Next(maxElements * 2)];
            var buff = new int[adversarialBuff.Length];
            
            //fill up the buffers with random data
            void RandomFill()
            {
                for (var i = 0; i < buff.Length; i++)
                {
                    var n = random.Next(maxElements);
                    adversarialBuff[i] = n;
                    buff[i] = n;
                }
            }
            
            var element = new Boxed<int>(random.Next(maxElements));

            bool Predicate(int value)
            {
                return value == element;
            }
            
            var times = random.Next(maxElements);
            for (var _ = 0; _ < times; _++)
            {
                element.Value = random.Next(-2, maxElements + 2);
                RandomFill();
                Exception error = null;
                int? expected = null;
                try
                {
                    expected = adversarial.FindLast(Predicate);
                }
                catch (Exception e)
                {
                    error = e;
                }

                int? res = null;
                try
                {
                    res = list.FindLast(Predicate);
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    try
                    {
                        Assert.NotNull(error);
                        Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                    }
                    catch (XunitException exception)
                    {
                        throw new AggregateException(e, exception);
                    }
                }

                if (error is null)
                {
                    Assert.Equal(adversarialBuff, buff);
                }
                Assert.Equal(expected, res);

                Assert.Equal(adversarial, list);
                mockList.VerifyNoOtherCalls();
            }
        }
        
        
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_FindLastIndex_With_StartIndex_And_Count(int seed)
        {
            const int maxElements = 32;
            var random = new TRandom(seed: seed);
            var numElements = random.Next(maxElements);
            var mockList = new Mock<ArrayList<int>>(args: new object[] {numElements}) {CallBase = true};
            var list = mockList.Object;
            mockList.Invocations.Clear();

            var adversarial = new List<int>(numElements);

            for (var i = 0; i < numElements; i++)
            {
                list.Add(i);
                adversarial.Add(i);
            }
            
            var adversarialBuff = new int[random.Next(maxElements * 2)];
            var buff = new int[adversarialBuff.Length];
            
            //fill up the buffers with random data
            void RandomFill()
            {
                for (var i = 0; i < buff.Length; i++)
                {
                    var n = random.Next(maxElements);
                    adversarialBuff[i] = n;
                    buff[i] = n;
                }
            }
            
            var element = new Boxed<int>(random.Next(maxElements));

            bool Predicate(int value)
            {
                return value == element;
            }
            
            var times = random.Next(maxElements);
            for (var _ = 0; _ < times; _++)
            {
                element.Value = random.Next(-2, maxElements + 2);
                RandomFill();
                var startIndex =  random.Next(-2, maxElements + 2);
                var count = random.Next(-2, adversarialBuff.Length + 2);
                
                Exception error = null;
                int? expected = null;
                try
                {
                    expected = adversarial.FindLastIndex(startIndex, count, Predicate);
                }
                catch (Exception e)
                {
                    error = e;
                }

                int? res = null;
                try
                {
                    res = list.FindLastIndex(startIndex, count, Predicate);
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    try
                    {
                        Assert.NotNull(error);
                        Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                    }
                    catch (XunitException exception)
                    {
                        throw new AggregateException(e, exception);
                    }
                }

                if (error is null)
                {
                    Assert.Equal(adversarialBuff, buff);
                }
                Assert.Equal(expected, res);

                Assert.Equal(adversarial, list);
                mockList.VerifyNoOtherCalls();
            }
        }
        
        
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_FindLastIndex_NoCount(int seed)
        {
            const int maxElements = 32;
            var random = new TRandom(seed: seed);
            var numElements = random.Next(maxElements);
            var mockList = new Mock<ArrayList<int>>(args: new object[] {numElements}) {CallBase = true};
            var list = mockList.Object;
            mockList.Invocations.Clear();

            var adversarial = new List<int>(numElements);

            for (var i = 0; i < numElements; i++)
            {
                list.Add(i);
                adversarial.Add(i);
            }
            
            var adversarialBuff = new int[random.Next(maxElements * 2)];
            var buff = new int[adversarialBuff.Length];
            
            //fill up the buffers with random data
            void RandomFill()
            {
                for (var i = 0; i < buff.Length; i++)
                {
                    var n = random.Next(maxElements);
                    adversarialBuff[i] = n;
                    buff[i] = n;
                }
            }
            
            var element = new Boxed<int>(random.Next(maxElements));

            bool Predicate(int value)
            {
                return value == element;
            }
            
            var times = random.Next(maxElements);
            for (var _ = 0; _ < times; _++)
            {
                element.Value = random.Next(-2, maxElements + 2);
                RandomFill();
                var startIndex =  random.Next(-2, maxElements + 2);

                Exception error = null;
                int? expected = null;
                try
                {
                    expected = adversarial.FindLastIndex(startIndex, Predicate);
                }
                catch (Exception e)
                {
                    error = e;
                }

                int? res = null;
                try
                {
                    res = list.FindLastIndex(startIndex, Predicate);
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    try
                    {
                        Assert.NotNull(error);
                        Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                    }
                    catch (XunitException exception)
                    {
                        throw new AggregateException(e, exception);
                    }
                }

                if (error is null)
                {
                    Assert.Equal(adversarialBuff, buff);
                }
                Assert.Equal(expected, res);

                Assert.Equal(adversarial, list);
                mockList.VerifyNoOtherCalls();
            }
        }
        
        
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_FindLastIndex(int seed)
        {
            const int maxElements = 32;
            var random = new TRandom(seed: seed);
            var numElements = random.Next(maxElements);
            var mockList = new Mock<ArrayList<int>>(args: new object[] {numElements}) {CallBase = true};
            var list = mockList.Object;
            mockList.Invocations.Clear();

            var adversarial = new List<int>(numElements);

            for (var i = 0; i < numElements; i++)
            {
                list.Add(i);
                adversarial.Add(i);
            }
            
            var adversarialBuff = new int[random.Next(maxElements * 2)];
            var buff = new int[adversarialBuff.Length];
            
            //fill up the buffers with random data
            void RandomFill()
            {
                for (var i = 0; i < buff.Length; i++)
                {
                    var n = random.Next(maxElements);
                    adversarialBuff[i] = n;
                    buff[i] = n;
                }
            }
            
            var element = new Boxed<int>(random.Next(maxElements));

            bool Predicate(int value)
            {
                return value == element;
            }
            
            var times = random.Next(maxElements);
            for (var _ = 0; _ < times; _++)
            {
                element.Value = random.Next(-2, maxElements + 2);
                RandomFill();

                Exception error = null;
                int? expected = null;
                try
                {
                    expected = adversarial.FindLastIndex(Predicate);
                }
                catch (Exception e)
                {
                    error = e;
                }

                int? res = null;
                try
                {
                    res = list.FindLastIndex(Predicate);
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    try
                    {
                        Assert.NotNull(error);
                        Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                    }
                    catch (XunitException exception)
                    {
                        throw new AggregateException(e, exception);
                    }
                }

                if (error is null)
                {
                    Assert.Equal(adversarialBuff, buff);
                }
                Assert.Equal(expected, res);

                Assert.Equal(adversarial, list);
                mockList.VerifyNoOtherCalls();
            }
        }
        
        
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_ForEach(int seed)
        {
            const int maxElements = 32;
            var random = new TRandom(seed: seed);
            var numElements = random.Next(maxElements);
            var mockList = new Mock<ArrayList<int>>(args: new object[] {numElements}) {CallBase = true};
            var list = mockList.Object;
            mockList.Invocations.Clear();

            var adversarial = new List<int>(numElements);

            for (var i = 0; i < numElements; i++)
            {
                list.Add(i);
                adversarial.Add(i);
            }
            
            var adversarialBuff = new int[random.Next(maxElements * 2)];
            var buff = new int[adversarialBuff.Length];
            
            //fill up the buffers with random data
            void RandomFill()
            {
                for (var i = 0; i < buff.Length; i++)
                {
                    var n = random.Next(maxElements);
                    adversarialBuff[i] = n;
                    buff[i] = n;
                }
            }
            
            var element = new Boxed<int>(0);

            void Action(int value)
            {
                element.Value ^= value;
            }
            
            var times = random.Next(maxElements);
            for (var _ = 0; _ < times; _++)
            {
                RandomFill();

                Exception error = null;
                int? expected = null;
                try
                {
                    adversarial.ForEach(Action);
                    expected = element;
                }
                catch (Exception e)
                {
                    error = e;
                }
                finally
                {
                    element.Value = 0;
                }

                int? res = null;
                try
                {
                    list.ForEach(Action);
                    res = element;
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    try
                    {
                        Assert.NotNull(error);
                        Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                    }
                    catch (XunitException exception)
                    {
                        throw new AggregateException(e, exception);
                    }
                }
                finally
                {
                    element.Value = 0;
                }

                if (error is null)
                {
                    Assert.Equal(adversarialBuff, buff);
                }
                Assert.Equal(expected, res);

                Assert.Equal(adversarial, list);
                mockList.VerifyNoOtherCalls();
            }
        }
        
        
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_IndexOf_With_StartIndex_And_Count(int seed)
        {
            const int maxElements = 32;
            var random = new TRandom(seed: seed);
            var numElements = random.Next(maxElements);
            var mockList = new Mock<ArrayList<int>>(args: new object[] {numElements}) {CallBase = true};
            var list = mockList.Object;
            mockList.Invocations.Clear();

            var adversarial = new List<int>(numElements);

            for (var i = 0; i < numElements; i++)
            {
                list.Add(i);
                adversarial.Add(i);
            }
            
            var adversarialBuff = new int[random.Next(maxElements * 2)];
            var buff = new int[adversarialBuff.Length];
            
            //fill up the buffers with random data
            void RandomFill()
            {
                for (var i = 0; i < buff.Length; i++)
                {
                    var n = random.Next(maxElements);
                    adversarialBuff[i] = n;
                    buff[i] = n;
                }
            }
            
            var element = new Boxed<int>(random.Next(maxElements));

            var times = random.Next(maxElements);
            for (var _ = 0; _ < times; _++)
            {
                element.Value = random.Next(-2, maxElements + 2);
                RandomFill();
                var startIndex =  random.Next(-2, maxElements + 2);
                var count = random.Next(-2, adversarialBuff.Length + 2);
                
                Exception error = null;
                int? expected = null;
                try
                {
                    expected = adversarial.IndexOf(element, startIndex, count);
                }
                catch (Exception e)
                {
                    error = e;
                }

                int? res = null;
                try
                {
                    res = list.IndexOf(element, startIndex, count);
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    try
                    {
                        Assert.NotNull(error);
                        Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                    }
                    catch (XunitException exception)
                    {
                        throw new AggregateException(e, exception);
                    }
                }

                if (error is null)
                {
                    Assert.Equal(adversarialBuff, buff);
                }
                Assert.Equal(expected, res);

                Assert.Equal(adversarial, list);
                mockList.VerifyNoOtherCalls();
            }
        }


        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_InsertRange_Collection(int seed)
        {
            const int maxElements = 32;
            var random = new TRandom(seed: seed);
            var numElements = random.Next(maxElements);
            var mockList = new Mock<ArrayList<int>>(args: new object[] {numElements}) {CallBase = true};
            var list = mockList.Object;
            mockList.Invocations.Clear();

            var adversarial = new List<int>(numElements);

            for (var i = 0; i < numElements; i++)
            {
                list.Add(i);
                adversarial.Add(i);
            }
            
            var adversarialBuff = new int[random.Next(maxElements * 2)];
            var buff = new int[adversarialBuff.Length];
            
            //fill up the buffers with random data
            void RandomFill()
            {
                for (var i = 0; i < buff.Length; i++)
                {
                    var n = random.Next(maxElements);
                    adversarialBuff[i] = n;
                    buff[i] = n;
                }
            }

            var times = random.Next(maxElements);
            for (var _ = 0; _ < times; _++)
            {
                RandomFill();
                var index =  random.Next(-2, maxElements + 2);

                IList<int> randomCollection;
                if (random.NextDouble() < 0.8)
                {
                    var capacity = random.Next(maxElements);
                    randomCollection = new List<int>(capacity);

                    for (var i = 0; i < capacity; i++)
                    {
                        randomCollection.Add(random.Next());
                    }
                }
                else
                {
                    randomCollection = null; // => insert into self
                }
                
                

                Exception error = null;
                try
                {
                    adversarial.InsertRange(index, randomCollection ?? adversarial);
                }
                catch (Exception e)
                {
                    error = e;
                }
                
                try
                {
                    list.InsertRange<IList<int>>(index, randomCollection ?? list);
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    try
                    {
                        Assert.NotNull(error);
                        Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                    }
                    catch (XunitException exception)
                    {
                        throw new AggregateException(e, exception);
                    }
                }

                if (error is null)
                {
                    Assert.Equal(adversarialBuff, buff);
                }

                Assert.Equal(adversarial, list);
                mockList.Invocations.Clear();
            }
        }
        
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_InsertRange_ReadonlyCollection(int seed)
        {
            const int maxElements = 32;
            var random = new TRandom(seed: seed);
            var numElements = random.Next(maxElements);
            var mockList = new Mock<ArrayList<int>>(args: new object[] {numElements}) {CallBase = true};
            var list = mockList.Object;
            mockList.Invocations.Clear();

            var adversarial = new List<int>(numElements);

            for (var i = 0; i < numElements; i++)
            {
                list.Add(i);
                adversarial.Add(i);
            }
            
            var adversarialBuff = new int[random.Next(maxElements * 2)];
            var buff = new int[adversarialBuff.Length];
            
            //fill up the buffers with random data
            void RandomFill()
            {
                for (var i = 0; i < buff.Length; i++)
                {
                    var n = random.Next(maxElements);
                    adversarialBuff[i] = n;
                    buff[i] = n;
                }
            }

            var times = random.Next(maxElements);
            for (var _ = 0; _ < times; _++)
            {
                RandomFill();
                var index =  random.Next(-2, maxElements + 2);

                IReadOnlyCollection<int> randomCollection;
                if (random.NextDouble() < 0.8)
                {
                    var capacity = random.Next(maxElements);
                    randomCollection = new List<int>(capacity);

                    for (var i = 0; i < capacity; i++)
                    {
                        ((IList) randomCollection).Add(random.Next());
                    }
                }
                else
                {
                    randomCollection = null; // => insert into self
                }
                
                

                Exception error = null;
                try
                {
                    adversarial.InsertRange(index, randomCollection ?? adversarial);
                }
                catch (Exception e)
                {
                    error = e;
                }
                
                try
                {
                    // ReSharper disable once RedundantCast
                    list.InsertRange(index, (IReadOnlyCollection<int>) (randomCollection ?? list));
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    try
                    {
                        Assert.NotNull(error);
                        Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                    }
                    catch (XunitException exception)
                    {
                        throw new AggregateException(e, exception);
                    }
                }

                if (error is null)
                {
                    Assert.Equal(adversarialBuff, buff);
                }

                Assert.Equal(adversarial, list);
                mockList.Invocations.Clear();
            }
        }
        
        
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_InsertRange_IEnumerable(int seed)
        {
            const int maxElements = 32;
            var random = new TRandom(seed: seed);
            var numElements = random.Next(maxElements);
            var mockList = new Mock<ArrayList<int>>(args: new object[] {numElements}) {CallBase = true};
            var list = mockList.Object;
            mockList.Invocations.Clear();

            var adversarial = new List<int>(numElements);

            for (var i = 0; i < numElements; i++)
            {
                list.Add(i);
                adversarial.Add(i);
            }
            
            var adversarialBuff = new int[random.Next(maxElements * 2)];
            var buff = new int[adversarialBuff.Length];
            
            //fill up the buffers with random data
            void RandomFill()
            {
                for (var i = 0; i < buff.Length; i++)
                {
                    var n = random.Next(maxElements);
                    adversarialBuff[i] = n;
                    buff[i] = n;
                }
            }

            var times = random.Next(maxElements);
            for (var _ = 0; _ < times; _++)
            {
                RandomFill();
                var index =  random.Next(-2, maxElements + 2);

                IList<int> randomCollection;
                if (random.NextDouble() < 0.8)
                {
                    var capacity = random.Next(maxElements);
                    randomCollection = new List<int>(capacity);

                    for (var i = 0; i < capacity; i++)
                    {
                        ((IList) randomCollection).Add(random.Next());
                    }
                }
                else
                {
                    randomCollection = null; // => insert into self
                }
                
                

                Exception error = null;
                try
                {
                    adversarial.InsertRange(index, randomCollection ?? adversarial);
                }
                catch (Exception e)
                {
                    error = e;
                }
                
                try
                {
                    // ReSharper disable once RedundantCast
                    list.InsertRange(index, (IEnumerable<int>) (randomCollection ?? list));
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    try
                    {
                        Assert.NotNull(error);
                        Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                    }
                    catch (XunitException exception)
                    {
                        throw new AggregateException(e, exception);
                    }
                }

                if (error is null)
                {
                    Assert.Equal(adversarialBuff, buff);
                }

                Assert.Equal(adversarial, list);
                mockList.Invocations.Clear();
            }
        }
        
        [Fact]
        public void Test_Regressions()
        {
            Test_FindIndex(seed: 269155879);
            Test_FindIndex_NoCount(seed: 2043079967);
            Test_FindIndex_With_StartIndex_And_Count(seed: 1306383154);

        }
        
        
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_GetRange(int seed)
        {
            const int maxElements = 32;
            var random = new TRandom(seed: seed);
            var numElements = random.Next(maxElements);
            var mockList = new Mock<ArrayList<int>>(args: new object[] {numElements}) {CallBase = true};
            var list = mockList.Object;
            mockList.Invocations.Clear();

            var adversarial = new List<int>(numElements);

            for (var i = 0; i < numElements; i++)
            {
                list.Add(i);
                adversarial.Add(i);
            }
            
            void RandomFill()
            {
                for (var i = 0; i < list.Count; i++)
                {
                    var n = random.Next();
                    list[i] = n;
                    adversarial[i] = n;
                }
            }
            
            var times = random.Next(maxElements);
            for (var _ = 0; _ < times; _++)
            {
                RandomFill();
                Exception error = null;
                var index =  random.Next(-2, maxElements + 2);
                var count = random.Next(-2, maxElements + 2);

                IList<int> expected = null; 
                try
                {
                    expected = adversarial.GetRange(index, count);
                }
                catch (Exception e)
                {
                    error = e;
                }

                ArrayList<int> res = null;
                try
                {
                    res = list.GetRange(index, count);
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    try
                    {
                        Assert.NotNull(error);
                        Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                    }
                    catch (XunitException exception)
                    {
                        throw new AggregateException(e, exception);
                    }
                }

                if (error is null)
                {
                    Assert.Equal(expected, res);
                }


                Assert.Equal(adversarial, list);
                mockList.VerifyNoOtherCalls();
            }
        }
        
        internal class RandomSeedGenerator : IEnumerable<object[]>
        {
            internal virtual RandomThreadSafe Random { get; } = new RandomThreadSafe();
            internal virtual int NumberOfGen => 64;

            public IEnumerator<object[]> GetEnumerator()
            {
                for (var i = 0; i < NumberOfGen; i++)
                {
                    yield return new object[] {Random.Next()};
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}