using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Maxisoft.Utils.Collections.LinkedLists;
using Maxisoft.Utils.Collections.Lists;
using Maxisoft.Utils.Collections.Queues;
using Maxisoft.Utils.Collections.Queues.Specialized;
using Troschuetz.Random;

namespace Maxisoft.Utils.Benchmarks.Collections.Queues
{
    public class DequeAddRemoveBenchmarks
    {
        private LinkedListAsIList<DataStruct> _linkedList = new LinkedListAsIList<DataStruct>();
        private Deque<DataStruct> _deque = new Deque<DataStruct>();
        private List<DataStruct> _list = new List<DataStruct>();
        private ArrayList<DataStruct> _arrayList = new ArrayList<DataStruct>();

        [Params(1000, 10_000)] public int N;

        [IterationSetup]
        public void Setup()
        {
            var random = new TRandom(N);
            _linkedList = new LinkedListAsIList<DataStruct>();
            _deque = new Deque<DataStruct>();
            _list = new List<DataStruct>();
            _arrayList = new ArrayList<DataStruct>();
            var x = new DataStruct();
            for (var i = 0; i < N; i++)
            {
                random.NextBytes(x.AsSpan());
                _linkedList.AddLast(x);
                _deque.PushBack(x);
                _list.Add(x);
                _arrayList.Add(x);
            }
        }

        [IterationCleanup]
        public void Cleanup()
        {
            _linkedList.Clear();
            _deque.Clear();
            _list.Clear();
            _arrayList.Clear();
        }

        internal long AddRemove<TList>(in TList list) where TList : IList<DataStruct>
        {
            var random = new TRandom(N);
            var data = new DataStruct();
            var span = data.AsSpan();
            long h = 0;
            var limit = N - 1;
            for (var i = 0; i < limit; i++)
            {
                if (random.NextBoolean())
                {
                    random.NextBytes(span);
                    list.Add(data);
                    h ^= unchecked(((long) list[^1].AsSpan()[0]) << (i % 63));
                }
                else 
                {
                    list.RemoveAt(list.Count - 1);
                    h ^= unchecked(((long) list[^1].AsSpan()[0]) << (i % 63));
                }
            }

            return h;
        }
        
        [Benchmark]
        public long LinkedList_AddRemove()
        {
            return AddRemove(_linkedList);
        }
        
        [Benchmark]
        public long Deque_AddRemove()
        {
            return AddRemove(_deque);
        }

        [Benchmark]
        public long List_AddRemove()
        {
            return AddRemove(_list);
        }
        
        
        [Benchmark]
        public long ArrayList_AddRemove()
        {
            return AddRemove(_arrayList);
        }


        internal unsafe struct DataStruct
        {
            public const int DataStructSize = 128;
            public fixed byte fixedBuffer[DataStructSize];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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