using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

#nullable enable

namespace Maxisoft.Utils.Collection
{
    public class Deque<T> : IDeque<T>, IList<T>, IReadOnlyList<T>, IList
    {
        internal readonly struct InternalPointer
        {
            private readonly LinkedListNode<T[]>? _node;
            internal readonly long Index;

            public InternalPointer(in LinkedListNode<T[]> node, long index)
            {
                _node = node;
                Index = index;
            }


            internal LinkedListNode<T[]> Node => _node!;
            public ref T Value => ref Node.Value[Index];

            public long DistanceToBeginning => Index;
            public long DistanceToEnd => Node.Value.LongLength - Index;

            public bool HasNode => !ReferenceEquals(_node, null);
            public bool Valid => HasNode && Index >= 0 && Index < Node.Value.LongLength;

            public static InternalPointer operator +(InternalPointer p, long inc)
            {
                if (inc < 0) return p - (inc * -1);
                var originalInc = inc;
                var res = p;
                var @continue = inc >= 0;
                while (@continue)
                {
                    @continue = inc != 0;
                    if (res.DistanceToEnd == 0)
                    {
                        if (res.Node.Next is null)
                        {
                            if (inc == 0) // stop here as res pointing at the very end of the allocated space
                            {
                                break;
                            }

                            throw new IndexOutOfRangeException($"Cannot move pointer to +{originalInc}");
                        }

                        res = new InternalPointer(res.Node.Next, 0);
                    }
                    else if (res.DistanceToEnd >= inc)
                    {
                        res = new InternalPointer(res.Node, res.Index + inc);
                        inc = 0;
                    }
                    else
                    {
                        var distanceToEnd = res.DistanceToEnd;
                        res = new InternalPointer(res.Node, res.Index + distanceToEnd);
                        inc -= distanceToEnd;
                    }
                }

                return res;
            }

            public static InternalPointer operator -(InternalPointer p, long decr)
            {
                if (decr < 0) return p + (decr * -1);
                var originalDecr = decr;
                var res = p;
                while (decr > 0)
                {
                    if (res.DistanceToBeginning == 0)
                    {
                        if (res.Node.Previous is null)
                        {
                            if (decr == 0) // stop here as res pointing at the very beginning of the allocated space
                            {
                                break;
                            }

                            throw new IndexOutOfRangeException($"Cannot move pointer to -{originalDecr}");
                        }

                        res = new InternalPointer(res.Node.Previous, res.Node.Previous.Value.Length);
                    }
                    else if (res.DistanceToBeginning >= decr)
                    {
                        res = new InternalPointer(res.Node, res.Index - decr);
                        decr = 0;
                    }
                    else
                    {
                        var distanceToBeginning = res.DistanceToBeginning;
                        res = new InternalPointer(res.Node, res.Index - distanceToBeginning);
                        decr -= distanceToBeginning;
                    }
                }

                return res;
            }
        }

        internal struct UpdateGuard : IDisposable
        {
            internal readonly int Version;
            internal readonly Deque<T> Deque;
            internal bool Increment { get; set; }

            public UpdateGuard(Deque<T> deque)
            {
                Version = deque._version;
                Deque = deque;
                Increment = false;
            }

            internal void Check()
            {
                if (Deque._version != Version)
                {
                    throw new InvalidOperationException("Concurrent modification detected");
                }
            }

            public void Dispose()
            {
                Check();
                if (!Increment)
                {
                    return;
                }

                var res = Interlocked.Increment(ref Deque._version);
                if (res != Version + 1)
                {
                    throw new InvalidOperationException("Concurrent modification detected");
                }
            }
        }

        private InternalPointer _begin;
        private InternalPointer _end;
        private readonly long _chunkSize;
        private long _size;
        private volatile int _version;
        protected bool TrimOnDeletion { get; set; } = false;

        private readonly LinkedList<T[]> _map = new LinkedList<T[]>();

        protected virtual long OptimalChunkSize()
        {
            var @sizeof = IntPtr.Size;
            try
            {
                @sizeof = Marshal.SizeOf<T>();
            }
            catch (ArgumentException e)
            {
            }

            return 2048 / Math.Max(@sizeof, IntPtr.Size);
        }

        public Deque()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            _chunkSize = OptimalChunkSize();
        }

