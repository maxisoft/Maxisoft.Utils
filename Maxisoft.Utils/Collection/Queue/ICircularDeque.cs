namespace Maxisoft.Utils.Collection.Queue
{
    public interface ICircularDeque<T> : IDeque<T>
    {
        bool IsFull { get; }
        long Capacity { get; }
    }
}