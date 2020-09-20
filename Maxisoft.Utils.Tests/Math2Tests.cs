using Xunit;

namespace Maxisoft.Utils.Tests
{
    public class Math2Tests
    {

        private static ulong NumberOfTrailingZerosSlow(ulong i)
        {
            checked
            {
                if (i == 0) return 64;
                if (i < 0) return 0;
                ulong res = 0;
                while ((i & 1) == 0)
                {
                    if (++res >= 64) return 64;
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
        }
    }
}