        internal Deque(long chunkSize)
        {
            if (chunkSize <= 0) throw new ArgumentException($"{nameof(chunkSize)} is invalid");
            _chunkSize = chunkSize;
        }

        private void Init()
        {
            var middle = (_chunkSize - 1) / 2;
            if (_begin.HasNode || _end.HasNode)
            {
                throw new AccessViolationException($"This {nameof(Deque<T>)} is screwed");
            }

            _map.AddFirst(Alloc(_chunkSize));
            _begin = new InternalPointer(_map.First, middle);
            _end = _begin;
        }

        private void InitIfNeeded()
        {
            if (!_begin.HasNode || !_end.HasNode)
            {
                Init();
            }
        }

        private void TrimBeginning()
        {
            if (!_begin.HasNode)
            {
                return;
            }

            var node = _begin.Node.Previous;
            while (node is { })
            {
                Free(node.Value);
                _map.Remove(node);
                node = node.Previous;
            }
        }

        private void TrimEnd()
        {
            if (!_end.HasNode)
            {
                return;
            }

            var node = _end.Node.Next;
            while (node is { })
            {
                Free(node.Value);
                _map.Remove(node);
                node = node.Next;
            }
        }

        public void TrimExcess()
        {
            using var _ = new UpdateGuard(this) {Increment = true};
            TrimBeginning();
            TrimEnd();
        }

        private void Prepend(in T item)
        {
            InitIfNeeded();
            using var _ = new UpdateGuard(this) {Increment = true};
            if (_begin.DistanceToBeginning == 0) // chunk is full
            {
                if (_begin.Node.Previous is null)
                {
                    _map.AddFirst(Alloc(_chunkSize));
                }

                _begin = new InternalPointer(_begin.Node.Previous!, _chunkSize - 1);
            }
            else
            {
                _begin -= 1;
            }

            _begin.Value = item;
            _size += 1;
        }


        private void Append(in T item)
        {
            InitIfNeeded();
            using var _ = new UpdateGuard(this) {Increment = true};
            if (_end.DistanceToEnd == 0) // chunk is full
            {
                if (_end.Node.Next is null)
                {
                    _map.AddLast(Alloc(_chunkSize));

                    if (ReferenceEquals(_begin.Node, _end.Node) && !_begin.Valid)
                    {
                        // rare case when Deque.Size == 0 and
                        // both _begin and _end point to the very end of the same chunk
                        _begin += 0; // _begin now point to the beginning of the newly created chunk
                        Debug.Assert(_begin.Valid);
                    }
                }

                _end = new InternalPointer(_end.Node.Next!, 0);
            }

            _end.Value = item;
            _end += 1;
            _size += 1;
        }

        protected virtual T[] Alloc(long size)
        {
            Debug.Assert(size == _chunkSize);
            return new T[size];
        }

        protected virtual void Free(T[] data)
        {
        }

        public bool IsEmpty => _size == 0;

        public int Length => (int) _size;
        public long LongLength => _size;

        public IEnumerator<T> GetEnumerator()
        {
            var p = _begin;
            using var ug = new UpdateGuard(this);
            for (long i = 0; i < LongLength; i++)
            {
                ug.Check();
                yield return p.Value;
                p += 1;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            Append(in item);
        }

        int IList.Add(object value)
        {
            Add((T) value);
            return (int) (_size - 1);
        }

        public void Clear()
        {
            using var _ = new UpdateGuard(this) {Increment = true};
            while (_map.First != null)
            {
                Free(_map.First.Value);
                _map.RemoveFirst();
            }

            _size = 0;
            _begin = default;
            _end = default;
        }

        bool IList.Contains(object value)
        {
            if (value is T t)
            {
                return Contains(t);
            }

            return false;
        }

        int IList.IndexOf(object value)
        {
            if (value is T t)
            {
                return IndexOf(t);
            }

            return -1;
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (T) value);
        }

        void IList.Remove(object value)
        {
            if (value is T t)
            {
                Remove(t);
            }
        }

