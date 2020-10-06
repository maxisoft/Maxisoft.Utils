using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Maxisoft.Utils.Collections.Dictionaries;
using Troschuetz.Random;

namespace Maxisoft.Utils.Benchmarks.Collections.Dictionaries
{
    public class OrderedDictionaryMoveBenchmarks
    {
        private readonly TRandom _random = new TRandom();
        private OrderedDictionary<int, BigStruct> _dictionary = new OrderedDictionary<int, BigStruct>();

        [Params(10, 100, 1000)] public int N;

        [IterationSetup]
        public void Setup()
        {
            _dictionary = new OrderedDictionary<int, BigStruct>(N);

            for (var i = 0; i < N; i++)
            {
                _dictionary.Add(new KeyValuePair<int, BigStruct>(_random.Next(), default));
            }
        }

        [IterationCleanup]
        public void Cleanup()
        {
            _dictionary.Clear();
        }

        [Benchmark]
        public void SpanMove()
        {
            var from = 0;
            var to = _dictionary.Count - 1;
            for (var i = 0; i < _dictionary.Count; i++)
            {
                _dictionary.SpanMove(from, to);
                from += 1;
                to -= 1;
            }
        }

        [Benchmark]
        public void NativeMove()
        {
            var from = 0;
            var to = _dictionary.Count - 1;
            for (var i = 0; i < _dictionary.Count; i++)
            {
                _dictionary.NativeMove(from, to);
                from += 1;
                to -= 1;
            }
        }

        /// <summary>
        ///     Note that varying buffer's size shouldn't have a impact on this benchmark
        /// </summary>
        internal unsafe struct BigStruct
        {
            public fixed sbyte fixedBuffer[256];
        }
    }
}