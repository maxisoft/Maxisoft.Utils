using System;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using AutoFixture;
using AutoFixture.Xunit2;
using Maxisoft.Utils.Empties;
using Xunit;

namespace Maxisoft.Utils.Tests.Empties
{
    public class NoOpDictionaryTests
    {
        [Theory, AutoData]
        public void TestAdd(string key, string value)
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var dict = new NoOpDictionary();
            dict.Add(key, value);
            Assert.Empty(dict);
        }
        
        [Fact]
        public void TestEnumerate()
        {
            var dict = new NoOpDictionary();
            Assert.Empty(dict);

            foreach (var _ in dict)
            {
                Assert.False(true);
            }
        }
        
        [Fact]
        public void TestClear()
        {
            var dict = new NoOpDictionary();
            dict.Clear();
            Assert.Empty(dict);
        }
        
        [Theory, AutoData]
        public void TestContains(string key)
        {
            var dict = new NoOpDictionary();
            Assert.False(dict.Contains(key));
        }
        
        [Theory, AutoData]
        public void TestCopyTo([Range(0, 16)]int arrayLength)
        {
            var fixture = new Fixture();
            var array = new object[arrayLength];
            for (var i = 0; i < arrayLength; i++)
            {
                array[i] = fixture.Create<object>();
            }
            var copy = array.ToImmutableList();
            var dict = new NoOpDictionary();
            dict.CopyTo(array, fixture.Create<int>());
            Assert.Equal(copy, array);
        }
        
        [Theory, AutoData]
        public void TestRemove(string key)
        {
            var dict = new NoOpDictionary();
            dict.Remove(key);
            Assert.Empty(dict);
        }
        
        [Theory, AutoData]
        public void TestIndexer(string key, string value)
        {
            var dict = new NoOpDictionary();
            Assert.Equal(default, dict[key]);
            dict[key] = value;
            Assert.Empty(dict);
        }
        
        [Fact]
        public void TestProperties()
        {
            var dict = new NoOpDictionary();
            Assert.False(dict.IsFixedSize);
            Assert.False(dict.IsReadOnly);
            Assert.IsType<EmptyCollection>(dict.Keys);
            Assert.IsType<EmptyCollection>(dict.Values);
            // ReSharper disable once xUnit2013
            Assert.Equal(0, dict.Count);
            Assert.False(dict.IsSynchronized);
            // ReSharper disable once xUnit2005
            Assert.IsType<NoOpDictionary>(dict.SyncRoot);
        }
    }
}