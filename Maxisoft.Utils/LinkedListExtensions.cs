using System;
using System.Collections.Generic;

namespace Maxisoft.Utils
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
        
        public static IEnumerable<T> Reverse<T>(this LinkedList<T> list) {
            var el = list.Last;
            while (el != null) {
                yield return el.Value;
                el = el.Previous;
            }
        }
    }
}