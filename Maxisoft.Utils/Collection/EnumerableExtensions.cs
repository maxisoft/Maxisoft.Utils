using System.Collections.Generic;

namespace Maxisoft.Utils.Collection
{
    public static class EnumerableExtensions
    {
        public static Deque<T> ToDeque<T>(this IEnumerable<T> enumerable)
        {
            var res = new Deque<T>();
            foreach (var element in enumerable)
            {
                res.PushBack(element);
            }

            return res;
        }
        
        public static LinkedList<T> ToLinkedList<T>(this IEnumerable<T> enumerable)
        {
            var res = new LinkedList<T>();
            foreach (var element in enumerable)
            {
                res.AddLast(element);
            }

            return res;
        }
    }
}