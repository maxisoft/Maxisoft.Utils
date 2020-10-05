using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Maxisoft.Utils.Collections.LinkedLists;
using Troschuetz.Random;

namespace Maxisoft.Utils.Benchmarks.Collections.LinkedLists
{
    [RPlotExporter]
    [JsonExporter]
    public class LinkedListAsIListBenchmarks
    {
        private readonly LinkedListAsIList<int> _linkedListAsIList = new LinkedListAsIList<int>();
        private readonly TRandom _random = new TRandom();

        [Params(10, 100)] public int N;

        [IterationSetup]
        public void Setup()
        {
            _linkedListAsIList.Clear();
            for (var i = 0; i < N; i++)
            {
                _linkedListAsIList.AddLast(i);
            }
        }

        [IterationCleanup]
        public void Cleanup()
        {
            _linkedListAsIList.Clear();
        }

        [Benchmark]
        public void IndexOfFromStart()
        {
            // note this is clearly not the good way to iterate over a linkedlist
            for (var i = 0; i < N; i++)
            {
                var index = LinkedListExtensions.IndexOfFromStart(_linkedListAsIList, i);
                if (index != i)
                {
                    throw new Exception();
                }
            }
        }

        [Benchmark]
        public void IndexOfFromEnd()
        {
            // note this is clearly not the good way to iterate over a linkedlist
            for (var i = 0; i < N; i++)
            {
                var index = LinkedListExtensions.IndexOfFromEnd(_linkedListAsIList, i);
                if (index != i)
                {
                    throw new Exception();
                }
            }
        }

        [Benchmark]
        public void IndexOfFromBoth()
        {
            // note this is clearly not the good way to iterate over a linkedlist
            for (var i = 0; i < N; i++)
            {
                var index = LinkedListExtensions.IndexOfFromBoth(_linkedListAsIList, i);
                if (index != i)
                {
                    throw new Exception();
                }
            }
        }
    }
}