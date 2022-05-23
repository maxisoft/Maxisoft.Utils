using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Maxisoft.Utils.Collections.Dictionaries;
using Maxisoft.Utils.Collections.Spans;
using Maxisoft.Utils.Random;
using Troschuetz.Random;
using Xunit;

namespace Maxisoft.Utils.Tests.Collections.Spans
{
    public class SpanDictFuzzingTests
    {
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_Add_Remove_Fuzz(int seed)
        {
            const int maxSize = 58;
            var dict = new SpanDict<int, int>(maxSize);
            var adversarial = new OrderedDictionary<int, int>(maxSize);
            Assert.True(DictionariesEquals(in dict, in adversarial));
            
            var random = new TRandom(seed);

            var nIter = random.Next(1024);
            var fillProb = random.NextDouble();
            var count = 0;

            for (var iter = 0; iter < nIter; iter++)
            {
                if (random.NextDouble() < fillProb)
                {
                    var pair = new KeyValuePair<int, int>(random.Next(maxSize * 2), random.Next());
                    if (count >= maxSize)
                    {
                        try
                        {
                            dict.Add(pair);
                            Assert.False(true);
                        }
                        catch (Exception e)
                        {
                            Assert.NotNull(e);
                        }
                    }
                    else
                    {
                        Exception error = null;
                        try
                        {
                            adversarial.Add(pair.Key, pair.Value);
                        }
                        catch (ArgumentException e)
                        {
                            error = e;
                        }

                        try
                        {
                            dict.Add(pair);
                            count += 1;
                            Assert.Null(error);
                        }
                        catch (ArgumentException)
                        {
                            Assert.NotNull(error);
                        }
                    }
                }
                else
                {
                    var key = random.Next(maxSize * 2);
                    var expected = adversarial.Remove(key:key);
                    var actual = dict.Remove(key);
                    Assert.Equal(expected, actual);
                    if (expected)
                    {
                        count -= 1;
                    }
                }

                Assert.Equal(count, adversarial.Count);
                Assert.Equal(count, dict.Count);
                Assert.True(DictionariesEquals(in dict, in adversarial));
            }
        }

        [Fact]
        public void Test_Regression()
        {
            Test_Add_Remove_Fuzz(seed: 1488662331);
        }

        private static bool DictionariesEquals<TKey, TValue, TDictionary>(in SpanDict<TKey, TValue> left, in TDictionary right)
            where TDictionary : IDictionary<TKey, TValue>
        {
            if (left.Count != right.Count) return false;
            var valueComparer = EqualityComparer<TValue>.Default;
            var c = 0;
            foreach (var (key, value) in left)
            {
                if (!right.TryGetValue(key, out var rightValue))
                {
                    return false;
                }

                if (!valueComparer.Equals(value, rightValue))
                {
                    return false;
                }

                c += 1;
            }
            Debug.Assert(c == left.Count);
            return c == right.Count;
        }
        
        internal class RandomSeedGenerator : IEnumerable<object[]>
        {
            internal virtual RandomThreadSafe Random { get; } = new RandomThreadSafe();
            internal virtual int NumberOfGen => 128;

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