using System;
using System.Linq;

namespace Maxisoft.Utils.Algorithm
{
    public static class Fibonacci
    {
        private static readonly Lazy<int[]> Cache =
            new Lazy<int[]>(() => Enumerable.Range(0, 100).Select(FiboCompute).ToArray());

        public static int Fibo(int n)
        {
            if (n < Cache.Value.Length)
            {
                return Cache.Value[n];
            }
            return FiboCompute(n);
        }

        private static int FiboCompute(int n)
        {
            var a = 0;
            var b = 1;
            for (var i = 0; i < n; i++)
            {
                var temp = a;
                a = b;
                b = temp + b;
            }
            return a;
        }
    }
}