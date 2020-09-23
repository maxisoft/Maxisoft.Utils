namespace Maxisoft.Utils.Collections.Queue
{
    public interface ICircularDeque<T> : IDeque<T>
    {
        bool IsFull { get; }
        long Capacity { get; }
    }
}