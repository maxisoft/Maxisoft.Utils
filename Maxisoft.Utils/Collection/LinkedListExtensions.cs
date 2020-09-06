using System;
using System.Collections.Generic;

namespace Maxisoft.Utils.Collection
{
    public static class LinkedListExtensions
    {
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
    }
}