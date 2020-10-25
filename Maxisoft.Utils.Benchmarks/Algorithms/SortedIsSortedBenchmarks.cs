using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Maxisoft.Utils.Algorithms;
using Maxisoft.Utils.Collections.Lists;
using Troschuetz.Random;

namespace Maxisoft.Utils.Benchmarks.Algorithms
{
    [MemoryDiagnoser]
    public class SortedIsSortedBenchmarks
    {
        private readonly TRandom _random = new TRandom();
        

        [Params(10, 100, 1000)] public int N;
        private ArrayList<long> _data = new ArrayList<long>();
        private ArrayList<long> _sortedData = new ArrayList<long>();

        [GlobalSetup]
        public void Setup()
        {
            _data = new ArrayList<long>(N);
            _sortedData = new ArrayList<long>(N);
            var random = new TRandom(N);
            for (var i = 0; i < N; i++)
            {
                var value = random.Next(N);
                _data.Add(value);
                _sortedData.Add(value);
            }
            _sortedData.Sort();
        }

        [Benchmark]
        public bool Test_IsSorted_Span()
        {
            var a = _data.AsSpan().IsSorted();
            var b = _sortedData.AsSpan().IsSorted();
            return a ^ b;
        }
        
        [Benchmark(Baseline = true)]
        public bool Test_IsSorted_Span_With_Comparator()
        {
            var cmp = Comparer<long>.Default;
            var a = _data.AsSpan().IsSorted(cmp);
            var b = _sortedData.AsSpan().IsSorted(cmp);
            return a ^ b;
        }
        
        [Benchmark]
        public bool Test_IsSorted_IEnumerable()
        {
            var a = ((IEnumerable<long>)_data).IsSorted();
            var b = ((IEnumerable<long>)_sortedData).IsSorted();
            return a ^ b;
        }
    }
}