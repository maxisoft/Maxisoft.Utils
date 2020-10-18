using System;
using System.Collections.Generic;

namespace Maxisoft.Utils.Collections.Lists
{
    public static class ListExtensions
    {
        public static ref T At<T>(this T[] array, WrappedIndex index)
        {
            return ref array[index.Resolve(array)];
        }

        public static T At<T, TList>(TList list, WrappedIndex index) where TList : IList<T>
        {
            return list[index.Resolve<T, TList>(list)];
        }

        public static T At<T>(this IList<T> list, WrappedIndex index)
        {
            return At<T, IList<T>>(list, index);
        }

        public static ArrayListWrapper<T> WrapAsList<T>(this T[] array)
        {
            return new ArrayListWrapper<T>(array);
        }

        public static bool TryPop<T, TList>(TList list, int index, out T value) where TList : IList<T>
        {
            if ((uint) index >= (uint) list.Count)
            {
                value = default!;
                return false;
            }

            value = list[index];
            list.RemoveAt(index);
            return true;
        }

        public static bool TryPop<T>(this IList<T> list, int index, out T value)
        {
            return TryPop<T, IList<T>>(list, index, out value);
        }

        public static bool TryPop<T>(this IList<T> list, out T value)
        {
            return TryPop<T, IList<T>>(list, list.Count - 1, out value);
        }

        public static T Pop<T>(this IList<T> list, int index)
        {
            if (TryPop<T, IList<T>>(list, index, out var value))
            {
                return value;
            }

            throw new ArgumentOutOfRangeException(nameof(index), index, null);
        }

        public static T Pop<T>(this IList<T> list)
        {
            if (TryPop<T, IList<T>>(list, list.Count - 1, out var value))
            {
                return value;
            }

            throw new ArgumentOutOfRangeException(nameof(list), list, "list is empty");
        }

        public static int AddSorted<T>(this List<T> list, in T item, IComparer<T>? comparer = null)
        {
            var index = comparer is null ? list.BinarySearch(item) : list.BinarySearch(item, comparer);
            var res = index < 0 ? ~index : index;
            list.Insert(res, item);
            return res;
        }

        public static int AddSorted<T>(this ArrayList<T> list, in T item, IComparer<T>? comparer = null)
        {
            var index = list.BinarySearch(in item, comparer);
            var res = index < 0 ? ~index : index;
            list.Insert(res, item);
            return res;
        }
    }
}