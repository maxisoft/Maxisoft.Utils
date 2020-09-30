using System;
using BenchmarkDotNet.Running;

namespace Maxisoft.Utils.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}