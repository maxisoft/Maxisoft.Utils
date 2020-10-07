using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Maxisoft.Utils.Collections.Dictionaries;
using Maxisoft.Utils.Collections.Lists;
using Troschuetz.Random;

namespace Maxisoft.Utils.Benchmarks.Collections.Dictionaries
{
    public class ListMoveBenchmarks
    {
        private readonly TRandom _random = new TRandom();
        private List<BigStruct> _list = new List<BigStruct>();
        private ArrayList<BigStruct> _arrayList = new ArrayList<BigStruct>();

        [Params(10, 100, 1000)] public int N;

        [IterationSetup]
        public void Setup()
        {
            _list = new List<BigStruct>(N);
            _arrayList = new ArrayList<BigStruct>(N);
            BigStruct data = default;
            var span = data.AsSpan();
            for (var i = 0; i < N; i++)
            {
                _random.NextBytes(span);
                _list.Add(data);
                _arrayList.Add(data);
            }
        }

        [IterationCleanup]
        public void Cleanup()
        {
            _list.Clear();
            _arrayList.Clear();
        }

        [Benchmark]
        public void SpanMove()
        {
            var from = 0;
            var to = _arrayList.Count - 1;
            for (var i = 0; i < N; i++)
            {
                _arrayList.Move(from, to);
                from += 1;
                to -= 1;
            }
        }

        [Benchmark]
        public void NativeMove()
        {
            var from = 0;
            var to = _list.Count - 1;
            for (var i = 0; i < N; i++)
            {
                NativeMove(_list, from, to);
                from += 1;
                to -= 1;
            }
        }

        internal static void NativeMove(List<BigStruct> list, int fromIndex, int toIndex)
        {
            var tmp = list[fromIndex];
            list.RemoveAt(fromIndex);
            list.Insert(toIndex, tmp);
        }

        /// <summary>
        ///     Note that varying buffer's size shouldn't have a impact on this benchmark
        /// </summary>
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
    }
}