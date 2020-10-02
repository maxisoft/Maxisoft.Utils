using System;
using System.Runtime.CompilerServices;

namespace Maxisoft.Utils.Collections.Allocators
{
    public struct DefaultAllocator<T> : IAllocator<T>
    {
        public static readonly T[] EmptyArray = Array.Empty<T>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] Alloc(ref int capacity)
        {
            return capacity == 0 ? EmptyArray : new T[capacity];
        }

        // ReSharper disable once RedundantAssignment
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Free(ref T[] array)
        {
            array = EmptyArray;
        }

        public void ReAlloc(ref T[] array, ref int capacity)
        {
            if (capacity == 0)
            {
                if (ReferenceEquals(array, EmptyArray))
                {
                    return;
                }

                Free(ref array);
                return;
            }

            if (ReferenceEquals(array, EmptyArray))
            {
                array = Alloc(ref capacity);
            }
            else
            {
                var old = array;
                Array.Resize(ref array, capacity);
                Free(ref old);
            }
        }
    }
}