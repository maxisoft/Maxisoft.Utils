using System;
using System.Collections;
using System.Collections.Generic;
using Maxisoft.Utils.Collections.Spans;
using Maxisoft.Utils.Random;
using Troschuetz.Random;
using Xunit;

namespace Maxisoft.Utils.Tests.Collections.Spans
{
    public class BitSpanTests
    {
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_Indexer(int seed)
        {
            const int maxLongLength = 32;
            var random = new TRandom(seed);
            var longLength = (random.Next(maxLongLength)) & (~1);

            var dataArr = new int[longLength];
            for (var i = 0; i < longLength; i++)
            {
                dataArr[i] = random.Next() ^ (random.Next() << 1);
            }

            var adversarial = new BitArray(dataArr);
            var bs = BitSpan.CreateFromBuffer((Span<int>) dataArr);

            Assert.Equal(adversarial.Count, bs.Count);
            Assert.Equal(adversarial.Length, bs.Length);
            AssertEqual(adversarial, in bs);

// test getter
            {
                for (var i = 0; i < bs.Length; i++)
                {
                    Assert.Equal(adversarial[i], bs[i]);
                }

                try
                {
                    var _ = bs[-1];
                    Assert.False(true);
                }
                catch (Exception e)
                {
                    Assert.Throws(e.GetType(), () => adversarial[-1]);
                }

                try
                {
                    var _ = bs[bs.Length];
                    Assert.False(true);
                }
                catch (Exception e)
                {
                    var len = bs.Length;
                    Assert.Throws(e.GetType(), () => adversarial[len]);
                }
            }

            // test setter and getter

            {
                for (var i = 0; i < bs.Length; i++)
                {
                    var index = random.Next(bs.Length);
                    var value = random.NextBoolean();
                    adversarial[index] = value;
                    bs[index] = value;
                    Assert.Equal(adversarial[i], bs[i]);
                    AssertEqual(adversarial, bs);
                }

                try
                {
                    bs[-1] = random.NextBoolean();
                    Assert.False(true);
                }
                catch (Exception e)
                {
                    Assert.Throws(e.GetType(), () => adversarial[-1] = random.NextBoolean());
                }

                try
                {
                    bs[bs.Length] = random.NextBoolean();
                    Assert.False(true);
                }
                catch (Exception e)
                {
                    var len = bs.Length;
                    Assert.Throws(e.GetType(), () => adversarial[len] = random.NextBoolean());
                }
            }
        }


        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_SetAll(int seed)
        {
            const int maxLongLength = 32;
            var random = new TRandom(seed);
            var longLength = (random.Next(maxLongLength)) & (~1);

            var dataArr = new int[longLength];
            for (var i = 0; i < longLength; i++)
            {
                dataArr[i] = random.Next() ^ (random.Next() << 1);
            }

            var adversarial = new BitArray(dataArr);
            var bs = BitSpan.CreateFromBuffer((Span<int>) dataArr);

            var value = random.NextBoolean();
            bs.SetAll(value);
            adversarial.SetAll(value);
            
            AssertEqual(adversarial, bs);
            
            value = !value;
            bs.SetAll(value);
            adversarial.SetAll(value);
            AssertEqual(adversarial, bs);
        }
        
        
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_And(int seed)
        {
            const int maxLongLength = 32;
            var random = new TRandom(seed);
            var longLength = (random.Next(maxLongLength)) & (~1);

            var dataArr = new int[longLength];
            for (var i = 0; i < longLength; i++)
            {
                dataArr[i] = random.Next() ^ (random.Next() << 1);
            }

            var adversarial = new BitArray(dataArr);
            var bs = BitSpan.CreateFromBuffer((Span<int>) dataArr);

            var value = new BitArray(dataArr);
            value.SetAll(false);
            for (var i = 0; i < value.Count; i++)
            {
                value[i] = random.NextBoolean();
            }

            AssertEqual(adversarial, bs);

            bs.And(value);
            adversarial.And(value);
            AssertEqual(adversarial, bs);
        }
        
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_Or(int seed)
        {
            const int maxLongLength = 32;
            var random = new TRandom(seed);
            var longLength = (random.Next(maxLongLength)) & (~1);

            var dataArr = new int[longLength];
            for (var i = 0; i < longLength; i++)
            {
                dataArr[i] = random.Next() ^ (random.Next() << 1);
            }

            var adversarial = new BitArray(dataArr);
            var bs = BitSpan.CreateFromBuffer((Span<int>) dataArr);

            var value = new BitArray(dataArr);
            value.SetAll(false);
            for (var i = 0; i < value.Count; i++)
            {
                value[i] = random.NextBoolean();
            }

            AssertEqual(adversarial, bs);

            bs.Or(value);
            adversarial.Or(value);
            AssertEqual(adversarial, bs);
        }
        
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_Xor(int seed)
        {
            const int maxLongLength = 32;
            var random = new TRandom(seed);
            var longLength = (random.Next(maxLongLength)) & (~1);

            var dataArr = new int[longLength];
            for (var i = 0; i < longLength; i++)
            {
                dataArr[i] = random.Next() ^ (random.Next() << 1);
            }

            var adversarial = new BitArray(dataArr);
            var bs = BitSpan.CreateFromBuffer((Span<int>) dataArr);

            var value = new BitArray(dataArr);
            value.SetAll(false);
            for (var i = 0; i < value.Count; i++)
            {
                value[i] = random.NextBoolean();
            }

            AssertEqual(adversarial, bs);

            bs.Xor(value);
            adversarial.Xor(value);
            AssertEqual(adversarial, bs);
        }
        
        
        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_Not(int seed)
        {
            const int maxLongLength = 32;
            var random = new TRandom(seed);
            var longLength = (random.Next(maxLongLength)) & (~1);

            var dataArr = new int[longLength];
            for (var i = 0; i < longLength; i++)
            {
                dataArr[i] = random.Next() ^ (random.Next() << 1);
            }

            var adversarial = new BitArray(dataArr);
            var bs = BitSpan.CreateFromBuffer((Span<int>) dataArr);

            AssertEqual(adversarial, bs);

            bs.Not();
            adversarial.Not();
            AssertEqual(adversarial, bs);
        }

        internal static void AssertEqual(BitArray bitArray, in BitSpan bitSpan)
        {
            var c = 0;
            foreach (var bit in bitSpan)
            {
                Assert.Equal(bitArray[c++], bit);
            }
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