using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using AutoFixture;
using AutoFixture.Xunit2;
using Maxisoft.Utils.Empties;
using Xunit;

namespace Maxisoft.Utils.Tests.Empties
{
    public class EmptyDictionaryGenericTests
    {
        [Theory, AutoData]
        public void TestAdd(string key, string value)
        {
            var dict = new EmptyDictionary<string, string>();
            Assert.Throws<InvalidOperationException>(() => dict.Add(key, value));
            Assert.Throws<InvalidOperationException>(() => dict.Add(new KeyValuePair<string, string>(key, value)));
        }
        
        [Fact]
        public void TestEnumerate()
        {
            var dict = new EmptyDictionary<string, string>();
            Assert.Empty(dict);

            foreach (var _ in dict)
            {
                Assert.False(true);
            }
        }
        
        [Fact]
        public void TestClear()
        {
            var dict = new EmptyDictionary<string, string>();
            dict.Clear();
        }
        
        [Theory, AutoData]
        public void TestContains(string key, string value)
        {
            var dict = new EmptyDictionary<string, string>();
            Assert.DoesNotContain(new KeyValuePair<string, string>(key, value), dict);
        }
        
        [Theory, AutoData]
        public void TestContainsKey(string key)
        {
            var dict = new EmptyDictionary<string, string>();
            Assert.False(dict.ContainsKey(key));
        }
        
        [Theory, AutoData]
        public void TestTryGetValue(string key)
        {
            var dict = new EmptyDictionary<string, string>();
            Assert.Throws<InvalidOperationException>(() => dict.TryGetValue(key, out var _));
        }
        
        [Theory, AutoData]
        public void TestCopyTo([Range(0, 16)]int arrayLength)
        {
            var fixture = new Fixture();
            var array = new KeyValuePair<string, string>[arrayLength];
            for (var i = 0; i < arrayLength; i++)
            {
                array[i] = fixture.Create<KeyValuePair<string, string>>();
            }

            var copy = array.ToImmutableList();
            var dict = new EmptyDictionary<string, string>();
            dict.CopyTo(array, fixture.Create<int>());
            Assert.Equal(copy, array);
        }
        
        [Theory, AutoData]
        public void TestRemove(string key, string value)
        {
            var dict = new EmptyDictionary<string, string>();
            Assert.False(dict.Remove(key));
            Assert.False(dict.Remove(new KeyValuePair<string, string>(key, value)));
            Assert.Empty(dict);
        }
        
        [Theory, AutoData]
        public void TestIndexer(string key, string value)
        {
            var dict = new EmptyDictionary<string, string>();
            Assert.Throws<InvalidOperationException>(() => dict[key]);
            Assert.Throws<InvalidOperationException>(() => dict[key] = value);
        }
        
        [Fact]
        public void TestProperties()
        {
            var dict = new EmptyDictionary<string, string>();
            Assert.False(dict.IsReadOnly);
            Assert.IsType<EmptyCollection<string>>(dict.Keys);
            Assert.IsType<EmptyCollection<string>>(dict.Values);
            // ReSharper disable once xUnit2013
            Assert.Equal(0, dict.Count);
        }
    }
}