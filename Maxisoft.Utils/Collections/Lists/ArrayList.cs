using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Maxisoft.Utils.Collections.Allocators;

namespace Maxisoft.Utils.Collections.Lists
{
    public abstract partial class ArrayList<T, TAllocator> : IList<T>, IReadOnlyList<T> where TAllocator : IAllocator<T>
    {
        public static readonly T[] EmptyArray = Array.Empty<T>();

        internal readonly TAllocator Allocator;


        private T[] _array;

        protected internal ArrayList(int capacity, in TAllocator allocator)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "negative");
            }

            Allocator = allocator;
            _array = EmptyArray;
            EnsureCapacity(capacity);
            Count = 0;
        }

        protected internal ArrayList(T[] initialArray, int size, in TAllocator allocator)
        {
            if ((uint) size > (uint) initialArray.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, "out of bounds");
            }

            Allocator = allocator;
            _array = initialArray;
            Count = size;
        }

        public int Capacity
        {
            get => _array.Length;
            set => EnsureCapacity(value);
        }


        public Span<T>.Enumerator GetEnumerator()
        {
            return AsSpan().GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>) new ArraySegment<T>(_array, 0, Count)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>) this).GetEnumerator();
        }

        public void Add(T item)
        {
            GrowIfNeeded();

            _array[Count] = item;
            Count++;
        }

        public void Clear()
        {
            Allocator.Free(ref _array);
            _array = EmptyArray;
            Count = 0;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(_array, 0, array, arrayIndex, Count);
        }


        public bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index < 0)
            {
                return false;
            }

            RemoveAt(index);
            return true;
        }

        public int Count { get; protected set; }

        public bool IsReadOnly => false;

        public int IndexOf(T item)
        {
            return Array.IndexOf(_array, item, 0, Count);
        }

        public void Insert(int index, T item)
        {
            if (index == Count)
            {
                Add(item);
                return;
            }

            CheckForOutOfBounds(index, nameof(index));
            GrowIfNeeded();
            Array.Copy(_array, index, _array, index + 1, Count - index);
            _array[index] = item;
            Count += 1;
        }

        public void RemoveAt(int index)
        {
            CheckForOutOfBounds(index, nameof(index));
            Array.Copy(_array, index + 1, _array, index, Count - 1 - index);
            Count -= 1;
        }


        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                CheckForOutOfBounds(index, nameof(index));
                return _array[index];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                CheckForOutOfBounds(index, nameof(index));
                _array[index] = value;
            }
        }

        protected internal virtual int ComputeGrowSize(int size, int capacity)
        {
            Debug.Assert(size <= capacity);
            var res = checked(capacity * 2);
            res &= ~1;
            res = Math.Max(res, 2);
            Debug.Assert(res >= size);
            return res;
        }

        public void EnsureCapacity(int minimalCapacity)
        {
            if (minimalCapacity > Capacity)
            {
                Allocator.ReAlloc(ref _array, ref minimalCapacity);
            }
        }

        public void ShrinkToFit()
        {
            if (Count != Capacity)
            {
                var capacity = Count;
                Allocator.ReAlloc(ref _array, ref capacity);
                Count = capacity;
            }
        }

        public void Resize(int size, bool clear = true)
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, "negative");
            }

            var prevSize = Count;
            EnsureCapacity(size);
            Count = size;
            if (clear && size < prevSize)
            {
                Array.Clear(_array, size, prevSize - size);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] Data()
        {
            return _array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T At(int index)
        {
            return ref _array[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Front()
        {
            CheckForOutOfBounds(0, nameof(Count));
            return ref _array[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Back()
        {
            CheckForOutOfBounds(Count - 1, nameof(Count));
            return ref _array[Count - 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan()
        {
            Span<T> res = Data();
            return res.Slice(0, Count);
        }


        public static implicit operator Span<T>(ArrayList<T, TAllocator> list)
        {
            return list.AsSpan();
        }

        public static implicit operator ReadOnlySpan<T>(ArrayList<T, TAllocator> list)
        {
            return list.AsSpan();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal bool GrowIfNeeded()
        {
            if (Capacity > Count)
            {
                return false;
            }

            var computeGrowSize = ComputeGrowSize(Count, Capacity);
            Allocator.ReAlloc(ref _array, ref computeGrowSize);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void CheckForOutOfBounds(int index, string paramName,
            string message =
                "Index was out of range. Must be non-negative and less than the size of the collection. (Parameter 'index')")
        {
            if ((uint) index >= (uint) Count)
            {
                throw new ArgumentOutOfRangeException(paramName, index, message);
            }
        }
    }


    public class ArrayList<T> : ArrayList<T, DefaultAllocator<T>>
    {
        protected internal static readonly DefaultAllocator<T> DefaultAllocator = new DefaultAllocator<T>();

        public ArrayList() : this(0)
        {
        }


        public ArrayList(int capacity) : this(capacity, DefaultAllocator)
        {
        }

        public ArrayList(T[] initialArray, int size) : this(initialArray, size, DefaultAllocator)
        {
        }

        public ArrayList(T[] initialArray) : this(initialArray, initialArray.Length)
        {
        }

        internal ArrayList(int capacity, in DefaultAllocator<T> allocator) : base(capacity, in allocator)
        {
        }

        internal ArrayList(T[] initialArray, int size, in DefaultAllocator<T> allocator) : base(initialArray, size,
            in allocator)
        {
        }
    }
}