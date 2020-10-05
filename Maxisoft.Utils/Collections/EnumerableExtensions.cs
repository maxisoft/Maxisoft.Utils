using System.Collections.Generic;
using Maxisoft.Utils.Collections.LinkedLists;
using Maxisoft.Utils.Collections.Lists;
using Maxisoft.Utils.Collections.Queues;

namespace Maxisoft.Utils.Collections
{
    public static class EnumerableExtensions
    {
        public static Deque<T> ToDeque<T>(this IEnumerable<T> enumerable)
        {
            return ToDeque<T, Deque<T>>(enumerable);
        }

        public static TDeque ToDeque<T, TDeque>(IEnumerable<T> enumerable) where TDeque : IDeque<T>, new()
        {
            var res = new TDeque();
            foreach (var element in enumerable)
            {
                res.PushBack(element);
            }

            return res;
        }

        public static LinkedListAsIList<T> ToLinkedList<T>(this IEnumerable<T> enumerable)
        {
            return ToLinkedList<T, LinkedListAsIList<T>>(enumerable);
        }

        public static ArrayList<T> ToArrayList<T>(this IEnumerable<T> enumerable)
        {
            switch (enumerable)
            {
                case ICollection<T> coll:
                    return ToArrayList<T>(coll);
                case IReadOnlyCollection<T> rcoll:
                    return ToArrayList(rcoll);
            }

            var res = new ArrayList<T>();
            foreach (var element in enumerable)
            {
                res.Add(element);
            }

            return res;
        }

        public static ArrayList<T> ToArrayList<T>(ICollection<T> collection)
        {
            var res = new ArrayList<T>(collection.Count);
            foreach (var element in collection)
            {
                res.Add(element);
            }

            return res;
        }

        public static ArrayList<T> ToArrayList<T>(IReadOnlyCollection<T> collection)
        {
            var res = new ArrayList<T>(collection.Count);
            foreach (var element in collection)
            {
                res.Add(element);
            }

            return res;
        }

        public static ArrayList<T> ToArrayList<T>(this T[] array, bool copy = true)
        {
            if (!copy)
            {
                return new ArrayList<T>(array);
            }

            var res = new ArrayList<T>(array.Length);
            foreach (var element in array)
            {
                res.Add(element);
            }

            return res;
        }

        public static TLinkedList ToLinkedList<T, TLinkedList>(IEnumerable<T> enumerable)
            where TLinkedList : LinkedList<T>, new()
        {
            var res = new TLinkedList();
            foreach (var element in enumerable)
            {
                res.AddLast(element);
            }

            return res;
        }
    }
}