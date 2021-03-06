﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoFixture;
using AutoFixture.Xunit2;
using Maxisoft.Utils.Empties;
using Xunit;

namespace Maxisoft.Utils.Tests.Empties
{
    public class EmptyListGenericTests
    {
        [Fact]
        public void Test_Enumerator()
        {
            var l = new EmptyList<string>();
            Assert.IsType<EmptyEnumerator<string>>(l.GetEnumerator());
            Assert.Empty(l);
            foreach (var _ in l)
            {
                Assert.False(true);
            }
            Assert.Equal(Enumerable.Empty<string>(), ((IEnumerable<string>)l));
        }
        
        [Theory, AutoData]
        public void TestAdd(string value)
        {
            var l = new EmptyList<string>();
            Assert.Throws<InvalidOperationException>(() => l.Add(value));
            Assert.Empty(l);
        }
        
        [Theory, AutoData]
        public void TestInsert(int index, string value)
        {
            var l = new EmptyList<string>();
            Assert.Throws<InvalidOperationException>(() => l.Insert(index, value));
            Assert.Empty(l);
        }
        
        
        [Fact]
        public void TestClear()
        {
            var l = new EmptyList<string>();
            l.Clear();
            Assert.Empty(l);
        }
        
        [Theory, AutoData]
        public void TestContains(string value)
        {
            var l = new EmptyList<string>();
            Assert.False(l.Contains(value));
        }
        
        [Theory, AutoData]
        public void TestCopyTo([Range(0, 16)]int arrayLength)
        {
            var fixture = new Fixture();
            var array = new string[arrayLength];
            for (var i = 0; i < arrayLength; i++)
            {
                array[i] = fixture.Create<string>();
            }
            
            var l = new EmptyList<string>();
            var copy = array.ToImmutableList();
            l.CopyTo(array, fixture.Create<int>());
            Assert.Equal(copy, array);
        }
        
        [Theory, AutoData]
        public void TestRemove(string key)
        {
            var l = new EmptyList<string>();
            l.Remove(key);
            Assert.Empty(l);
        }
        
        [Theory, AutoData]
        public void TestRemoveAt(int index)
        {
            var l = new EmptyList<string>();
            l.RemoveAt(index);
            Assert.Empty(l);
        }
        
        [Theory, AutoData]
        public void TestIndexOf(string obj)
        {
            var l = new EmptyList<string>();
            Assert.Equal(-1, l.IndexOf(obj));
            Assert.Empty(l);
        }
        
        [Theory, AutoData]
        public void TestIndexer(int index, string value)
        {
            var l = new EmptyList<string>();
            Assert.Throws<InvalidOperationException>(() => l[index]);
            Assert.Throws<InvalidOperationException>(() => l[index] = value);
        }
        
        [Fact]
        public void TestProperties()
        {
            var l = new EmptyList<string>();
            Assert.False(l.IsReadOnly);
            // ReSharper disable once xUnit2013
            Assert.Equal(0, l.Count);
            // ReSharper disable once xUnit2005
        }
    }
}