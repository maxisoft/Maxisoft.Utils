using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Maxisoft.Utils.Collections;
using Maxisoft.Utils.Collections.LinkedLists;
using Maxisoft.Utils.Collections.Lists;
using Maxisoft.Utils.Random;
using Troschuetz.Random;
using Xunit;

namespace Maxisoft.Utils.Tests.Collections.LinkedLists
{
    public class LinkedListExtensionsFuzzingTests
    {
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void TestIndexOf_With_Duplicate(int seed)
        {
            const int maxSize = 128;
            var random = new TRandom(seed);
            var size = random.Next(maxSize);

            var l = new LinkedListAsIList<int>();
            var adversarial = new ArrayList<int>(size);
            for (int i = 0; i < size; i++)
            {
                var n = random.Next();
                l.AddLast(n);
                adversarial.Add(n);
            }
            
            Assert.Equal(adversarial, l);
            
            for (var i = -2; i < maxSize + 2; i++)
            {
                Assert.Equal(adversarial.IndexOf(i), l.IndexOf(i));
                Assert.Equal(adversarial.LastIndexOf(i), l.LastIndexOf(i));
            }
        }
        
        
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void TestIndexOfFromBoth_With_Duplicate(int seed)
        {
            const int maxSize = 128;
            var random = new TRandom(seed);
            var size = random.Next(maxSize);
            
            var adversarial = new ArrayList<int>(size);
            for (var i = 0; i < size; i++)
            {
                adversarial.AddSorted(random.Next());
            }

            var l = adversarial.ToLinkedList();
            
            Assert.Equal(adversarial, l);
            
            for (var i = -2; i < maxSize + 2; i++)
            {
                Assert.Equal(adversarial.IndexOf(i), l.IndexOf(i));
                Assert.Equal(adversarial.LastIndexOf(i), l.LastIndexOf(i));
                var bs = adversarial.BinarySearch(i);
                bs = Math.Max(bs, -1);
                Assert.Equal(bs, l.IndexOfFromBoth(i));
            }
        }

        [Fact]
        public void Test_Regressions()
        {
            TestIndexOfFromBoth_With_Duplicate(seed: 1208303184);
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