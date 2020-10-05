using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Maxisoft.Utils.Collections.Allocators;
using Maxisoft.Utils.Collections.UpdateGuards;

#nullable enable

namespace Maxisoft.Utils.Collections.Queues
{
    [DebuggerDisplay("Count = {Count}, Capacity = {Capacity}")]
    [DebuggerTypeProxy(typeof(Deque<>.DebuggerTypeProxyImpl))]
    [Serializable]
    [JsonDequeConverter]
    public partial class Deque<T> : IDeque<T>, IList<T>, IReadOnlyList<T>, IList, ISerializable
    {
        private readonly LinkedList<T[]> _map = new LinkedList<T[]>();
        private readonly UpdateGuardedContainer _version = new UpdateGuardedContainer();
        private static readonly DefaultAllocator<T> DefaultAllocator = new DefaultAllocator<T>(); 
        protected internal readonly IAllocator<T> Allocator = DefaultAllocator;
        protected internal readonly long ChunkSize;
        private InternalPointer _begin;
        private InternalPointer _end;
        public float InitialChunkRatio { get; protected internal set; } = 0.5f;

        internal protected Deque(IAllocator<T> allocator)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            ChunkSize = OptimalChunkSize();
            Allocator = allocator;
        }

        public Deque() : this(DefaultAllocator)
        {
            
        }

        public Deque(long chunkSize, DequeInitialUsage usage = DequeInitialUsage.Both) : this(chunkSize, usage.ToRatio())
        {
        }

        public Deque(long chunkSize, float initialChunkRatio) : this(chunkSize, initialChunkRatio, DefaultAllocator)
        {
            
        }
        
        protected internal Deque(long chunkSize, float initialChunkRatio, IAllocator<T> allocator)
        {
            if (chunkSize <= 0)
            {
                throw new ArgumentException("negative size", nameof(chunkSize));
            }

            if (float.IsNaN(initialChunkRatio) || initialChunkRatio < 0)
            {
                throw new ArgumentException("negative ratio", nameof(initialChunkRatio));
            }

            if (initialChunkRatio > 1)
            {
                throw new ArgumentException("more than 100% ratio", nameof(initialChunkRatio));
            }
            Allocator = allocator;
            InitialChunkRatio = initialChunkRatio;
            ChunkSize = chunkSize;
        }

        protected Deque(
            SerializationInfo info,
            StreamingContext context)
        {
            _version = (UpdateGuardedContainer) info.GetValue(nameof(_version), typeof(UpdateGuardedContainer));
            var count = info.GetInt64(nameof(Count));
            ChunkSize = info.GetInt64(nameof(ChunkSize));
            TrimOnDeletion = info.GetBoolean(nameof(TrimOnDeletion));
            InitialChunkRatio = info.GetSingle(nameof(InitialChunkRatio));

            if (count == 0)
            {
                return;
            }

            T[] array;
            array = (T[]) info.GetValue(nameof(array), typeof(T[]));
            if (array.Length != count)
            {
                throw new SerializationException("Size mismatch");
            }

            foreach (var elem in array)
            {
                PushBack(in elem);
            }
        }

        public bool TrimOnDeletion { get; set; } = false;

        public bool IsEmpty => LongLength == 0;

        public int Length => (int) LongLength;

        // ReSharper disable once MemberCanBePrivate.Global
        public long LongLength { get; protected set; }

        public long Capacity => _map.Count * ChunkSize;

        public T this[long index]
        {
            get => At(index);
            set => At(index) = value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var p = _begin;
            using var ug = _version.CreateGuard();
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
            using var _ = _version.CreateGuard(true);
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
            using var ug = _version.CreateGuard();
            long c = 0;
            var p = _begin;
            while (c < LongLength)
            {
                var added = Math.Min(p.DistanceToEnd, LongLength - c);
                Array.Copy(p.Node.Value, p.Index, array, c + arrayIndex, added);
                c += added;
                p += added;
            }
        }

