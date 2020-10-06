using System;
using System.Collections;
using System.Collections.Generic;
using Maxisoft.Utils.Collections.Lists;
using Maxisoft.Utils.Objects;
using Maxisoft.Utils.Random;
using Moq;
using Troschuetz.Random;
using Xunit;
using Xunit.Sdk;

namespace Maxisoft.Utils.Tests.Collections.Lists
{
    public class ArrayListFuzzingTests
    {
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_Contains_Object(int seed)
        {
            var random = new TRandom(seed);
            var mockPool = new Dictionary<EquatableObject, Mock<EquatableObject>>();
            var adversarial = new List<EquatableObject>();

            var initialNumObjects = random.Next(32);
            adversarial.Capacity = initialNumObjects;
            for (var i = 0; i < initialNumObjects; i++)
            {
                if (i == 0 || random.NextDouble() < 0.8)
                {
                    var mock = new Mock<EquatableObject> {CallBase = true};
                    Assert.NotNull(mock.Object);
                    mockPool.Add(mock.Object, mock);
                    adversarial.Add(mock.Object);
                }
                else // insert duplicate
                {
                    var index = random.Next(adversarial.Count);
                    var mock = mockPool[adversarial[index]];
                    Assert.NotNull(mock.Object);
                    adversarial.Add(mock.Object);
                }
            }

            var listMock = new Mock<ArrayList<EquatableObject>> {CallBase = true};
            var list = listMock.Object;
            list.InsertRange(0, adversarial);

            Assert.Equal(adversarial, list);
            Assert.Equal(adversarial.Count, list.Count);

            void ClearInvocations()
            {
                foreach (var pair in mockPool)
                {
                    pair.Value.Invocations.Clear();
                }

                listMock.Invocations.Clear();
            }

            for (var i = 0; i < initialNumObjects; i++)
            {
                ClearInvocations();
                var rnd = random.Next(4);
                var itemToSeek = rnd switch
                {
                    0 => new EquatableObject(),
                    1 => adversarial[i],
                    2 => adversarial[random.Next(adversarial.Count)],
                    _ => null
                };

                var expected = adversarial.Contains(itemToSeek);
                var res = list.Contains(itemToSeek);
                Assert.Equal(expected, res);
                if (itemToSeek is {} && mockPool.TryGetValue(itemToSeek, out var eqMock))
                {
                    eqMock.Verify(mock => mock.Equals(It.IsAny<EquatableObject>()), Times.Exactly(3));
                    eqMock.Verify(mock => mock.GetHashCode());
                    eqMock.VerifyNoOtherCalls();
                }
            }
        }

        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_CopyTo(int seed)
        {
            var random = new TRandom(seed);
            var mockPool = new Dictionary<EquatableObject, Mock<EquatableObject>>();
            var adversarial = new List<EquatableObject>();

            var initialNumObjects = random.Next(32);
            adversarial.Capacity = initialNumObjects;
            for (var i = 0; i < initialNumObjects; i++)
            {
                if (i == 0 || random.NextDouble() < 0.8)
                {
                    var mock = new Mock<EquatableObject> {CallBase = true};
                    Assert.NotNull(mock.Object);
                    mockPool.Add(mock.Object, mock);
                    adversarial.Add(mock.Object);
                }
                else // insert duplicate
                {
                    var index = random.Next(adversarial.Count);
                    var mock = mockPool[adversarial[index]];
                    Assert.NotNull(mock.Object);
                    adversarial.Add(mock.Object);
                }
            }

            var listMock = new Mock<ArrayList<EquatableObject>> {CallBase = true};
            var list = listMock.Object;
            list.InsertRange(0, adversarial);

            Assert.Equal(adversarial, list);
            Assert.Equal(adversarial.Count, list.Count);

            for (var i = 0; i < initialNumObjects; i++)
            {
                var index = random.Next(-2, initialNumObjects + 2);
                Exception error = null;
                var adversarialBuffer = adversarial.ToArray();
                Array.Clear(adversarialBuffer, 0, adversarialBuffer.Length);
                try
                {
                    adversarial.CopyTo(adversarialBuffer, index);
                }
                catch (Exception e)
                {
                    error = e;
                }


                var buffer = new EquatableObject[list.Count];
                try
                {
                    list.CopyTo(buffer, index);
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    Assert.NotNull(error);
                    Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                }

                Assert.Equal(adversarialBuffer, buffer);
            }
        }

        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_Remove_Object(int seed)
        {
            var random = new TRandom(seed);
            var mockPool = new Dictionary<EquatableObject, Mock<EquatableObject>>();
            var adversarial = new List<EquatableObject>();

            var initialNumObjects = random.Next(32);
            adversarial.Capacity = initialNumObjects;
            for (var i = 0; i < initialNumObjects; i++)
            {
                if (i == 0 || random.NextDouble() < 0.8)
                {
                    var mock = new Mock<EquatableObject> {CallBase = true};
                    Assert.NotNull(mock.Object);
                    mockPool.Add(mock.Object, mock);
                    adversarial.Add(mock.Object);
                }
                else // insert duplicate
                {
                    var index = random.Next(adversarial.Count);
                    var mock = mockPool[adversarial[index]];
                    Assert.NotNull(mock.Object);
                    adversarial.Add(mock.Object);
                }
            }

            var listMock = new Mock<ArrayList<EquatableObject>> {CallBase = true};
            var list = listMock.Object;
            list.InsertRange(0, adversarial);

            Assert.Equal(adversarial, list);
            Assert.Equal(adversarial.Count, list.Count);

            void ClearInvocations()
            {
                foreach (var pair in mockPool)
                {
                    pair.Value.Invocations.Clear();
                }

                listMock.Invocations.Clear();
            }

            for (var i = 0; i < initialNumObjects * 5; i++)
            {
                ClearInvocations();
                var index = random.Next(-2, initialNumObjects + 2);
                EquatableObject obj;
                try
                {
                    obj = adversarial[index];
                }
                catch (Exception e)
                {
                    obj = random.NextBoolean() ? null : new EquatableObject();
                }

                Exception error = null;
                bool? expected = null;
                try
                {
                    expected = adversarial.Remove(obj);
                }
                catch (Exception e)
                {
                    error = e;
                }

                bool? res = null;
                try
                {
                    res = list.Remove(obj);
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    Assert.NotNull(error);
                    Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                }

                Assert.Equal(expected, res);
                Assert.Equal(adversarial, list);
                listMock.VerifyNoOtherCalls();


                if (obj is {} && mockPool.TryGetValue(obj, out var eqMock))
                {
                    eqMock.Verify(mock => mock.Equals(It.IsAny<EquatableObject>()), Times.AtLeast(3));
                    eqMock.Verify(mock => mock.GetHashCode());
                    eqMock.VerifyNoOtherCalls();
                }
            }
        }


        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_Indexof_Object(int seed)
        {
            var random = new TRandom(seed);
            var mockPool = new Dictionary<EquatableObject, Mock<EquatableObject>>();
            var adversarial = new List<EquatableObject>();

            var initialNumObjects = random.Next(32);
            adversarial.Capacity = initialNumObjects;
            for (var i = 0; i < initialNumObjects; i++)
            {
                if (i == 0 || random.NextDouble() < 0.8)
                {
                    var mock = new Mock<EquatableObject> {CallBase = true};
                    Assert.NotNull(mock.Object);
                    mockPool.Add(mock.Object, mock);
                    adversarial.Add(mock.Object);
                }
                else // insert duplicate
                {
                    var index = random.Next(adversarial.Count);
                    var mock = mockPool[adversarial[index]];
                    Assert.NotNull(mock.Object);
                    adversarial.Add(mock.Object);
                }
            }

            var listMock = new Mock<ArrayList<EquatableObject>> {CallBase = true};
            var list = listMock.Object;
            list.InsertRange(0, adversarial);

            Assert.Equal(adversarial, list);
            Assert.Equal(adversarial.Count, list.Count);

            void ClearInvocations()
            {
                foreach (var pair in mockPool)
                {
                    pair.Value.Invocations.Clear();
                }

                listMock.Invocations.Clear();
            }

            for (var i = 0; i < initialNumObjects; i++)
            {
                ClearInvocations();
                var index = random.Next(-2, initialNumObjects + 2);
                EquatableObject obj;
                try
                {
                    obj = adversarial[index];
                }
                catch (Exception)
                {
                    obj = null;
                }

                Exception error = null;
                int? expected = null;
                try
                {
                    expected = adversarial.IndexOf(obj);
                }
                catch (Exception e)
                {
                    error = e;
                }


                int? res = null;
                try
                {
                    res = list.IndexOf(obj);
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    Assert.NotNull(error);
                    Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                }

                Assert.Equal(expected, res);
                Assert.Equal(adversarial, list);

                if (obj is {} && mockPool.TryGetValue(obj, out var eqMock))
                {
                    eqMock.Verify(mock => mock.Equals(It.IsAny<EquatableObject>()), Times.AtLeast(3));
                    eqMock.Verify(mock => mock.GetHashCode());
                    eqMock.VerifyNoOtherCalls();
                }
            }
        }

        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_Insert_Object(int seed)
        {
            var random = new TRandom(seed);
            var mockPool = new Dictionary<EquatableObject, Mock<EquatableObject>>();
            var adversarial = new List<EquatableObject>();

            var initialNumObjects = random.Next(32);
            adversarial.Capacity = initialNumObjects;
            for (var i = 0; i < initialNumObjects; i++)
            {
                if (i == 0 || random.NextDouble() < 0.8)
                {
                    var mock = new Mock<EquatableObject> {CallBase = true};
                    Assert.NotNull(mock.Object);
                    mockPool.Add(mock.Object, mock);
                    adversarial.Add(mock.Object);
                }
                else // insert duplicate
                {
                    var index = random.Next(adversarial.Count);
                    var mock = mockPool[adversarial[index]];
                    Assert.NotNull(mock.Object);
                    adversarial.Add(mock.Object);
                }
            }

            var listMock = new Mock<ArrayList<EquatableObject>> {CallBase = true};
            var list = listMock.Object;
            list.InsertRange(0, adversarial);

            Assert.Equal(adversarial, list);
            Assert.Equal(adversarial.Count, list.Count);

            void ClearInvocations()
            {
                foreach (var pair in mockPool)
                {
                    pair.Value.Invocations.Clear();
                }

                listMock.Invocations.Clear();
            }

            for (var i = 0; i < initialNumObjects; i++)
            {
                ClearInvocations();
                var index = random.Next(-2, adversarial.Count + 2);
                var insertObj = random.NextDouble() < 0.8 ? new EquatableObject() : null;

                Exception error = null;
                try
                {
                    adversarial.Insert(index, insertObj);
                }
                catch (Exception e)
                {
                    error = e;
                }


                try
                {
                    list.Insert(index, insertObj);
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    Assert.NotNull(error);
                    Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                }

                Assert.Equal(adversarial, list);
            }
        }


        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_RemoveAt_Object(int seed)
        {
            var random = new TRandom(seed);
            var mockPool = new Dictionary<EquatableObject, Mock<EquatableObject>>();
            var adversarial = new List<EquatableObject>();

            var initialNumObjects = random.Next(32);
            adversarial.Capacity = initialNumObjects;
            for (var i = 0; i < initialNumObjects; i++)
            {
                if (i == 0 || random.NextDouble() < 0.8)
                {
                    var mock = new Mock<EquatableObject> {CallBase = true};
                    Assert.NotNull(mock.Object);
                    mockPool.Add(mock.Object, mock);
                    adversarial.Add(mock.Object);
                }
                else // insert duplicate
                {
                    var index = random.Next(adversarial.Count);
                    var mock = mockPool[adversarial[index]];
                    Assert.NotNull(mock.Object);
                    adversarial.Add(mock.Object);
                }
            }

            var listMock = new Mock<ArrayList<EquatableObject>> {CallBase = true};
            var list = listMock.Object;
            list.InsertRange(0, adversarial);

            Assert.Equal(adversarial, list);
            Assert.Equal(adversarial.Count, list.Count);

            void ClearInvocations()
            {
                foreach (var pair in mockPool)
                {
                    pair.Value.Invocations.Clear();
                }

                listMock.Invocations.Clear();
            }

            for (var i = 0; i < initialNumObjects * 5; i++)
            {
                ClearInvocations();
                var index = random.Next(-2, initialNumObjects + 2);

                Exception error = null;
                try
                {
                    adversarial.RemoveAt(index);
                }
                catch (Exception e)
                {
                    error = e;
                }

                try
                {
                    list.RemoveAt(index);
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    Assert.NotNull(error);
                    Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                }

                Assert.Equal(adversarial, list);
                listMock.VerifyNoOtherCalls();
            }
        }


        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_Indexer_Get_Object(int seed)
        {
            var random = new TRandom(seed);
            var mockPool = new Dictionary<EquatableObject, Mock<EquatableObject>>();
            var adversarial = new List<EquatableObject>();

            var initialNumObjects = random.Next(32);
            adversarial.Capacity = initialNumObjects;
            for (var i = 0; i < initialNumObjects; i++)
            {
                if (i == 0 || random.NextDouble() < 0.8)
                {
                    var mock = new Mock<EquatableObject> {CallBase = true};
                    Assert.NotNull(mock.Object);
                    mockPool.Add(mock.Object, mock);
                    adversarial.Add(mock.Object);
                }
                else // insert duplicate
                {
                    var index = random.Next(adversarial.Count);
                    var mock = mockPool[adversarial[index]];
                    Assert.NotNull(mock.Object);
                    adversarial.Add(mock.Object);
                }
            }

            var listMock = new Mock<ArrayList<EquatableObject>> {CallBase = true};
            var list = listMock.Object;
            list.InsertRange(0, adversarial);

            Assert.Equal(adversarial, list);
            Assert.Equal(adversarial.Count, list.Count);

            void ClearInvocations()
            {
                foreach (var pair in mockPool)
                {
                    pair.Value.Invocations.Clear();
                }

                listMock.Invocations.Clear();
            }

            for (var i = 0; i < initialNumObjects * 5; i++)
            {
                ClearInvocations();
                var index = random.Next(-2, initialNumObjects + 2);
                EquatableObject obj = null;
                Exception error = null;
                try
                {
                    obj = adversarial[index];
                }
                catch (Exception e)
                {
                    error = e;
                }

                EquatableObject res = null;
                try
                {
                    res = list[index];
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    Assert.NotNull(error);
                    Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                }

                Assert.Equal(obj, res);

                Assert.Equal(adversarial, list);
                listMock.VerifyNoOtherCalls();


                if (obj is {} && mockPool.TryGetValue(obj, out var eqMock))
                {
                    eqMock.Verify(mock => mock.Equals(It.IsAny<EquatableObject>()), Times.AtLeast(1));
                    eqMock.Verify(mock => mock.GetHashCode());
                    eqMock.VerifyNoOtherCalls();
                }
            }
        }
        
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_IList_Indexer_Get_Object(int seed)
        {
            var random = new TRandom(seed);
            var mockPool = new Dictionary<EquatableObject, Mock<EquatableObject>>();
            var adversarial = new List<EquatableObject>();

            var initialNumObjects = random.Next(32);
            adversarial.Capacity = initialNumObjects;
            for (var i = 0; i < initialNumObjects; i++)
            {
                if (i == 0 || random.NextDouble() < 0.8)
                {
                    var mock = new Mock<EquatableObject> {CallBase = true};
                    Assert.NotNull(mock.Object);
                    mockPool.Add(mock.Object, mock);
                    adversarial.Add(mock.Object);
                }
                else // insert duplicate
                {
                    var index = random.Next(adversarial.Count);
                    var mock = mockPool[adversarial[index]];
                    Assert.NotNull(mock.Object);
                    adversarial.Add(mock.Object);
                }
            }

            var listMock = new Mock<ArrayList<EquatableObject>> {CallBase = true};
            var list = listMock.Object;
            list.InsertRange(0, adversarial);

            Assert.Equal(adversarial, list);
            Assert.Equal(adversarial.Count, list.Count);

            void ClearInvocations()
            {
                foreach (var pair in mockPool)
                {
                    pair.Value.Invocations.Clear();
                }

                listMock.Invocations.Clear();
            }

            for (var i = 0; i < initialNumObjects * 5; i++)
            {
                ClearInvocations();
                var index = random.Next(-2, initialNumObjects + 2);
                EquatableObject obj = null;
                Exception error = null;
                try
                {
                    obj = ((IList<EquatableObject>) adversarial)[index];
                }
                catch (Exception e)
                {
                    error = e;
                }

                EquatableObject res = null;
                try
                {
                    res = ((IList<EquatableObject>)list)[index];
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    Assert.NotNull(error);
                    Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                }

                Assert.Equal(obj, res);

                Assert.Equal(adversarial, list);
                listMock.VerifyNoOtherCalls();


                if (obj is {} && mockPool.TryGetValue(obj, out var eqMock))
                {
                    eqMock.Verify(mock => mock.Equals(It.IsAny<EquatableObject>()), Times.AtLeast(1));
                    eqMock.Verify(mock => mock.GetHashCode());
                    eqMock.VerifyNoOtherCalls();
                }
            }
        }
        
        
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_IReadOnlyList_Indexer_Get_Object(int seed)
        {
            var random = new TRandom(seed);
            var mockPool = new Dictionary<EquatableObject, Mock<EquatableObject>>();
            var adversarial = new List<EquatableObject>();

            var initialNumObjects = random.Next(32);
            adversarial.Capacity = initialNumObjects;
            for (var i = 0; i < initialNumObjects; i++)
            {
                if (i == 0 || random.NextDouble() < 0.8)
                {
                    var mock = new Mock<EquatableObject> {CallBase = true};
                    Assert.NotNull(mock.Object);
                    mockPool.Add(mock.Object, mock);
                    adversarial.Add(mock.Object);
                }
                else // insert duplicate
                {
                    var index = random.Next(adversarial.Count);
                    var mock = mockPool[adversarial[index]];
                    Assert.NotNull(mock.Object);
                    adversarial.Add(mock.Object);
                }
            }

            var listMock = new Mock<ArrayList<EquatableObject>> {CallBase = true};
            var list = listMock.Object;
            list.InsertRange(0, adversarial);

            Assert.Equal(adversarial, list);
            Assert.Equal(adversarial.Count, list.Count);

            void ClearInvocations()
            {
                foreach (var pair in mockPool)
                {
                    pair.Value.Invocations.Clear();
                }

                listMock.Invocations.Clear();
            }

            for (var i = 0; i < initialNumObjects * 5; i++)
            {
                ClearInvocations();
                var index = random.Next(-2, initialNumObjects + 2);
                EquatableObject obj = null;
                Exception error = null;
                try
                {
                    obj = ((IReadOnlyList<EquatableObject>) adversarial)[index];
                }
                catch (Exception e)
                {
                    error = e;
                }

                EquatableObject res = null;
                try
                {
                    res = ((IReadOnlyList<EquatableObject>) list)[index];
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    Assert.NotNull(error);
                    Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                }

                Assert.Equal(obj, res);

                Assert.Equal(adversarial, list);
                listMock.VerifyNoOtherCalls();


                if (obj is {} && mockPool.TryGetValue(obj, out var eqMock))
                {
                    eqMock.Verify(mock => mock.Equals(It.IsAny<EquatableObject>()), Times.AtLeast(1));
                    eqMock.Verify(mock => mock.GetHashCode());
                    eqMock.VerifyNoOtherCalls();
                }
            }
        }


        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_Indexer_Set_Object(int seed)
        {
            var random = new TRandom(seed);
            var mockPool = new Dictionary<EquatableObject, Mock<EquatableObject>>();
            var adversarial = new List<EquatableObject>();

            var initialNumObjects = random.Next(32);
            adversarial.Capacity = initialNumObjects;
            for (var i = 0; i < initialNumObjects; i++)
            {
                if (i == 0 || random.NextDouble() < 0.8)
                {
                    var mock = new Mock<EquatableObject> {CallBase = true};
                    Assert.NotNull(mock.Object);
                    mockPool.Add(mock.Object, mock);
                    adversarial.Add(mock.Object);
                }
                else // insert duplicate
                {
                    var index = random.Next(adversarial.Count);
                    var mock = mockPool[adversarial[index]];
                    Assert.NotNull(mock.Object);
                    adversarial.Add(mock.Object);
                }
            }

            var listMock = new Mock<ArrayList<EquatableObject>> {CallBase = true};
            var list = listMock.Object;
            list.InsertRange(0, adversarial);

            Assert.Equal(adversarial, list);
            Assert.Equal(adversarial.Count, list.Count);

            void ClearInvocations()
            {
                foreach (var pair in mockPool)
                {
                    pair.Value.Invocations.Clear();
                }

                listMock.Invocations.Clear();
            }

            for (var i = 0; i < initialNumObjects * 5; i++)
            {
                ClearInvocations();
                var index = random.Next(-2, initialNumObjects + 2);
                EquatableObject obj = null;
                if (random.NextDouble() < 0.8)
                {
                    var mock = new Mock<EquatableObject> {CallBase = true};
                    mockPool.Add(mock.Object, mock);
                    obj = mock.Object;
                }

                Exception error = null;
                try
                {
                    adversarial[index] = obj;
                }
                catch (Exception e)
                {
                    error = e;
                }

                try
                {
                    list[index] = obj;
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    Assert.NotNull(error);
                    Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                }

                if (error is null)
                {
                    Assert.Equal(obj, list[index]);
                    Assert.Equal(adversarial[index], list[index]);
                }


                Assert.Equal(adversarial, list);
                listMock.VerifyNoOtherCalls();


                if (obj is {} && mockPool.TryGetValue(obj, out var eqMock))
                {
                    eqMock.Verify(mock => mock.Equals(It.IsAny<EquatableObject>()), Times.AtLeast(1));
                    eqMock.Verify(mock => mock.GetHashCode());
                    eqMock.VerifyNoOtherCalls();
                }
            }
        }
        
        
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_IList_Indexer_Set_Object(int seed)
        {
            var random = new TRandom(seed);
            var mockPool = new Dictionary<EquatableObject, Mock<EquatableObject>>();
            var adversarial = new List<EquatableObject>();

            var initialNumObjects = random.Next(32);
            adversarial.Capacity = initialNumObjects;
            for (var i = 0; i < initialNumObjects; i++)
            {
                if (i == 0 || random.NextDouble() < 0.8)
                {
                    var mock = new Mock<EquatableObject> {CallBase = true};
                    Assert.NotNull(mock.Object);
                    mockPool.Add(mock.Object, mock);
                    adversarial.Add(mock.Object);
                }
                else // insert duplicate
                {
                    var index = random.Next(adversarial.Count);
                    var mock = mockPool[adversarial[index]];
                    Assert.NotNull(mock.Object);
                    adversarial.Add(mock.Object);
                }
            }

            var listMock = new Mock<ArrayList<EquatableObject>> {CallBase = true};
            var list = listMock.Object;
            list.InsertRange(0, adversarial);

            Assert.Equal(adversarial, list);
            Assert.Equal(adversarial.Count, list.Count);

            void ClearInvocations()
            {
                foreach (var pair in mockPool)
                {
                    pair.Value.Invocations.Clear();
                }

                listMock.Invocations.Clear();
            }

            for (var i = 0; i < initialNumObjects * 5; i++)
            {
                ClearInvocations();
                var index = random.Next(-2, initialNumObjects + 2);
                EquatableObject obj = null;
                if (random.NextDouble() < 0.8)
                {
                    var mock = new Mock<EquatableObject> {CallBase = true};
                    mockPool.Add(mock.Object, mock);
                    obj = mock.Object;
                }

                Exception error = null;
                try
                {
                    // ReSharper disable once TryCastAlwaysSucceeds
                    var casted = adversarial as IList<EquatableObject>;
                    casted[index] = obj;
                }
                catch (Exception e)
                {
                    error = e;
                }

                try
                {
                    // ReSharper disable once TryCastAlwaysSucceeds
                    var casted = list as IList<EquatableObject>;
                    casted[index] = obj;
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    Assert.NotNull(error);
                    Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                }

                if (error is null)
                {
                    Assert.Equal(obj, list[index]);
                    Assert.Equal(adversarial[index], list[index]);
                }


                Assert.Equal(adversarial, list);
                listMock.VerifyNoOtherCalls();


                if (obj is {} && mockPool.TryGetValue(obj, out var eqMock))
                {
                    eqMock.Verify(mock => mock.Equals(It.IsAny<EquatableObject>()), Times.AtLeast(1));
                    eqMock.Verify(mock => mock.GetHashCode());
                    eqMock.VerifyNoOtherCalls();
                }
            }
        }

        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_Move(int seed)
        {
            var random = new TRandom(seed);
            var mockPool = new Dictionary<EquatableObject, Mock<EquatableObject>>();
            var adversarial = new List<EquatableObject>();

            var initialNumObjects = random.Next(32);
            adversarial.Capacity = initialNumObjects;
            for (var i = 0; i < initialNumObjects; i++)
            {
                if (i == 0 || random.NextDouble() < 0.8)
                {
                    var mock = new Mock<EquatableObject> {CallBase = true};
                    Assert.NotNull(mock.Object);
                    mockPool.Add(mock.Object, mock);
                    adversarial.Add(mock.Object);
                }
                else // insert duplicate
                {
                    var index = random.Next(adversarial.Count);
                    var mock = mockPool[adversarial[index]];
                    Assert.NotNull(mock.Object);
                    adversarial.Add(mock.Object);
                }
            }

            var listMock = new Mock<ArrayList<EquatableObject>> {CallBase = true};
            var list = listMock.Object;
            list.InsertRange(0, adversarial);

            Assert.Equal(adversarial, list);
            Assert.Equal(adversarial.Count, list.Count);

            void ClearInvocations()
            {
                foreach (var pair in mockPool)
                {
                    pair.Value.Invocations.Clear();
                }

                listMock.Invocations.Clear();
            }
            
            static void Move<T>(IList<T> list, int fromIndex, int toIndex)
            {
                var tmp = list[fromIndex];
                list.RemoveAt(fromIndex);
                try
                {
                    list.Insert(toIndex, tmp);
                }
                catch (Exception)
                {
                    list.Insert(fromIndex, tmp);
                    throw;
                }
            }
            
            for (var i = 0; i < initialNumObjects * 5; i++)
            {
                ClearInvocations();
                var fromIndex = random.Next(-2, initialNumObjects + 2);
                var toIndex = random.Next(-2, initialNumObjects + 2);

                Exception error = null;
                try
                {
                    Move(adversarial, fromIndex, toIndex);
                }
                catch (Exception e)
                {
                    error = e;
                }
                
                try
                {
                    list.Move(fromIndex, toIndex);
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    Assert.NotNull(error);
                    Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                }
                

                Assert.Equal(adversarial, list);
                listMock.VerifyNoOtherCalls();
            }
        }
        
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_Swap(int seed)
        {
            var random = new TRandom(seed);
            var mockPool = new Dictionary<EquatableObject, Mock<EquatableObject>>();
            var adversarial = new List<EquatableObject>();

            var initialNumObjects = random.Next(32);
            adversarial.Capacity = initialNumObjects;
            for (var i = 0; i < initialNumObjects; i++)
            {
                if (i == 0 || random.NextDouble() < 0.8)
                {
                    var mock = new Mock<EquatableObject> {CallBase = true};
                    Assert.NotNull(mock.Object);
                    mockPool.Add(mock.Object, mock);
                    adversarial.Add(mock.Object);
                }
                else // insert duplicate
                {
                    var index = random.Next(adversarial.Count);
                    var mock = mockPool[adversarial[index]];
                    Assert.NotNull(mock.Object);
                    adversarial.Add(mock.Object);
                }
            }

            var listMock = new Mock<ArrayList<EquatableObject>> {CallBase = true};
            var list = listMock.Object;
            list.InsertRange(0, adversarial);

            Assert.Equal(adversarial, list);
            Assert.Equal(adversarial.Count, list.Count);

            void ClearInvocations()
            {
                foreach (var pair in mockPool)
                {
                    pair.Value.Invocations.Clear();
                }

                listMock.Invocations.Clear();
            }
            
            static void Swap<T>(IList<T> list, int a, int b)
            {
                (list[a], list[b]) = (list[b], list[a]);
            }
            
            for (var i = 0; i < initialNumObjects * 5; i++)
            {
                ClearInvocations();
                var fromIndex = random.Next(-2, initialNumObjects + 2);
                var toIndex = random.Next(-2, initialNumObjects + 2);

                Exception error = null;
                try
                {
                    Swap(adversarial, fromIndex, toIndex);
                }
                catch (Exception e)
                {
                    error = e;
                }
                
                try
                {
                    list.Swap(fromIndex, toIndex);
                    Assert.Null(error);
                }
                catch (Exception e) when (!(e is XunitException))
                {
                    Assert.NotNull(error);
                    Assert.True(e.GetType().IsInstanceOfType(error) || error.GetType().IsInstanceOfType(e));
                }
                

                Assert.Equal(adversarial, list);
                listMock.VerifyNoOtherCalls();
            }
        }
        

        public class EquatableObject : IEquatable<EquatableObject>
        {
            public virtual bool Equals(EquatableObject other)
            {
                return new ReferenceEqualityComparer<EquatableObject>().Equals(this, other!);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != GetType())
                {
                    return false;
                }

                return Equals((EquatableObject) obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
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