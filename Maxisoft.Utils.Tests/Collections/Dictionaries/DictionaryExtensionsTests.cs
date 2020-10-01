using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Maxisoft.Utils.Collections;
using Maxisoft.Utils.Collections.Dictionaries;
using Maxisoft.Utils.Empties;
using Maxisoft.Utils.Random;
using Moq;
using Troschuetz.Random;
using Xunit;

namespace Maxisoft.Utils.Tests.Collections.Dictionaries
{
    public class DictionaryExtensionsTests
    {
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_GetKeyValuePairEnumerator(int seed)
        {
            const int maxElements = 16;
            var @base = new OrderedDictionary();
            var numberGenerator = new TRandom(seed);
            var dictMock = new Mock<VirtualOrderedDictionary>(@base) {CallBase = true};
            var numElements = numberGenerator.Next(maxElements);

            for (var i = 0; i < numElements; i++)
            {
                @base.Add($"{i:X}", i);
            }

            var res = dictMock.Object.GetKeyValuePairEnumerator<string, int>();
            using var enumerator = res.GetEnumerator();
            foreach (dynamic pair in @base)
            {
                Assert.True(enumerator.MoveNext());
                Assert.Equal(pair.Key, enumerator.Current.Key);
                Assert.Equal(pair.Value, enumerator.Current.Value);
            }

            dictMock.VerifyNoOtherCalls();
        }

        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_ToOrderedDictionary(int seed)
        {
            const int maxElements = 16;
            var @base = new OrderedDictionary();
            var numberGenerator = new TRandom(seed);
            var dictMock = new Mock<VirtualOrderedDictionary>(@base) {CallBase = true};
            var numElements = numberGenerator.Next(maxElements);

            for (var i = 0; i < numElements; i++)
            {
                @base.Add($"{i:X}", i);
            }

            var res = dictMock.Object.ToOrderedDictionary<string, int>();
            using var enumerator = res.GetEnumerator();
            foreach (dynamic pair in @base)
            {
                Assert.True(enumerator.MoveNext());
                Assert.Equal(pair.Key, enumerator.Current.Key);
                Assert.Equal(pair.Value, enumerator.Current.Value);
            }

            dictMock.Verify(mock => mock.Count);
            dictMock.VerifyNoOtherCalls();
        }

        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_ToOrderedDictionary_WithComparer(int seed)
        {
            const int maxElements = 16;
            var @base = new OrderedDictionary();
            var numberGenerator = new TRandom(seed);
            var dictMock = new Mock<VirtualOrderedDictionary>(@base) {CallBase = true};
            var numElements = numberGenerator.Next(maxElements);

            for (var i = 0; i < numElements; i++)
            {
                @base.Add($"{i:X}", i);
            }

            var res = dictMock.Object.ToOrderedDictionary<string, int>(EqualityComparer<string>.Default);
            using var enumerator = res.GetEnumerator();
            foreach (dynamic pair in @base)
            {
                Assert.True(enumerator.MoveNext());
                Assert.Equal(pair.Key, enumerator.Current.Key);
                Assert.Equal(pair.Value, enumerator.Current.Value);
            }

            dictMock.Verify(mock => mock.Count);
            dictMock.VerifyNoOtherCalls();
        }

        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_ToOrderedDictionary_Collection(int seed)
        {
            const int maxElements = 16;
            var @base = new OrderedDictionary();
            var numberGenerator = new TRandom(seed);
            var dictMock = new Mock<VirtualOrderedDictionary>(@base) {CallBase = true};
            var numElements = numberGenerator.Next(maxElements);

            for (var i = 0; i < numElements; i++)
            {
                @base.Add($"{i:X}", i);
            }

            OrderedDictionary<string, int> res;
            if (numberGenerator.NextBoolean())
            {
                res = dictMock.Object.GetKeyValuePairEnumerator<string, int>().ToList()
                    .ToOrderedDictionary<string, int>(EqualityComparer<string>.Default);
            }
            else
            {
                res = dictMock.Object.GetKeyValuePairEnumerator<string, int>().ToList()
                    .ToOrderedDictionary<string, int>();
            }

            using var enumerator = res.GetEnumerator();
            foreach (dynamic pair in @base)
            {
                Assert.True(enumerator.MoveNext());
                Assert.Equal(pair.Key, enumerator.Current.Key);
                Assert.Equal(pair.Value, enumerator.Current.Value);
            }

            dictMock.VerifyNoOtherCalls();
        }

        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_ToOrderedDictionary_IEnumerable(int seed)
        {
            const int maxElements = 16;
            var @base = new OrderedDictionary();
            var numberGenerator = new TRandom(seed);
            var dictMock = new Mock<VirtualOrderedDictionary>(@base) {CallBase = true};
            var numElements = numberGenerator.Next(maxElements);

            for (var i = 0; i < numElements; i++)
            {
                @base.Add($"{i:X}", i);
            }

            OrderedDictionary<string, int> res;
            if (numberGenerator.NextBoolean())
            {
                res = dictMock.Object.GetKeyValuePairEnumerator<string, int>()
                    .ToOrderedDictionary<string, int>(EqualityComparer<string>.Default);
            }
            else
            {
                res = dictMock.Object.GetKeyValuePairEnumerator<string, int>().ToOrderedDictionary<string, int>();
            }

            using var enumerator = res.GetEnumerator();
            foreach (dynamic pair in @base)
            {
                Assert.True(enumerator.MoveNext());
                Assert.Equal(pair.Key, enumerator.Current.Key);
                Assert.Equal(pair.Value, enumerator.Current.Value);
            }

            dictMock.VerifyNoOtherCalls();
        }

        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_GetOrAdd(int seed)
        {
            const int maxElements = 16;
            var @base = new OrderedDictionary<string, int>();
            var random = new TRandom(seed);
            var dictMock = new Mock<VirtualOrderedDictionaryStringInt>(@base) {CallBase = true};
            var numElements = random.Next(maxElements);

            for (var i = 0; i < numElements; i++)
            {
                @base.Add($"{i:X}", i);
            }

            var valueFactoryMock = new Mock<VirtualOrderedDictionaryStringInt>(new OrderedDictionary<string, int>()){CallBase = true};

            int ValueFactory()
            {
                _ = valueFactoryMock.Object.Count;
                return @base.Count;
            }

            // get a non existing value
            {
                var expected = ValueFactory();
                valueFactoryMock.Invocations.Clear();

                var res = dictMock.Object.GetOrAdd("non existing", expected);
                Assert.Equal(expected, res);
                Assert.Equal(expected, @base[^1]);
                dictMock.Verify(mock => mock.Add("non existing", expected));
                dictMock.Verify(mock => mock.TryGetValue("non existing", out It.Ref<int>.IsAny));
                dictMock.VerifyGet(mock => mock["non existing"]);
                dictMock.VerifyNoOtherCalls();
                dictMock.Invocations.Clear();
            }
            
            
            // get a non existing value with factory
            {
                
                var expected = ValueFactory();
                valueFactoryMock.Invocations.Clear();
                
                var res = dictMock.Object.GetOrAdd("non existing value factory", ValueFactory);
                Assert.Equal(expected, res);
                Assert.Equal(expected, @base[^1]);
                dictMock.Verify(mock => mock.Add("non existing value factory", expected));
                dictMock.Verify(mock => mock.TryGetValue("non existing value factory", out It.Ref<int>.IsAny));
                dictMock.VerifyGet(mock => mock["non existing value factory"]);
                dictMock.VerifyNoOtherCalls();
                dictMock.Invocations.Clear();
                
                valueFactoryMock.Verify(mock => mock.Count, Times.Once);
                valueFactoryMock.VerifyNoOtherCalls();
            }
            
            // get a existing value
            {
                var key = random.Choice(@base.Keys.ToArrayList());
                var expected = @base[key];
                var res = dictMock.Object.GetOrAdd(key, @base.Count);
                Assert.Equal(expected, res);
                dictMock.Verify(mock => mock.TryGetValue(key, out It.Ref<int>.IsAny));
                dictMock.VerifyNoOtherCalls();
                dictMock.Invocations.Clear();
            }
            
            // get a existing value with valueFactory
            {
                var key = random.Choice(@base.Keys.ToArrayList());
                var expected = @base[key];
                var res = dictMock.Object.GetOrAdd(key, ValueFactory);
                Assert.Equal(expected, res);
                dictMock.Verify(mock => mock.TryGetValue(key, out It.Ref<int>.IsAny));
                dictMock.VerifyNoOtherCalls();
                dictMock.Invocations.Clear();
                valueFactoryMock.VerifyNoOtherCalls();
            }
        }
        
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public async void Test_GetOrAddAsync(int seed)
        {
            const int maxElements = 16;
            var @base = new OrderedDictionary<string, int>();
            var random = new TRandom(seed);
            var dictMock = new Mock<VirtualOrderedDictionaryStringInt>(@base) {CallBase = true};
            var numElements = random.Next(maxElements);

            for (var i = 0; i < numElements; i++)
            {
                @base.Add($"{i:X}", i);
            }

            
            Task<int> ValueFactory()
            {
                return new ValueTask<int>(@base.Count).AsTask();
            }
            
            
            
            // get a non existing value with factory
            {
                
                var expected = await ValueFactory();

                var res = await dictMock.Object.GetOrAddAsync("non existing value factory", ValueFactory());
                Assert.Equal(expected, res);
                Assert.Equal(expected, @base[^1]);
                dictMock.Verify(mock => mock.Add("non existing value factory", expected));
                dictMock.Verify(mock => mock.TryGetValue("non existing value factory", out It.Ref<int>.IsAny));
                dictMock.VerifyGet(mock => mock["non existing value factory"]);
                dictMock.VerifyNoOtherCalls();
                dictMock.Invocations.Clear();
                
            }

            // get a existing value with valueFactory
            {
                var key = random.Choice(@base.Keys.ToArrayList());
                var expected = @base[key];
                var res = await dictMock.Object.GetOrAddAsync(key, ValueFactory());
                Assert.Equal(expected, res);
                dictMock.Verify(mock => mock.TryGetValue(key, out It.Ref<int>.IsAny));
                dictMock.VerifyNoOtherCalls();
                dictMock.Invocations.Clear();
            }
        }

        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_Chain(int seed)
        {
            const int maxElements = 16;
            var @base = new OrderedDictionary<string, int>();
            var random = new TRandom(seed);
            var dictMock = new Mock<VirtualOrderedDictionaryStringInt>(@base) {CallBase = true};
            var numElements = random.Next(maxElements);

            for (var i = 0; i < numElements; i++)
            {
                @base.Add($"{i:X}", i);
            }

            var additionals = new[]
            {
                new OrderedDictionary<string, int>()
                {
                    {"a", random.Next()},
                    {"b", random.Next()}
                },
                new OrderedDictionary<string, int>()
                {
                    {"a", random.Next()},
                    {"b", random.Next()},
                    {"c", -1}
                },
            };

            var res = dictMock.Object.Chain(additionals);
            var expected = new OrderedDictionary<string, int>();
            foreach (var pair in @base)
            {
                expected.Add(pair);
            }

            expected.Add("a", additionals[0]["a"]);
            expected.Add("b", additionals[0]["b"]);
            expected.Add("c", additionals[1]["c"]);
            Assert.Equal(expected, res);
            
            var anotherOne = new OrderedDictionary<string, int>()
            {
                {"c", 5},
                {"d", 8}
            };

            var res2 = res.Chain(anotherOne);
            expected.Add("d", anotherOne["d"]);
            Assert.Equal(expected, res2);

        }

