using System;
using System.Runtime.CompilerServices;

namespace Maxisoft.Utils.Collections.Lists
{
    public abstract partial class ArrayList<T, TAllocator>
    {
        public void Resize(int size, bool clear = true)
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, "negative");
            }

            var prevSize = Count;
            EnsureCapacity(size);
            Count = size;
            if (clear && size < prevSize)
            {
                Array.Clear(_array, size, prevSize - size);
            }
        }

        public void Move(int fromIndex, int toIndex)
        {
            CheckForOutOfBounds(fromIndex, nameof(fromIndex));
            CheckForOutOfBounds(toIndex, nameof(toIndex));
            if (fromIndex == toIndex)
            {
                return;
            }

            var tmp = this[fromIndex];
            if (fromIndex < toIndex)
            {
                GetSlice(fromIndex + 1, toIndex - fromIndex).CopyTo(GetSlice(fromIndex, toIndex - fromIndex));
            }
            else
            {
                GetSlice(toIndex, fromIndex - toIndex).CopyTo(GetSlice(toIndex + 1, fromIndex - toIndex));
            }

            this[toIndex] = tmp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Swap(int left, int right)
        {
            var tmp = this[left];
            this[left] = this[right];
            this[right] = tmp;
        }
    }
}