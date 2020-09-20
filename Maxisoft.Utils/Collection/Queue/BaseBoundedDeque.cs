using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Maxisoft.Utils.Collection.Queue
{
    public abstract class BaseBoundedDeque<T, TDeque> : IBoundedDeque<T> where TDeque : IDeque<T>
    {
        protected readonly TDeque Deque;

        public BaseBoundedDeque(TDeque deque, long cappedSize)
        {
            if (cappedSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cappedSize), cappedSize, "negative size");
            }

            if (deque.Count > cappedSize)
            {
                throw new ArgumentOutOfRangeException(nameof(cappedSize), cappedSize,
                    "deque's size already bigger than capped size");
            }

            Deque = deque;
            CappedSize = cappedSize;
        }

        public bool IsFull => Deque.Count >= CappedSize;

        public long CappedSize { get; }

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
            ThrowForDequeFull();
            Deque.PushBack(in element);
        }

        public void PushFront(in T element)
        {
            ThrowForDequeFull();
            Deque.PushFront(in element);
        }

        public bool TryPushBack(in T element)
        {
            if (DequeOverflow())
            {
                return false;
            }

            Deque.PushBack(in element);
            return true;
        }

        public bool TryPushFront(in T element)
        {
            if (DequeOverflow())
            {
                return false;
            }

            Deque.PushFront(in element);
            return true;
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
            Debug.Assert(Deque.Count <= CappedSize);
            return Deque.Count + elementToAdd > CappedSize;
        }

        protected void ThrowForDequeFull(int elementToAdd = 1)
        {
            if (DequeOverflow(elementToAdd))
            {
                throw new InvalidOperationException("Deque full");
            }
        }
    }
}