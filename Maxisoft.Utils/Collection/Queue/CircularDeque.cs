using System;
using System.Collections.Generic;

namespace Maxisoft.Utils.Collection.Queue
{
    [Serializable]
    public class CircularDeque<T> : BaseCircularDeque<T, Deque<T>>, IList<T>, IReadOnlyList<T>
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