using System;

namespace Maxisoft.Utils.Collections.Lists
{
    public class ArrayListWrapper<T> : ArrayList<T>
    {
        public ArrayListWrapper()
        {
        }

        public ArrayListWrapper(T[] array) : base(array, array.Length)
        {
        }

        public ArrayListWrapper(T[] array, int size) : base(array, size)
        {
        }

        protected internal override T[] Alloc(int size)
        {
            if (size != Capacity)
            {
                throw new InvalidOperationException("Cannot allocate new array");
            }

            return Data();
        }

        protected internal override void Free(T[] array)
        {
        }

        protected internal override int ComputeGrowSize(int size, int capacity)
        {
            return capacity;
        }

        protected internal override void ReAlloc(ref T[] array, int actualSize, int capacity)
        {
            if (capacity != Capacity)
            {
                throw new InvalidOperationException("Cannot reallocate new array");
            }
        }
    }
}