        public bool Remove(T item)
        {
            var (index, p) = Find(item);
            return RemoveAtDispatch(index, in p);
        }
        
        public bool RemoveLast(in T item)
        {
            var (index, p) = FindLast(item);
            return RemoveAtDispatch(index, in p);
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
            using var _ = _version.CreateGuard(true);
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
            using var _ = _version.CreateGuard(true);
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
                InsertRight(p, in item);
            }
            else
            {
                InsertLeft(p, in item);
            }
        }

        public void RemoveAt(int index)
        {
            var p = GetPointerForIndex(index);
            var res = RemoveAtDispatch(index, p);
            Debug.Assert(res);
        }

        public T this[int index]
        {
            get => At(index);
            set => At(index) = value;
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(_version), _version);
            info.AddValue(nameof(Count), LongLength);
            info.AddValue(nameof(ChunkSize), ChunkSize);
            info.AddValue(nameof(TrimOnDeletion), TrimOnDeletion);
            info.AddValue(nameof(InitialChunkRatio), InitialChunkRatio);
            if (Count <= 0)
            {
                return;
            }

            T[] array = new T[Count];
            CopyTo(array, 0);
            info.AddValue(nameof(array), array, typeof(T[]));
        }

        public virtual long OptimalChunkSize()
        {
            var @sizeof = IntPtr.Size;
            try
            {
                @sizeof = Marshal.SizeOf<T>();
            }
            catch (ArgumentException)
            {
            }

            return 2048 / Math.Max(@sizeof, IntPtr.Size);
        }

        private void Init()
        {
            var startIndex = (long) (ChunkSize * InitialChunkRatio);

            if (_begin.HasNode || _end.HasNode)
            {
                throw new AccessViolationException($"This {nameof(Deque<T>)} had already a node");
            }

            _map.AddFirst(Alloc(ChunkSize));
            _begin = new InternalPointer(_map.First, startIndex, ChunkSize);
            _end = _begin;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            using var _ = _version.CreateGuard(true);
            TrimBeginning();
            TrimEnd();
        }

        private void Prepend(in T item)
        {
            InitIfNeeded();
            using var _ = _version.CreateGuard(true);
            if (_begin.DistanceToBeginning == 0) // chunk is full
            {
                if (_begin.Node.Previous is null)
                {
                    _map.AddFirst(Alloc(ChunkSize));
                }

                _begin = new InternalPointer(_begin.Node.Previous!, ChunkSize - 1, ChunkSize);
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
            using var _ = _version.CreateGuard(true);
            if (_end.DistanceToEnd == 0) // chunk is full
            {
                if (_end.Node.Next is null)
                {
                    _map.AddLast(Alloc(ChunkSize));

                    if (ReferenceEquals(_begin.Node, _end.Node) && !_begin.Valid)
                    {
                        // rare case when Deque.Size == 0 and
                        // both _begin and _end point to the very end of the same chunk
                        _begin += 0; // _begin now point to the beginning of the newly created chunk
                        Debug.Assert(_begin.Valid);
                    }
                }

                _end = new InternalPointer(_end.Node.Next!, 0, ChunkSize);
            }

            _end.Value = item;
            _end += 1;
            LongLength += 1;
        }

        protected T[] Alloc(long size)
        {
            Debug.Assert(size == ChunkSize);
            var intSize = checked((int) size);
            var res = Allocator.Alloc(ref intSize);
            Debug.Assert(res.LongLength >= size);
            return res;
        }

        protected void Free(T[] data)
        {
            Allocator.Free(ref data);
        }

        private bool RemoveAtDispatch(long index, in InternalPointer pointer)
        {
            if (index < 0)
            {
                return false;
            }


            if (index >= LongLength / 2)
            {
                RemoveRight(in pointer);
            }
            else
            {
                RemoveLeft(in pointer);
            }

            return true;
        }

        private void RemoveRight(in InternalPointer pointer)
        {
            using var ug = _version.CreateGuard(true);
            var node = pointer.Node;
            var initialShift = pointer.DistanceToBeginning;
            while (!(node is null))
            {
                ug.Check();
                Array.Copy(node.Value, 1 + initialShift, node.Value, initialShift,
                    ChunkSize - initialShift - 1);
                if (!(node.Next is null))
                {
                    node.Value[ChunkSize - 1] = node.Next.Value[0];
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

        private void RemoveLeft(in InternalPointer pointer)
        {
            using var ug = _version.CreateGuard(true);
            var node = pointer.Node;
            var length = pointer.DistanceToBeginning;
            while (!(node is null))
            {
                ug.Check();
                Array.Copy(node.Value, 0, node.Value, 1, length);
                if (!(node.Previous is null))
                {
                    node.Value[0] = node.Previous.Value[ChunkSize - 1];
                }

                node = node.Previous;
                length = ChunkSize - 1;
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
            return RemoveAtDispatch(index, in p);
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
        
        private (long index, InternalPointer pointer) FindLast(in T item)
        {
            var comparer = EqualityComparer<T>.Default;
            var p = _end - (LongLength > 0 ? 1 : 0);
            for (var i = LongLength - 1; i >= 0; i--)
            {
                if (ReferenceEquals(p.Value, item) || comparer.Equals(item, p.Value))
                {
                    return (i, p);
                }

                p -= 1;
            }

            return (-1, _begin);
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

        public long IndexOfFast(in T item)
        {
            return FindFastPath(in item).index;
        }
        
        public long LastIndexOf(in T item)
        {
            return FindLast(in item).index;
        }

        private void InsertLeft(InternalPointer pointer, in T item)
        {
            Prepend(_begin.Value);
            using var ug = _version.CreateGuard(true);
            var beginIt = _begin + 1;
            var node = beginIt.Node!;
            var initialShift = beginIt.DistanceToBeginning;
            while (!ReferenceEquals(node, pointer.Node))
            {
                ug.Check();
                if (node.Previous is { })
                {
                    node.Previous.Value[ChunkSize - 1] = node.Value[0];
                }

                Array.Copy(node.Value, 1 + initialShift, node.Value, 0 + initialShift,
                    ChunkSize - 1 - initialShift);
                node = node.Next ?? throw new IndexOutOfRangeException();
                initialShift = 0;
            }

            Debug.Assert(ReferenceEquals(node, pointer.Node));

            if (node.Previous is { })
            {
                node.Previous.Value[ChunkSize - 1] = node.Value[0];
            }

            Array.Copy(node.Value, 1, node.Value, 0, pointer.Index);
            (pointer - 1).Value = item;
        }

        private void InsertRight(in InternalPointer pointer, in T item)
        {
            Append((_end - 1).Value);
            using var ug = _version.CreateGuard(true);
            var endIt = _end - 1;
            var node = endIt.Node!;
            var effectiveLength = endIt.DistanceToBeginning;
            while (!ReferenceEquals(node, pointer.Node))
            {
                ug.Check();
                if (node.Next is { })
                {
                    node.Next.Value[0] = node.Value[ChunkSize - 1];
                }

                Array.Copy(node.Value, 0, node.Value, 1,
                    effectiveLength);

                node = node.Previous ?? throw new IndexOutOfRangeException();
                effectiveLength = ChunkSize - 1;
            }

            Debug.Assert(ReferenceEquals(node, pointer.Node));

            if (node.Next is { })
            {
                node.Next.Value[0] = node.Value[ChunkSize - 1];
            }

            Array.Copy(node.Value, pointer.Index, node.Value, pointer.Index + 1,
                ChunkSize - pointer.Index - 1);
            pointer.Value = item;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        protected internal InternalPointer GetPointerForIndex(long index)
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