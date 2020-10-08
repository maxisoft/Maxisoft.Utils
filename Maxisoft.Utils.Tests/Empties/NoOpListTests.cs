using System;
using System.Collections;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoFixture;
using AutoFixture.Xunit2;
using Maxisoft.Utils.Empties;
using Xunit;

namespace Maxisoft.Utils.Tests.Empties
{
    public class NoOpListTests
    {
        [Fact]
        public void Test_Enumerator()
        {
            var l = new NoOpList();
            Assert.IsType<EmptyEnumerator>(l.GetEnumerator());
            Assert.Empty(l);
            foreach (var _ in l)
            {
                Assert.False(true);
            }
            Assert.Equal(Enumerable.Empty<object>(), ((IEnumerable)l));
        }
        
        [Theory, AutoData]
        public void TestAdd(string value)
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var l = new NoOpList();
            l.Add(value);
            Assert.Empty(l);
        }
        
        [Theory, AutoData]
        public void TestInsert(int index, string value)
        {
            var l = new NoOpList();
            l.Insert(index, value);
            Assert.Empty(l);
        }
        
        
        [Fact]
        public void TestClear()
        {
            var l = new NoOpList();
            l.Clear();
            Assert.Empty(l);
        }
        
        [Theory, AutoData]
        public void TestContains(string value)
        {
            var l = new NoOpList();
            Assert.False(l.Contains(value));
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
            
            var l = new NoOpList();
            var copy = array.ToImmutableList();
            l.CopyTo(array, fixture.Create<int>());
            Assert.Equal(copy, array);
        }
        
        [Theory, AutoData]
        public void TestRemove(string key)
        {
            var l = new NoOpList();
            l.Remove(key);
            Assert.Empty(l);
        }
        
        [Theory, AutoData]
        public void TestRemoveAt(int index)
        {
            var l = new NoOpList();
            l.RemoveAt(index);
            Assert.Empty(l);
        }
        
        [Theory, AutoData]
        public void TestIndexOf(string obj)
        {
            var l = new NoOpList();
            Assert.Equal(-1, l.IndexOf(obj));
            Assert.Empty(l);
        }
        
        [Theory, AutoData]
        public void TestIndexer(int index, string value)
        {
            var l = new NoOpList();
            Assert.Equal(default, l[index]);
            l[index] = value;
            Assert.Empty(l);
        }
        
        [Fact]
        public void TestProperties()
        {
            var l = new NoOpList();
            Assert.False(l.IsFixedSize);
            Assert.False(l.IsReadOnly);
            // ReSharper disable once xUnit2013
            Assert.Equal(0, l.Count);
            Assert.False(l.IsSynchronized);
            // ReSharper disable once xUnit2005
            Assert.IsType<NoOpList>(l.SyncRoot);
        }
    }
}