using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Maxisoft.Utils.Collections.Dictionaries;
using Maxisoft.Utils.Collections.Lists;
using Troschuetz.Random;

namespace Maxisoft.Utils.Benchmarks.Collections.Lists
{
    [MemoryDiagnoser]
    public class ListSwapBenchmarks
    {
        private ArrayList<BigStruct> _list = new ArrayList<BigStruct>();

        private readonly TRandom _random = new TRandom();

        [Params(10, 100, 1000)] public int N;

        [IterationSetup]
        public void Setup()
        {
            var random = new TRandom(N);
            _list = new ArrayList<BigStruct>(capacity: N);
            _list.Resize(N);
            for (var i = 0; i < N; i++)
            {
                random.NextBytes(_list[i].AsSpan());                
            }
        }

        [IterationCleanup]
        public void Cleanup()
        {
            _list.Clear();
        }
        
        internal unsafe struct BigStruct
        {
            public const int DataStructSize = 256;
            public fixed byte fixedBuffer[DataStructSize];

            public Span<byte> AsSpan()
            {
                fixed (byte* ptr = fixedBuffer)
                {
                    return new Span<byte>(ptr, DataStructSize);
                }
            }
        }

        [Benchmark(Baseline = true)]
        public void Swap()
        {
            var from = 0;
            var to = _list.Count - 1;
            for (var i = 0; i < _list.Count; i++)
            {
                _list.Swap(from, to);
                from += 1;
                to -= 1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TupleSwap(int a, int b)
        {
            (_list[a], _list[b]) = (_list[b], _list[a]);
        }

        [Benchmark]
        public void SwapWithTuple()
        {
            var from = 0;
            var to = _list.Count - 1;
            for (var i = 0; i < _list.Count; i++)
            {
                TupleSwap(from, to);
                from += 1;
                to -= 1;
            }
        }
    }
}