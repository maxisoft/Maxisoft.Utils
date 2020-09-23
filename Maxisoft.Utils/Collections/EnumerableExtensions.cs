﻿using System.Collections.Generic;
using Maxisoft.Utils.Collection.Queue;

namespace Maxisoft.Utils.Collection
{
    public static class EnumerableExtensions
    {
        public static Deque<T> ToDeque<T>(this IEnumerable<T> enumerable)
        {
            return ToDeque<T, Deque<T>>(enumerable);
        }
        
        public static TDeque ToDeque<T, TDeque>(IEnumerable<T> enumerable) where TDeque: IDeque<T>, new()
        {
            var res = new TDeque();
            foreach (var element in enumerable)
            {
                res.PushBack(element);
            }

            return res;
        }
        
        public static LinkedList<T> ToLinkedList<T>(this IEnumerable<T> enumerable)
        {
            return ToLinkedList<T, LinkedList<T>>(enumerable);
        }

        public static TLinkedList ToLinkedList<T, TLinkedList>(IEnumerable<T> enumerable) where TLinkedList: LinkedList<T>, new()
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