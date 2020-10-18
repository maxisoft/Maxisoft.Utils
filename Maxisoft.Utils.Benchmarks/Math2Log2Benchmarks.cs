using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using BenchmarkDotNet.Attributes;
using Troschuetz.Random;

namespace Maxisoft.Utils.Benchmarks
{
    public class Math2Log2Benchmarks
    {
        private List<double> floatingPoints;
        private List<ulong> values;

        [GlobalSetup]
        public void Setup()
        {
            const int n = 16 * 1024;
            floatingPoints = new List<double>(n);
            values = new List<ulong>(n);
            var random = new TRandom(n);
            for (var i = 0; i < n; i++)
            {
                var val = random.Next();
                floatingPoints.Add(val);
                values.Add((ulong) val);
            }
        }

        [Benchmark(Baseline = true)]
        public double Math_Log()
        {
            // Note the following add 1 float operation for each element
            return floatingPoints.Aggregate(0.0, (current, f) => current * Math.Log(f, 2));
        }

        [Benchmark]
        public ulong Math2_Log2()
        {
            // Note the following add 1 long operation for each element
            return values.Aggregate(0UL, (current, f) => current * Math2.Log2(f));
        }
        
        
        [Benchmark]
        public ulong BitOperation_Log2()
        {
            // Note the following add 1 long operation for each element
            return values.Aggregate(0UL, (current, f) => current * (ulong) BitOperations.Log2(f));
        }
    }
}