        #region Test classes

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

        public class VirtualOrderedDictionary : IOrderedDictionary
        {
            internal IOrderedDictionary _dict;

            public VirtualOrderedDictionary(IOrderedDictionary dict)
            {
                _dict = dict;
            }

            public virtual void Add(object key, object? value)
            {
                _dict.Add(key, value);
            }

            public virtual void Clear()
            {
                _dict.Clear();
            }

            public virtual bool Contains(object key)
            {
                return _dict.Contains(key);
            }

            IDictionaryEnumerator IOrderedDictionary.GetEnumerator()
            {
                return _dict.GetEnumerator();
            }

            public virtual void Insert(int index, object key, object value)
            {
                _dict.Insert(index, key, value);
            }

            public virtual void RemoveAt(int index)
            {
                _dict.RemoveAt(index);
            }

            public virtual object this[int index]
            {
                get => _dict[index];
                set => _dict[index] = value;
            }

            IDictionaryEnumerator IDictionary.GetEnumerator()
            {
                return ((IDictionary) _dict).GetEnumerator();
            }

            public virtual void Remove(object key)
            {
                _dict.Remove(key);
            }

            public virtual bool IsFixedSize => _dict.IsFixedSize;

            public virtual bool IsReadOnly => _dict.IsReadOnly;

