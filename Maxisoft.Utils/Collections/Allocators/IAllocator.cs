namespace Maxisoft.Utils.Collections.Allocators
{
    public interface IAllocator<T>
    {
        T[] Alloc(ref int capacity);

        void Free(ref T[] array);

        void ReAlloc(ref T[] array, ref int capacity);
    }
}