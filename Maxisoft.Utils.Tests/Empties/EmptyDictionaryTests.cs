using System;
using System.ComponentModel.DataAnnotations;
using AutoFixture;
using AutoFixture.Xunit2;
using Maxisoft.Utils.Empties;
using Xunit;

namespace Maxisoft.Utils.Tests.Empties
{
    public class EmptyDictionaryTests
    {
        [Theory, AutoData]
        public void TestAdd(string key, string value)
        {
            var dict = new EmptyDictionary();
            Assert.Throws<InvalidOperationException>(() => dict.Add(key, value));
            Assert.Empty(dict);
        }
        
        [Fact]
        public void TestEnumerate()
        {
            var dict = new EmptyDictionary();
            Assert.Empty(dict);

            foreach (var _ in dict)
            {
                Assert.False(true);
            }
        }
        
        [Fact]
        public void TestClear()
        {
            var dict = new EmptyDictionary();
            dict.Clear();
        }
        
        [Theory, AutoData]
        public void TestContains(string key)
        {
            var dict = new EmptyDictionary();
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
            
            var dict = new EmptyDictionary();
            dict.CopyTo(array, fixture.Create<int>());
        }
        
        [Theory, AutoData]
        public void TestRemove(string key)
        {
            var dict = new EmptyDictionary();
            dict.Remove(key);
            Assert.Empty(dict);
        }
        
        [Theory, AutoData]
        public void TestIndexer(string key, string value)
        {
            var dict = new EmptyDictionary();
            Assert.Throws<InvalidOperationException>(() => dict[key]);
            Assert.Throws<InvalidOperationException>(() => dict[key] = value);
        }
        
        [Fact]
        public void TestProperties()
        {
            var dict = new EmptyDictionary();
            Assert.False(dict.IsFixedSize);
            Assert.False(dict.IsReadOnly);
            Assert.IsType<EmptyCollection>(dict.Keys);
            Assert.IsType<EmptyCollection>(dict.Values);
            // ReSharper disable once xUnit2013
            Assert.Equal(0, dict.Count);
            Assert.False(dict.IsSynchronized);
            // ReSharper disable once xUnit2005
            Assert.IsType<EmptyDictionary>(dict.SyncRoot);
        }
    }
}