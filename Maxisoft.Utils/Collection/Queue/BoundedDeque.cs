using System;
using System.Collections.Generic;

namespace Maxisoft.Utils.Collection.Queue
{
    [Serializable]
    public class BoundedDeque<T> : BaseBoundedDeque<T, Deque<T>>, IList<T>, IReadOnlyList<T>
    {
        internal BoundedDeque(Deque<T> deque, long cappedSize) : base(deque, cappedSize)
        {
        }

        public BoundedDeque(long cappedSize) : this(new Deque<T>(OptimalChunkSize(cappedSize)), cappedSize)
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
            if (cappedSize <= 1)
            {
                return 2;
            }

            var mult = 1 / Math.Log(cappedSize + 1, 2);

            return checked((long) (Math.Max(1, mult) * cappedSize));
        }
    }
}