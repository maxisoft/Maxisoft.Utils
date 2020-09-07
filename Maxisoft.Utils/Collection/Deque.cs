using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

#nullable enable

namespace Maxisoft.Utils.Collection
{
    public partial class Deque<T> : IDeque<T>, IList<T>, IReadOnlyList<T>, IList
    {
        private readonly long _chunkSize;

        private readonly LinkedList<T[]> _map = new LinkedList<T[]>();
        private InternalPointer _begin;
        private InternalPointer _end;
        private volatile int _version;

        public Deque()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            _chunkSize = OptimalChunkSize();
        }

        internal Deque(long chunkSize)
        {
            if (chunkSize <= 0)
            {
                throw new ArgumentException($"{nameof(chunkSize)} is invalid");
            }

            _chunkSize = chunkSize;
        }

        protected bool TrimOnDeletion { get; set; } = false;

        public bool IsEmpty => LongLength == 0;

        public int Length => (int) LongLength;
        public long LongLength { get; protected set; }

        public T this[long index]
        {
            get => At(index);
            set => At(index) = value;
        }

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

        public void Clear()
        {
            using var _ = new UpdateGuard(this) {Increment = true};
            while (_map.First != null)
            {
                Free(_map.First.Value);
                _map.RemoveFirst();
            }

            LongLength = 0;
            _begin = default;
            _end = default;
        }

        public bool Contains(T item)
        {
            return FindFastPath(item).index >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (LongLength == 0)
            {
                return;
            }

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
            if (IsEmpty)
            {
                return false;
            }

            var (index, p) = Find(item);
            return DoRemoveAt(index, in p);
        }

        public int Count => Length;

        public bool IsReadOnly => false;

        public ref T At(long index)
        {
            return ref GetPointerForIndex(index).Value;
        }

        public ref T Front()
        {
            if (IsEmpty)
            {
                ThrowForEmptyQueue();
            }

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
            if (IsEmpty)
            {
                ThrowForEmptyQueue();
            }

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

        public void PushBack(in T element)
        {
            Append(in element);
        }

        public void PushFront(in T element)
        {
            Prepend(in element);
        }

        public T PopBack()
        {
            var res = Back();
            using var _ = new UpdateGuard(this) {Increment = true};
            LongLength -= 1;
            _end -= 1;
            if (TrimOnDeletion)
            {
                TrimEnd();
            }

            return res;
        }

        public T PopFront()
        {
            var res = Front();
            using var _ = new UpdateGuard(this) {Increment = true};
            LongLength -= 1;
            _begin += 1;
            if (TrimOnDeletion)
            {
                TrimBeginning();
            }

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

        int IList.Add(object value)
        {
            Add((T) value);
            return (int) (LongLength - 1);
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

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((T[]) array, index);
        }

        public bool IsSynchronized => false;

        public object SyncRoot => this;

        object IList.this[int index]
        {
            get => At(index)!;
            set => At(index) = (T) value;
        }

        public bool IsFixedSize => false;

        public int IndexOf(T item)
        {
            return (int) Find(in item).index;
        }

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

        public void RemoveAt(int index)
        {
            var p = GetPointerForIndex(index);
            var res = DoRemoveAt(index, p);
            Debug.Assert(res);
        }

        public T this[int index]
        {
            get => At(index);
            set => At(index) = value;
        }

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
            LongLength += 1;
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
            LongLength += 1;
        }

        protected virtual T[] Alloc(long size)
        {
            Debug.Assert(size == _chunkSize);
            return new T[size];
        }

        protected virtual void Free(T[] data)
        {
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

            LongLength -= 1;
            _end -= 1;
            if (TrimOnDeletion)
            {
                TrimEnd();
            }
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

            LongLength -= 1;
            _begin += 1;
            if (TrimOnDeletion)
            {
                TrimBeginning();
            }
        }

        public bool RemoveFast(in T item)
        {
            if (IsEmpty)
            {
                return false;
            }

            var (index, p) = FindFastPath(in item);
            return DoRemoveAt(index, in p);
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
            if (IsEmpty)
            {
                return (-1, _end);
            }

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

        public long IndexOfFast(T item)
        {
            return FindFastPath(in item).index;
        }

        private void DoInsertLeft(long index, InternalPointer pointer, in T item)
        {
            Prepend(_begin.Value);
            using var ug = new UpdateGuard(this);
            var beginIt = _begin + 1;
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
            var endIt = _end - 1;
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

        internal InternalPointer GetPointerForIndex(long index)
        {
            InternalPointer p;
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "< 0");
            }

            if (index >= LongLength)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, $">= {LongLength}");
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
            Debug.Assert(LongLength == 0);
            throw new InvalidOperationException($"The {nameof(Deque<T>)} is empty");
        }
    }
}