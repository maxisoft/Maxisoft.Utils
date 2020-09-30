using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Maxisoft.Utils.Collections.LinkedLists;
using Troschuetz.Random;

namespace Maxisoft.Utils.Benchmark.Collections.LinkedLists
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31, baseline: true)]
    [RPlotExporter]
    public class LinkedListAsIListBenchmarks
    {

        private readonly LinkedListAsIList<int> _linkedListAsIList = new LinkedListAsIList<int>();
        private readonly TRandom _random = new TRandom();
        
        [Params(1000, 10000)]
        public int N;
        
        [GlobalSetup]
        public void Setup()
        {
            _linkedListAsIList.Clear();
            for (var i = 0; i < N; i++)
            {
                _linkedListAsIList.AddLast(i);
            }
        }

        [Benchmark]
        public void IndexOfFromStart()
        {
            for (var i = 0; i < N; i++)
            {
                LinkedListExtensions.IndexOfFromStart(_linkedListAsIList, i);
            }
        }

        [Benchmark]
        public void IndexOfFromEnd()
        {
            for (var i = 0; i < N; i++)
            {
                LinkedListExtensions.IndexOfFromEnd(_linkedListAsIList, i);
            }
        }
        
        [Benchmark]
        public void IndexOfFromBoth()
        {
            for (var i = 0; i < N; i++)
            {
                LinkedListExtensions.IndexOfFromBoth(_linkedListAsIList, i);
            }
        }
    }
    
}