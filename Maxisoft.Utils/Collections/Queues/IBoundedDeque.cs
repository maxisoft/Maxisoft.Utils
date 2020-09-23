namespace Maxisoft.Utils.Collections.Queues
{
    public interface IBoundedDeque<T> : IDeque<T>
    {
        bool IsFull { get; }
        long CappedSize { get; }
        bool TryPushBack(in T element);
        bool TryPushFront(in T element);
    }
}