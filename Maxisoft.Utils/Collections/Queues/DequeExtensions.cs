namespace Maxisoft.Utils.Collections.Queues
{
    public static class DequeExtensions
    {
        public static CircularDequeWrapper<T, TDeque> ToCircular<T, TDeque>(this TDeque deque, long capacity)
            where TDeque : IDeque<T>
        {
            return new CircularDequeWrapper<T, TDeque>(deque, capacity);
        }

        public static CircularDeque<T> ToCircular<T>(this Deque<T> deque, long capacity)
        {
            return new CircularDeque<T>(deque, capacity);
        }

        public static BoundedDequeWrapper<T, TDeque> ToBounded<T, TDeque>(this TDeque deque, long capacity)
            where TDeque : IDeque<T>
        {
            return new BoundedDequeWrapper<T, TDeque>(deque, capacity);
        }

        public static BoundedDeque<T> ToBounded<T>(this Deque<T> deque, long capacity)
        {
            return new BoundedDeque<T>(deque, capacity);
        }

        public sealed class CircularDequeWrapper<T, TDeque> : CircularDeque<T, TDeque> where TDeque : IDeque<T>
        {
            internal CircularDequeWrapper(TDeque deque, long capacity) : base(deque, capacity)
            {
            }
        }

        public sealed class BoundedDequeWrapper<T, TDeque> : BoundedDeque<T, TDeque> where TDeque : IDeque<T>
        {
            internal BoundedDequeWrapper(TDeque deque, long capacity) : base(deque, capacity)
            {
            }
        }
    }
}