            public virtual object? this[object key]
            {
                get => _dict[key];
                set => _dict[key] = value;
            }

            public virtual ICollection Keys => _dict.Keys;

            public virtual ICollection Values => _dict.Values;

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable) _dict).GetEnumerator();
            }

            public virtual void CopyTo(Array array, int index)
            {
                _dict.CopyTo(array, index);
            }

            public virtual int Count => _dict.Count;

            public virtual bool IsSynchronized => _dict.IsSynchronized;

            public virtual object SyncRoot => _dict.SyncRoot;
        }

        public class VirtualOrderedDictionaryStringInt : IOrderedDictionary<string, int>
        {
            internal IOrderedDictionary<string, int> Dict;

            public VirtualOrderedDictionaryStringInt()
            {
            }

            public VirtualOrderedDictionaryStringInt(IOrderedDictionary<string, int> dict)
            {
                Dict = dict;
            }

            public virtual IEnumerator<KeyValuePair<string, int>> GetEnumerator()
            {
                return Dict.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable) Dict).GetEnumerator();
            }

            public virtual void Add(KeyValuePair<string, int> item)
            {
                Dict.Add(item);
            }

            public virtual void Clear()
            {
                Dict.Clear();
            }

            public virtual bool Contains(KeyValuePair<string, int> item)
            {
                return Dict.Contains(item);
            }

            public virtual void CopyTo(KeyValuePair<string, int>[] array, int arrayIndex)
            {
                Dict.CopyTo(array, arrayIndex);
            }

            public virtual bool Remove(KeyValuePair<string, int> item)
            {
                return Dict.Remove(item);
            }

            public virtual int Count => Dict.Count;

            public virtual bool IsReadOnly => Dict.IsReadOnly;

            public virtual void Add(string key, int value)
            {
                Dict.Add(key, value);
            }

            public virtual bool ContainsKey(string key)
            {
                return Dict.ContainsKey(key);
            }

            public virtual bool Remove(string key)
            {
                return Dict.Remove(key);
            }

            public virtual bool TryGetValue(string key, out int value)
            {
                return Dict.TryGetValue(key, out value);
            }

            public virtual int this[string key]
            {
                get => Dict[key];
                set => Dict[key] = value;
            }

            public virtual ICollection<string> Keys => Dict.Keys;

            public virtual ICollection<int> Values => Dict.Values;

            public virtual int this[int index]
            {
                get => Dict[index];
                set => Dict[index] = value;
            }

            public virtual void Insert(int index, in string key, in int value)
            {
                Dict.Insert(index, in key, in value);
            }

            public virtual void RemoveAt(int index)
            {
                Dict.RemoveAt(index);
            }

            public virtual int IndexOf(in string key)
            {
                return Dict.IndexOf(in key);
            }

            public virtual int IndexOf(in int value)
            {
                return Dict.IndexOf(in value);
            }
        }

        #endregion
    }
}