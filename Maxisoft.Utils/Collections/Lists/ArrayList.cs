using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Maxisoft.Utils.Collections.Lists
{
    public partial class ArrayList<T> : IList<T>, IReadOnlyList<T>
    {
        public static readonly T[] EmptyArray = Array.Empty<T>();

        private T[] _array;

        public ArrayList(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "negative");
            }

            _array = EmptyArray;
            EnsureCapacity(capacity);
            Count = 0;
        }

        public ArrayList() : this(0)
        {
        }

        public ArrayList(T[] initialArray, int size)
        {
            if ((uint) size > (uint) initialArray.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, "out of bounds");
            }


            _array = initialArray;
            Count = size;
        }

        public ArrayList(T[] initialArray) : this(initialArray, initialArray.Length)
        {
        }

        public int Capacity
        {
            get => _array.Length;
            set => EnsureCapacity(value);
        }


        public IEnumerator<T> GetEnumerator()
        {
            var c = 0;
            foreach (var element in _array)
            {
                if (c++ >= Count)
                {
                    yield break;
                }

                yield return element;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            GrowIfNeeded();

            _array[Count] = item;
            Count++;
        }

        public void Clear()
        {
            Free(_array);
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

        protected internal virtual T[] Alloc(int size)
        {
            return new T[size];
        }

        protected internal virtual void Free(T[] array)
        {
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

        protected internal virtual void ReAlloc(ref T[] array, int actualSize, int capacity)
        {
            if (capacity == 0)
            {
                if (ReferenceEquals(array, EmptyArray))
                {
                    return;
                }

                Free(array);
                array = EmptyArray;
                return;
            }

            if (ReferenceEquals(array, EmptyArray))
            {
                array = Alloc(capacity);
            }
            else
            {
                var old = array;
                Array.Resize(ref array, capacity);
                Free(old);
            }
        }

        public void EnsureCapacity(int minimalCapacity)
        {
            if (minimalCapacity > Capacity)
            {
                ReAlloc(ref _array, Count, minimalCapacity);
            }
        }

        public void ShrinkToFit()
        {
            if (Count != Capacity)
            {
                ReAlloc(ref _array, Count, Count);
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
            return ref _array[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Back()
        {
            return ref _array[Count - 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan()
        {
            Span<T> res = Data();
            return res.Slice(0, Count);
        }


        public static implicit operator Span<T>(ArrayList<T> list)
        {
            return list.AsSpan();
        }

        public static implicit operator ReadOnlySpan<T>(ArrayList<T> list)
        {
            return list.AsSpan();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool GrowIfNeeded()
        {
            if (Capacity > Count)
            {
                return false;
            }

            ReAlloc(ref _array, Count, ComputeGrowSize(Count, Capacity));
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
}