using System;
using System.Buffers;

namespace Maxisoft.Utils.Collections.Lists.Specialized
{
    public class PooledList<T> : ArrayList<T>, IDisposable
    {
        public static readonly ArrayPool<T> DefaultPool = ArrayPool<T>.Shared;

        public readonly ArrayPool<T> ArrayPool;

        public PooledList(int capacity, ArrayPool<T> arrayPool) : base(0)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "negative");
            }

            ArrayPool = arrayPool;
            EnsureCapacity(capacity);
        }

        public PooledList() : this(0, DefaultPool)
        {
        }

        public PooledList(int capacity) : this(capacity, DefaultPool)
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected internal override T[] Alloc(int size)
        {
            return ArrayPool.Rent(size);
        }

        protected internal override void Free(T[] array)
        {
            if (!ReferenceEquals(array, EmptyArray))
            {
                ArrayPool.Return(array);
            }
        }

        protected internal override void ReAlloc(ref T[] array, int actualSize, int capacity)
        {
            if (capacity == array.Length)
            {
                return;
            }

            var old = array;
            try
            {
                if (capacity == 0)
                {
                    array = EmptyArray;
                }
                else
                {
                    array = Alloc(capacity);
                    Array.Copy(old, array, Math.Min(actualSize, capacity));
                }
            }
            finally
            {
                Free(old);
            }
        }

        private void ReleaseUnmanagedResources()
        {
            Clear();
        }
        
        protected internal virtual void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
        }

        ~PooledList()
        {
            Dispose(false);
        }
    }
}