using System;
using Maxisoft.Utils.Collections.Allocators;

namespace Maxisoft.Utils.Collections.Lists
{
    public class ArrayListWrapper<T> : ArrayList<T, ArrayListWrapper<T>.NoAlloc>
    {
        protected internal static NoAlloc DefaultNoAlloc = new NoAlloc();

        public ArrayListWrapper() : base(0, DefaultNoAlloc)
        {
        }

        public ArrayListWrapper(T[] array) : this(array, array.Length)
        {
        }

        public ArrayListWrapper(T[] array, int size) : base(array, size, DefaultNoAlloc)
        {
        }

        protected internal override int ComputeGrowSize(int size, int capacity)
        {
            throw new InvalidOperationException("Cannot grow");
        }

        public readonly struct NoAlloc : IAllocator<T>
        {
            public T[] Alloc(ref int capacity)
            {
                throw new InvalidOperationException("Cannot allocate new array");
            }

            public void Free(ref T[] array)
            {
            }

            public void ReAlloc(ref T[] array, ref int capacity)
            {
                if (array.Length != capacity)
                {
                    throw new InvalidOperationException("Cannot allocate new array");
                }
            }
        }
    }
}