using System;
using System.Linq;

namespace Maxisoft.Utils.Algorithm
{
    public static class Fibonacci
    {
        #region data

        internal const double Sqrt5 = 2.236067977499789805051477742381393909454345703125;
        private const double Sqrt5Invert = 1 / Sqrt5;
        private const double Sqrt5P1 = Sqrt5 + 1;
        private const double Sqrt5P1Half = Sqrt5P1 * 0.5;

        internal static readonly long[] PreComputed =
        {
            0x0, 0x1, 0x1, 0x2, 0x3, 0x5, 0x8, 0xD, 0x15, 0x22, 0x37, 0x59, 0x90, 0xE9, 0x179, 0x262, 0x3DB, 0x63D,
            0xA18, 0x1055, 0x1A6D, 0x2AC2, 0x452F, 0x6FF1, 0xB520, 0x12511, 0x1DA31, 0x2FF42, 0x4D973, 0x7D8B5, 0xCB228,
            0x148ADD, 0x213D05, 0x35C7E2, 0x5704E7, 0x8CCCC9, 0xE3D1B0, 0x1709E79, 0x2547029, 0x3C50EA2, 0x6197ECB,
            0x9DE8D6D, 0xFF80C38, 0x19D699A5, 0x29CEA5DD, 0x43A53F82, 0x6D73E55F, 0xB11924E1, 0x11E8D0A40, 0x1CFA62F21,
            0x2EE333961, 0x4BDD96882, 0x7AC0CA1E3, 0xC69E60A65, 0x1415F2AC48, 0x207FD8B6AD, 0x3495CB62F5, 0x5515A419A2,
            0x89AB6F7C97, 0xDEC1139639, 0x1686C8312D0, 0x2472D96A909, 0x3AF9A19BBD9, 0x5F6C7B064E2, 0x9A661CA20BB,
            0xF9D297A859D, 0x19438B44A658, 0x28E0B4BF2BF5, 0x42244003D24D, 0x6B04F4C2FE42, 0xAD2934C6D08F,
            0x1182E2989CED1, 0x1C5575E509F60, 0x2DD8587DA6E31, 0x4A2DCE62B0D91, 0x780626E057BC2, 0xC233F54308953,
            0x13A3A1C2360515, 0x1FC6E116668E68, 0x336A82D89C937D, 0x533163EF0321E5, 0x869BE6C79FB562, 0xD9CD4AB6A2D747,
            0x16069317E428CA9, 0x23A367C34E563F0, 0x39A9FADB327F099, 0x5D4D629E80D5489, 0x96F75D79B354522,
            0xF444C01834299AB, 0x18B3C1D91E77DECD, 0x27F80DDAA1BA7878, 0x40ABCFB3C0325745
        };

        #endregion

        public static long Fibo(int n)
        {
            if (n <= 0)
            {
                return 0;
            }

            if (n < PreComputed.Length)
            {
                return PreComputed[n];
            }

            throw new OverflowException($"Use {nameof(ComputeImprecise)} method");
        }

        internal static long Compute(int n)
        {
            long a = 0;
            long b = 1;
            for (long i = 0; i < n; i++)
            {
                var temp = a;

                a = b;
                checked
                {
                    b = temp + b;
                }
            }

            return a;
        }

        public static double ComputeImprecise(int n)
        {
            var res = Sqrt5Invert * Math.Pow(Sqrt5P1Half, n);
            return Math.Round(res);
        }
    }
}