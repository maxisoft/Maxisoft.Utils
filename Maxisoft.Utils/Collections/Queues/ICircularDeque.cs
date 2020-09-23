namespace Maxisoft.Utils.Collections.Queues
{
    public interface ICircularDeque<T> : IDeque<T>
    {
        bool IsFull { get; }
        long Capacity { get; }
    }
}