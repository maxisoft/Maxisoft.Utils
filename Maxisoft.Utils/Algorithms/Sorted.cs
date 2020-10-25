using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Maxisoft.Utils.Collections.Lists;

namespace Maxisoft.Utils.Algorithms
{
    public static class Sorted
    {
        public static bool IsSorted<T>(this IEnumerable<T> enumerable)
        {
            return IsSorted(enumerable, Comparer<T>.Default);
        }

        public static bool IsSorted<T, TComparer>(this IEnumerable<T> enumerable, TComparer comparer)
            where TComparer : IComparer<T>
        {
            using var enumerator = enumerable.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return true; // empty is sorted
            }

            var prev = enumerator.Current;
            while (enumerator.MoveNext())
            {
                var value = enumerator.Current;
                if (comparer.Compare(prev, value) > 0)
                {
                    return false;
                }

                prev = value;
            }

            return true;
        }


        internal static bool IsSorted<T, TList, TComparer>(TList list, TComparer comparer)
            where TList : IList<T>
            where TComparer : IComparer<T>
        {
            if (list.Count <= 1)
            {
                return true;
            }

            var prev = list[0];

            foreach (var value in list)
            {
                if (comparer.Compare(prev, value) > 0)
                {
                    return false;
                }

                prev = value;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSorted<T, TComparer>(this List<T> list, TComparer comparer)
            where TComparer : IComparer<T>
        {
            return IsSorted<T, List<T>, TComparer>(list, comparer);
        }

        internal static bool IsSorted<T, TList>(TList list)
            where TList : IList<T>
            where T : IComparable<T>
        {
            if (list.Count <= 1)
            {
                return true;
            }

            var prev = list[0];

            foreach (var value in list)
            {
                if (prev.CompareTo(value) > 0)
                {
                    return false;
                }

                prev = value;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSorted<T>(this List<T> list)
            where T : IComparable<T>
        {
            return IsSorted<T, List<T>>(list);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSorted<T>(this ArrayList<T> list)
            where T : IComparable<T>
        {
            return IsSorted(list.AsSpan());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSorted<T, TComparer>(this ArrayList<T> list, TComparer comparer)
            where TComparer : IComparer<T>
        {
            return IsSorted(list.AsSpan(), comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSorted<T>(this Span<T> span)
            where T : IComparable<T>
        {
            return IsSorted((ReadOnlySpan<T>) span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSorted<T, TComparer>(this Span<T> span, TComparer comparer)
            where TComparer : IComparer<T>
        {
            return IsSorted((ReadOnlySpan<T>) span, comparer);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSorted<T>(this T[] arr)
            where T : IComparable<T>
        {
            return IsSorted((ReadOnlySpan<T>) arr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSorted<T, TComparer>(this T[] arr, TComparer comparer)
            where TComparer : IComparer<T>
        {
            return IsSorted((ReadOnlySpan<T>) arr, comparer);
        }

        public static bool IsSorted<T>(this ReadOnlySpan<T> span)
            where T : IComparable<T>
        {
            if (span.Length <= 1)
            {
                return true;
            }

            var prev = span[0];
            foreach (var value in span.Slice(1))
            {
                if (prev.CompareTo(value) > 0)
                {
                    return false;
                }

                prev = value;
            }

            return true;
        }

        public static bool IsSorted<T, TComparer>(this ReadOnlySpan<T> span, TComparer comparer)
            where TComparer : IComparer<T>
        {
            if (span.Length <= 1)
            {
                return true;
            }

            var prev = span[0];
            foreach (var value in span.Slice(1))
            {
                if (comparer.Compare(prev, value) > 0)
                {
                    return false;
                }

                prev = value;
            }

            return true;
        }
    }
}