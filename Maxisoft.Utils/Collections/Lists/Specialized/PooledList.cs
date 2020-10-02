using System;
using System.Buffers;
using Maxisoft.Utils.Collections.Allocators;

namespace Maxisoft.Utils.Collections.Lists.Specialized
{
    public class PooledList<T> : ArrayList<T, PooledAllocator<T>>, IDisposable
    {
        public static readonly ArrayPool<T> DefaultPool = ArrayPool<T>.Shared;


        public PooledList(int capacity, ArrayPool<T> arrayPool) : base(0, new PooledAllocator<T>(arrayPool))
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "negative");
            }

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