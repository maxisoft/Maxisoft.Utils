using System;
using System.Buffers;

namespace Maxisoft.Utils.Collections.Allocators
{
    public class PooledAllocator<T> : IAllocator<T>
    {
        public static readonly T[] EmptyArray = Array.Empty<T>();

        public static readonly ArrayPool<T> DefaultPool = ArrayPool<T>.Shared;

        public readonly ArrayPool<T> ArrayPool;

        public PooledAllocator(ArrayPool<T>? arrayPool = null)
        {
            ArrayPool = arrayPool ?? DefaultPool;
        }

        public T[] Alloc(ref int capacity)
        {
            return capacity == 0 ? EmptyArray : ArrayPool.Rent(capacity);
        }

        public void Free(ref T[] array)
        {
            if (!ReferenceEquals(array, EmptyArray))
            {
                ArrayPool.Return(array);
            }
        }

        public void ReAlloc(ref T[] array, ref int capacity)
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
                    var oldCapacity = array.Length;
                    array = Alloc(ref capacity);
                    Array.Copy(old, array, Math.Min(oldCapacity, array.Length));
                }
            }
            finally
            {
                Free(ref old);
            }
        }
    }
}