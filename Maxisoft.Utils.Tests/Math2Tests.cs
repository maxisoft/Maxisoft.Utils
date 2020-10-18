using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Maxisoft.Utils.Random;
using Troschuetz.Random;
using Xunit;

namespace Maxisoft.Utils.Tests
{
    public class Math2Tests
    {
        private static ulong NumberOfTrailingZerosSlow(ulong i)
        {
            checked
            {
                if (i == 0)
                {
                    return 64;
                }

                if (i < 0)
                {
                    return 0;
                }

                ulong res = 0;
                while ((i & 1) == 0)
                {
                    if (++res >= 64)
                    {
                        return 64;
                    }

                    i >>= 1;
                }

                return res;
            }
        }

        [Theory]
        [InlineData(0b010, 1)]
        [InlineData(0b011, 0)]
        [InlineData(0b001, 0)]
        [InlineData(0b1001000, 3)]
        [InlineData(0b11100100000000L, 8)]
        public void TestNumberOfTrailingZeros(ulong x, ulong expected)
        {
            Assert.Equal(expected, Math2.NumberOfTrailingZeros(x));
            Assert.Equal(NumberOfTrailingZerosSlow(x), Math2.NumberOfTrailingZeros(x));
            Assert.Equal(BitOperations.TrailingZeroCount(x), (int) Math2.NumberOfTrailingZeros(x));
        }

        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void TestNumberOfTrailingZerosRandomFuzz(int seed)
        {
            var random = new TRandom(seed);
            const int numIter = 1024;
            for (var i = 0; i < numIter; i++)
            {
                var x = ((ulong) random.Next() << 48) ^ ((ulong) random.Next() << 16) ^ (ulong) random.Next();

                // check for ulong
                {
                    var expected = BitOperations.TrailingZeroCount(x);
                    checked
                    {
                        Assert.Equal((ulong) expected, Math2.NumberOfTrailingZeros(x));
                    }
                }

                // check for signed long
                {
                    if ((long) x < 0)
                    {
                        Assert.Equal(64, Math2.NumberOfTrailingZeros((long) x));
                    }
                    else
                    {
                        var expected = BitOperations.TrailingZeroCount(x);
                        checked
                        {
                            Assert.Equal(expected, Math2.NumberOfTrailingZeros((long) x));
                        }
                    }
                }

                var xi = unchecked((uint) x);

                // check for uint
                {
                    var expected = BitOperations.TrailingZeroCount(xi);
                    checked
                    {
                        Assert.Equal((uint) expected, Math2.NumberOfTrailingZeros(xi));
                    }
                }

                // check for signed int
                {
                    if ((int) xi < 0)
                    {
                        Assert.Equal(32, Math2.NumberOfTrailingZeros((int) xi));
                    }
                    else
                    {
                        var expected = BitOperations.TrailingZeroCount(xi);
                        checked
                        {
                            Assert.Equal(expected, Math2.NumberOfTrailingZeros((int) xi));
                        }
                    }
                }
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(1U << 4)]
        [InlineData(1U << 8)]
        [InlineData(1U << 16)]
        [InlineData(1U << 31)]
        [InlineData(1UL << 32)]
        [InlineData(1UL << 63)]
        [InlineData(uint.MinValue)]
        [InlineData(uint.MaxValue)]
        [InlineData(uint.MaxValue - 1)]
        [InlineData(uint.MaxValue - 2)]
        [InlineData(ulong.MinValue)]
        [InlineData(ulong.MaxValue)]
        [InlineData(ulong.MaxValue - 1)]
        [InlineData(ulong.MaxValue - 2)]
        public void Test_NumberOfLeadingZeros(ulong value)
        {
            {
                var expected = BitOperations.LeadingZeroCount(value);
                Assert.Equal((ulong) expected, Math2.NumberOfLeadingZeros(value));
            }

            {
                var expected = BitOperations.LeadingZeroCount((uint) value);
                Assert.Equal((uint) expected, Math2.NumberOfLeadingZeros((uint) value));
            }
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(1U << 4)]
        [InlineData(1U << 8)]
        [InlineData(1U << 16)]
        [InlineData(1U << 31)]
        [InlineData(1UL << 32)]
        [InlineData(1UL << 63)]
        [InlineData(uint.MinValue)]
        [InlineData(uint.MaxValue)]
        [InlineData(uint.MaxValue - 1)]
        [InlineData(uint.MaxValue - 2)]
        [InlineData(ulong.MinValue)]
        [InlineData(ulong.MaxValue)]
        [InlineData(ulong.MaxValue - 1)]
        [InlineData(ulong.MaxValue - 2)]
        public void Test_NumberOfTrailingZeros(ulong value)
        {
            {
                var expected = BitOperations.TrailingZeroCount(value);
                Assert.Equal((ulong) expected, Math2.NumberOfTrailingZeros(value));
            }

            {
                var expected = BitOperations.TrailingZeroCount((uint) value);
                Assert.Equal((uint) expected, Math2.NumberOfTrailingZeros((uint) value));
            }
        }

        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void TestNumberOfLeadingZerosRandomFuzz(int seed)
        {
            var random = new TRandom(seed);
            const int numIter = 1024;
            for (var i = 0; i < numIter; i++)
            {
                var x = ((ulong) random.Next() << 48) ^ ((ulong) random.Next() << 16) ^ (ulong) random.Next();

                // check for ulong
                {
                    var expected = BitOperations.LeadingZeroCount(x);
                    checked
                    {
                        Assert.Equal((ulong) expected, Math2.NumberOfLeadingZeros(x));
                    }
                }

                // check for signed long
                {
                    if ((long) x < 0)
                    {
                        Assert.Equal(0, Math2.NumberOfLeadingZeros((long) x));
                    }
                    else
                    {
                        var expected = BitOperations.LeadingZeroCount(x);
                        checked
                        {
                            Assert.Equal(expected, Math2.NumberOfLeadingZeros((long) x));
                        }
                    }
                }

                var xi = unchecked((uint) x);

                // check for uint
                {
                    var expected = BitOperations.LeadingZeroCount(xi);
                    checked
                    {
                        Assert.Equal((uint) expected, Math2.NumberOfLeadingZeros(xi));
                    }
                }

                // check for signed int
                {
                    if ((int) xi < 0)
                    {
                        Assert.Equal(0, Math2.NumberOfLeadingZeros((int) xi));
                    }
                    else
                    {
                        var expected = BitOperations.LeadingZeroCount(xi);
                        checked
                        {
                            Assert.Equal(expected, Math2.NumberOfLeadingZeros((int) xi));
                        }
                    }
                }
            }
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(1U << 4)]
        [InlineData(1U << 8)]
        [InlineData(1U << 16)]
        [InlineData(1U << 31)]
        [InlineData(uint.MinValue)]
        [InlineData(uint.MaxValue)]
        public void Test_Log2(uint value)
        {
            var expected = BitOperations.Log2(value);
            Assert.Equal((uint) expected, Math2.Log2(value));
        }

        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_Log2_RandomFuzz(int seed)
        {
            var random = new TRandom(seed);
            const int numIter = 1024;
            for (var i = 0; i < numIter; i++)
            {
                var x = ((ulong) random.Next() << 48) ^ ((ulong) random.Next() << 16) ^ (ulong) random.Next();

                // check for ulong
                {
                    var expected = BitOperations.Log2(x);
                    checked
                    {
                        Assert.Equal((ulong) expected, Math2.Log2(x));
                    }
                }

                // check for signed long
                {
                    if ((long) x < 0)
                    {
                        Assert.Equal(0, Math2.Log2((long) x));
                    }
                    else
                    {
                        var expected = BitOperations.Log2(x);
                        checked
                        {
                            Assert.Equal(expected, Math2.Log2((long) x));
                        }
                    }
                }

                var xi = unchecked((uint) x);

                // check for uint
                {
                    var expected = BitOperations.Log2(xi);
                    checked
                    {
                        Assert.Equal((uint) expected, Math2.Log2(xi));
                    }
                }

                // check for signed int
                {
                    if ((int) xi < 0)
                    {
                        Assert.Equal(0, Math2.Log2((int) xi));
                    }
                    else
                    {
                        var expected = BitOperations.Log2(xi);
                        checked
                        {
                            Assert.Equal(expected, Math2.Log2((int) xi));
                        }
                    }
                }
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