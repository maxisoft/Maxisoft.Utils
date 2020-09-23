using System.Runtime.CompilerServices;

namespace Maxisoft.Utils
{
    public static class Math2
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NumberOfTrailingZeros(int x)
        {
            const int numbit = sizeof(int) * 8;
            unchecked
            {
                if (x <= 0)
                {
                    return x < 0 ? 0 : numbit;
                }

                var res = (int) NumberOfTrailingZeros((ulong) x) - numbit;
                return res;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint NumberOfTrailingZeros(uint x)
        {
            const int numbit = sizeof(uint) * 8;
            unchecked
            {
                var res = (uint) NumberOfTrailingZeros((ulong) x) - numbit;
                return res;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long NumberOfTrailingZeros(long x)
        {
            unchecked
            {
                if (x <= 0)
                {
                    return x < 0 ? 0 : sizeof(long) * 8;
                }

                var res = (long) NumberOfTrailingZeros((ulong) x);
                return res;
            }
        }

        public static ulong NumberOfTrailingZeros(ulong i)
        {
            unchecked
            {
                const ulong log2Of64 = 6;
                const ulong numbit = sizeof(ulong) * 8;
                if (i == 0)
                {
                    return numbit;
                }

                var n = numbit - 1;
                for (ulong j = 0; j < log2Of64; j++)
                {
                    var power = (int) (numbit >> (int) (j + 1));

                    var y = i << power;
                    if (y != 0)
                    {
                        n -= (ulong) power;
                        i = y;
                    }
                }

                return n - ((i << 1) >> (int) (numbit - 1));
            }
        }
    }
}