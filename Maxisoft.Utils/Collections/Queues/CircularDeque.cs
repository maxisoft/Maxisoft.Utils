using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Maxisoft.Utils.Collections.Queues
{
    public abstract class CircularDeque<T, TDeque> : ICircularDeque<T> where TDeque : IDeque<T>
    {
        protected readonly TDeque Deque;

        public CircularDeque(TDeque deque, long capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "negative size");
            }

            if (deque.Count > capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity,
                    "deque's size already bigger than capped size");
            }

            Deque = deque;
            Capacity = capacity;
        }

        public bool IsFull => Deque.Count >= Capacity;

        public long Capacity { get; }

        public IEnumerator<T> GetEnumerator()
        {
            return Deque.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) Deque).GetEnumerator();
        }

        public void Add(T item)
        {
            PushBack(in item);
        }

        public void Clear()
        {
            Deque.Clear();
        }

        public bool Contains(T item)
        {
            return Deque.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Deque.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return Deque.Remove(item);
        }

        public int Count => Deque.Count;

        public bool IsReadOnly => Deque.IsReadOnly;

        public ref T At(long index)
        {
            return ref Deque.At(index);
        }

        public ref T Front()
        {
            return ref Deque.Front();
        }

        public bool TryPeekFront(out T result)
        {
            return Deque.TryPeekFront(out result);
        }

        public ref T Back()
        {
            return ref Deque.Back();
        }

        public bool TryPeekBack(out T result)
        {
            return Deque.TryPeekBack(out result);
        }

        public void PushBack(in T element)
        {
            if (DequeOverflow())
            {
                Deque.PopFront();
            }

            Deque.PushBack(in element);
        }

        public void PushFront(in T element)
        {
            if (DequeOverflow())
            {
                Deque.PopBack();
            }

            Deque.PushFront(in element);
        }

        public T PopBack()
        {
            return Deque.PopBack();
        }

        public T PopFront()
        {
            return Deque.PopFront();
        }

        public bool TryPopBack(out T result)
        {
            return Deque.TryPopBack(out result);
        }

        public bool TryPopFront(out T result)
        {
            return Deque.TryPopFront(out result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool DequeOverflow(int elementToAdd = 1)
        {
            Debug.Assert(Deque.Count <= Capacity);
            return Deque.Count + elementToAdd > Capacity;
        }
    }


    [Serializable]
    public class CircularDeque<T> : CircularDeque<T, Deque<T>>, IList<T>, IReadOnlyList<T>
    {
        internal CircularDeque(Deque<T> deque, long capacity) : base(deque, capacity)
        {
        }

        public CircularDeque(long cappedSize) : this(new Deque<T>(OptimalChunkSize(cappedSize)), cappedSize)
        {
        }

        public int IndexOf(T item)
        {
            return Deque.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            Deque.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Deque.RemoveAt(index);
        }

        public T this[int index]
        {
            get => Deque[index];
            set => Deque[index] = value;
        }

        internal static long OptimalChunkSize(long cappedSize)
        {
            return BoundedDeque<T>.OptimalChunkSize(cappedSize);
        }
    }
}