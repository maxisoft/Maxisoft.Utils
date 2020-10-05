using System;
using System.Collections;
using BenchmarkDotNet.Attributes;
using Maxisoft.Utils.Collections.Lists;
using Maxisoft.Utils.Collections.Spans;
using Troschuetz.Random;

namespace Maxisoft.Utils.Benchmarks.Collections.Spans
{
    [RPlotExporter]
    [JsonExporter]
    public class BitSpanBenchmarks
    {
        private const int repeat = 32; 
        private ArrayList<int> Data = new ArrayList<int>();
        private readonly TRandom _random = new TRandom();

        [Params(1, 10, 100, 1000, 10_000)] public int N;

        [IterationSetup]
        public void Setup()
        {
            Data.Resize(N);
            for (var i = 0; i < N; i++)
            {
                Data[i] = _random.Next();
            }
        }

        [IterationCleanup]
        public void Cleanup()
        {
            Data = new ArrayList<int>();
        }

        [Benchmark]
        public void BitArray()
        {
            var ba = new BitArray(Data.Data());
            for (var i = 0; i < repeat; i++)
            {
                ba.And(ba);
                ba.Or(ba);
                ba.Xor(ba);
                ba.SetAll(i % 2 == 0);
            }
        }

        [Benchmark]
        public void BitSpan()
        {
            BitSpan ba = Data.AsSpan();
            for (var i = 0; i < repeat; i++)
            {
                ba.And(ba);
                ba.Or(ba);
                ba.Xor(ba);
                ba.SetAll(i % 2 == 0);
            }
        }
        
        [Benchmark]
        public void BitSpanWithCopy()
        {
            Span<int> data = stackalloc int[10_000];
            Data.AsSpan().CopyTo(data);
            BitSpan ba = data.Slice(0, N);
            for (var i = 0; i < repeat; i++)
            {
                ba.And(ba);
                ba.Or(ba);
                ba.Xor(ba);
                ba.SetAll(i % 2 == 0);
            }
        }
    }
}