        public bool Contains(T item) => FindFastPath(item).index >= 0;

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (LongLength == 0) return;
            using var ug = new UpdateGuard(this);
            long c = 0;
            var p = _begin;
            while (c < LongLength)
            {
                ug.Check();
                var added = Math.Min(p.DistanceToEnd, LongLength - c);
                Array.Copy(p.Node.Value, p.Index, array, c + arrayIndex, added);
                c += added;
                p += added;
            }
        }

        public bool Remove(T item)
        {
            if (IsEmpty) return false;

            var (index, p) = Find(item);
            return DoRemoveAt(index, in p);
        }

        private bool DoRemoveAt(long index, in InternalPointer pointer)
        {
            if (index < 0)
            {
                return false;
            }


            if (index >= LongLength / 2)
            {
                DoRemoveRight(index, in pointer);
            }
            else
            {
                DoRemoveLeft(index, in pointer);
            }

            return true;
        }

        private void DoRemoveRight(long index, in InternalPointer pointer)
        {
            using var ug = new UpdateGuard(this) {Increment = true};
            var node = pointer.Node;
            var initialShift = pointer.DistanceToBeginning;
            while (!(node is null))
            {
                ug.Check();
                Array.Copy(node.Value, 1 + initialShift, node.Value, initialShift,
                    node.Value.LongLength - initialShift - 1);
                if (!(node.Next is null))
                {
                    node.Value[node.Value.Length - 1] = node.Next.Value[0];
                }

                node = node.Next;
                initialShift = 0;
            }

            _size -= 1;
            _end -= 1;
            if (TrimOnDeletion) TrimEnd();
        }

        private void DoRemoveLeft(long index, in InternalPointer pointer)
        {
            using var ug = new UpdateGuard(this) {Increment = true};
            var node = pointer.Node;
            var length = pointer.DistanceToBeginning;
            while (!(node is null))
            {
                ug.Check();
                Array.Copy(node.Value, 0, node.Value, 1, length);
                if (!(node.Previous is null))
                {
                    node.Value[0] = node.Previous.Value[node.Previous.Value.Length - 1];
                }

                node = node.Previous;
                length = node?.Value.Length - 1 ?? default;
            }

            _size -= 1;
            _begin += 1;
            if (TrimOnDeletion) TrimBeginning();
        }

        public bool RemoveFast(in T item)
        {
            if (IsEmpty) return false;

            var (index, p) = FindFastPath(in item);
            return DoRemoveAt(index, in p);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((T[]) array, index);
        }

        public int Count => Length;

        public bool IsSynchronized => false;

        public object SyncRoot => this;

        public bool IsReadOnly => false;

        object IList.this[int index]
        {
            get => At(index)!;
            set => At(index) = (T) value;
        }

        private (long index, InternalPointer pointer) Find(in T item)
        {
            var comparer = EqualityComparer<T>.Default;
            var p = _begin;
            for (long i = 0; i < LongLength; i++)
            {
                if (ReferenceEquals(p.Value, item) || comparer.Equals(item, p.Value))
                {
                    return (i, p);
                }

                p += 1;
            }

            return (-1, _end);
        }

        private (long index, InternalPointer pointer) FindFastPath(in T item)
        {
            if (IsEmpty) return (-1, _end);
            var comparer = EqualityComparer<T>.Default;
            var forward = _begin;
            var backward = _end - 1;
            for (long i = 0; i < (LongLength + 1) / 2; i++)
            {
                if (ReferenceEquals(forward.Value, item) || comparer.Equals(item, forward.Value))
                {
                    return (i, forward);
                }

                if (ReferenceEquals(backward.Value, item) || comparer.Equals(item, backward.Value))
                {
                    return (LongLength - i - 1, backward);
                }

                forward += 1;
                backward -= 1;
            }

            return (-1, _end);
        }

        public int IndexOf(T item) => (int) Find(in item).index;
        public long IndexOfFast(T item) => FindFastPath(in item).index;

        public void Insert(int index, T item)
        {
            if (index == Length)
            {
                Append(in item);
                return;
            }

            var p = GetPointerForIndex(index);
            if (index >= LongLength / 2)
            {
                DoInsertRight(index, p, in item);
            }
            else
            {
                DoInsertLeft(index, p, in item);
            }
        }

        private void DoInsertLeft(long index, InternalPointer pointer, in T item)
        {
            Prepend(_begin.Value);
            using var ug = new UpdateGuard(this);
            var beginIt = (_begin + 1);
            var node = beginIt.Node!;
            var initialShift = beginIt.DistanceToBeginning;
            while (!ReferenceEquals(node, pointer.Node))
            {
                ug.Check();
                if (node.Previous is { })
                {
                    node.Previous.Value[node.Previous.Value.LongLength - 1] = node.Value[0];
                }

                Array.Copy(node.Value, 1 + initialShift, node.Value, 0 + initialShift,
                    node.Value.LongLength - 1 - initialShift);
                node = node.Next ?? throw new IndexOutOfRangeException();
                initialShift = 0;
            }

            Debug.Assert(ReferenceEquals(node, pointer.Node));

            if (node.Previous is { })
            {
                node.Previous.Value[node.Previous.Value.LongLength - 1] = node.Value[0];
            }

            Array.Copy(node.Value, 1, node.Value, 0, pointer.Index);
            (pointer - 1).Value = item;
        }

        private void DoInsertRight(long index, in InternalPointer pointer, in T item)
        {
            Append((_end - 1).Value);
            using var ug = new UpdateGuard(this);
            var endIt = (_end - 1);
            var node = endIt.Node!;
            var effectiveLength = endIt.DistanceToBeginning;
            while (!ReferenceEquals(node, pointer.Node))
            {
                ug.Check();
                if (node.Next is { })
                {
                    node.Next.Value[0] = node.Value[node.Value.LongLength - 1];
                }

                Array.Copy(node.Value, 0, node.Value, 1,
                    effectiveLength);

                node = node.Previous ?? throw new IndexOutOfRangeException();
                effectiveLength = node.Value.Length - 1;
            }

            Debug.Assert(ReferenceEquals(node, pointer.Node));

            if (node.Next is { })
            {
                node.Next.Value[0] = node.Value[node.Value.LongLength - 1];
            }

            Array.Copy(node.Value, pointer.Index, node.Value, pointer.Index + 1,
                node.Value.LongLength - pointer.Index - 1);
            pointer.Value = item;
        }

        public void RemoveAt(int index)
        {
            var p = GetPointerForIndex(index);
            var res = DoRemoveAt(index, p);
            Debug.Assert(res);
        }

        public bool IsFixedSize => false;

        internal InternalPointer GetPointerForIndex(long index)
        {
            InternalPointer p;
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "< 0");
            }

            if (index >= _size)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, $">= {_size}");
            }

            if (index > LongLength / 2)
            {
                p = _end;
                p -= LongLength - index;
            }
            else
            {
                p = _begin;
                p += index;
            }

            return p;
        }

        private void ThrowForEmptyQueue()
        {
            Debug.Assert(_size == 0);
            throw new InvalidOperationException($"The {nameof(Deque<T>)} is empty");
        }

        public ref T At(long index) => ref GetPointerForIndex(index).Value;

        public ref T Front()
        {
            if (IsEmpty) ThrowForEmptyQueue();
            return ref _begin.Value;
        }

        public bool TryPeekFront(out T result)
        {
            if (IsEmpty)
            {
                result = default!;
                return false;
            }

            result = Front();
            return true;
        }

        public ref T Back()
        {
            if (IsEmpty) ThrowForEmptyQueue();
            return ref (_end - 1).Value;
        }

        public bool TryPeekBack(out T result)
        {
            if (IsEmpty)
            {
                result = default!;
                return false;
            }

            result = Back();
            return true;
        }

        public void PushBack(in T element) => Append(in element);

        public void PushFront(in T element) => Prepend(in element);

        public T PopBack()
        {
            var res = Back();
            using var _ = new UpdateGuard(this) {Increment = true};
            _size -= 1;
            _end -= 1;
            if (TrimOnDeletion) TrimEnd();
            return res;
        }

        public T PopFront()
        {
            var res = Front();
            using var _ = new UpdateGuard(this) {Increment = true};
            _size -= 1;
            _begin += 1;
            if (TrimOnDeletion) TrimBeginning();
            return res;
        }

        public bool TryPopBack(out T result)
        {
            if (IsEmpty)
            {
                result = default!;
                return false;
            }

            result = PopBack();
            return true;
        }

        public bool TryPopFront(out T result)
        {
            if (IsEmpty)
            {
                result = default!;
                return false;
            }

            result = PopFront();
            return true;
        }

        public T this[int index]
        {
            get => At(index);
            set => At(index) = value;
        }

        public T this[long index]
        {
            get => At(index);
            set => At(index) = value;
        }
    }
}