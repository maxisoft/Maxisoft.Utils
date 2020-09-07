using System;
using System.Collections.Generic;

namespace Maxisoft.Utils.Collection
{
    public static class LinkedListExtensions
    {
        public static LinkedList<int> Range(int size) => Range(0, size);
        
        public static LinkedList<int> Range(int start, int size, int step = 1)
        {
            var l = new LinkedList<int>();
            for (var i = start; i < start + size; i+= step)
            {
                l.AddLast(i);
            }

            return l;
        }
        
        public static void RemoveAll<T>(this LinkedList<T> linkedList,
            Func<T, bool> predicate)
        {
            for (var node = linkedList.First; node != null;)
            {
                var next = node.Next;
                if (predicate(node.Value))
                {
                    linkedList.Remove(node);
                }

                node = next;
            }
        }

        public static void RemoveAll<T>(this LinkedList<T> linkedList,
            Func<LinkedListNode<T>, bool> predicate)
        {
            for (var node = linkedList.First; node != null;)
            {
                var next = node.Next;
                if (predicate(node))
                {
                    linkedList.Remove(node);
                }

                node = next;
            }
        }

        public static IEnumerable<T> ReversedIterator<T>(this LinkedList<T> list)
        {
            var el = list.Last;
            while (el != null)
            {
                yield return el.Value;
                el = el.Previous;
            }
        }

        public static IEnumerable<LinkedListNode<T>> ReversedNodeIterator<T>(this LinkedList<T> list)
        {
            var el = list.Last;
            while (el != null)
            {
                yield return el;
                el = el.Previous;
            }
        }

        public static LinkedListNode<T> At<T>(this LinkedList<T> list, int index)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), index, "negative index");
            if (index >= list.Count)
                throw new ArgumentOutOfRangeException(nameof(index), index,
                    $"list contains only {list.Count} elements");
            return index > (list.Count - 1) / 2 ? AtRight(list, index) : AtLeft(list, index);
        }

        private static LinkedListNode<T> AtLeft<T>(LinkedList<T> list, int index)
        {
            var el = list.First;
            var c = 0;
            while (el != null)
            {
                if (c++ == index)
                {
                    return el;
                }

                el = el.Next;
            }

            throw new InvalidOperationException();
        }

        private static LinkedListNode<T> AtRight<T>(LinkedList<T> list, int index)
        {
            var el = list.Last;
            var c = list.Count;
            while (el != null)
            {
                if (--c == index)
                {
                    return el;
                }

                el = el.Previous;
            }

            throw new InvalidOperationException();
        }


        public static LinkedListNode<T> Insert<T>(this LinkedList<T> list, int index, in T value)
        {
            if (index == 0)
            {
                return list.AddFirst(value);
            }

            if (index == list.Count)
            {
                return list.AddLast(value);
            }

            var node = At(list, index);
            return list.AddBefore(node, value);
        }

        public static LinkedListNode<T> RemoveAt<T>(this LinkedList<T> list, int index)
        {
            var node = At(list, index);
            list.Remove(node);
            return node;
        }

        public static int IndexOf<T>(this LinkedList<T> list, in T value)
        {
            var el = list.First;
            var c = 0;
            var comparer = EqualityComparer<T>.Default;
            while (el != null)
            {
                if (comparer.Equals(value, el.Value))
                {
                    return c;
                }

                c += 1;
                el = el.Next;
            }
            return -1;
        }
    }
}