using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Maxisoft.Utils.Collections.Lists
{
    public partial class ArrayList<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BinarySearch(int index, int count, in T item, IComparer<T>? comparer = null)
        {
            comparer ??= Comparer<T>.Default;
            return GetSlice(index, count).BinarySearch(item, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BinarySearch(in T item, IComparer<T>? comparer = null) => BinarySearch(0, Count, in item, comparer);

        public TList ConvertAll<TOutput, TList>(Converter<T, TOutput> converter) where TList : ArrayList<TOutput>, new()
        {
            var res = new TList();
            res.EnsureCapacity(Count);
            var c = 0;
            foreach (var elem in AsSpan())
            {
                res.Add(converter(elem));
            }

            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArrayList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) =>
            ConvertAll<TOutput, ArrayList<TOutput>>(converter);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            if (Count - index < count) {
                throw new ArgumentOutOfRangeException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            }
            Span<T> target = array;

            AsSpan().Slice(index, count).CopyTo(target.Slice(arrayIndex, count));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(Span<T> span)
        {
            AsSpan().CopyTo(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFindIndex(in Predicate<T> predicate, out int index)
        {
            index = Array.FindIndex(_array, 0, Count, predicate);
            return (uint) index < (uint) Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFindIndex(int startIndex, int count, in Predicate<T> predicate, out int index)
        {
            index = Array.FindIndex(_array, startIndex, count, predicate);
            return (uint) index < (uint) Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Exists(in Predicate<T> predicate)
        {
            return TryFindIndex(in predicate, out _);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Find(in Predicate<T> predicate, in T @default = default)
        {
            return TryFindIndex(in predicate, out var index) ? At(index) : @default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindIndex(int startIndex, int count, in Predicate<T> match)
        {
            TryFindIndex(startIndex, count, in match, out var res);
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindIndex(int startIndex, in Predicate<T> match)
        {
            return FindIndex(startIndex, Count - startIndex, in match);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindIndex(in Predicate<T> match)
        {
            return FindIndex(0, Count, in match);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFindLastIndex(in Predicate<T> predicate, out int index)
        {
            index = Array.FindLastIndex(_array, predicate);
            return (uint) index < (uint) Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryFindLastIndex(int startIndex, int count, in Predicate<T> predicate, out int index)
        {
            index = Array.FindLastIndex(_array, startIndex, count, predicate);
            return (uint) index < (uint) Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T FindLast(in Predicate<T> predicate, in T @default = default)
        {
            return TryFindLastIndex(in predicate, out var index) ? At(index) : @default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindLastIndex(int startIndex, int count, in Predicate<T> match)
        {
            TryFindLastIndex(startIndex, count, in match, out var res);
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindLastIndex(int startIndex, in Predicate<T> match)
        {
            return FindLastIndex(startIndex, startIndex + 1, in match);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindLastIndex(in Predicate<T> match)
        {
            TryFindLastIndex(in match, out var res);
            return res;
        }

        public void ForEach(Action<T> action)
        {
            foreach (var element in AsSpan())
            {
                action(element);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(in T item, int index, int count)
        {
            if (index + count > Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            return Array.IndexOf(_array, item, index, count);
        }

        public void InsertRange<TCollection>(int index, in TCollection c) where TCollection : ICollection<T>
        {
            if ((uint) index > (uint) Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "out of bound");
            }

            var count = c.Count;
            if (count == 0)
            {
                return;
            }

            EnsureCapacity(Count + count);
            if (index < Count)
            {
                Array.Copy(Data(), index, Data(), index + count, Count - index);
            }

            // inserting into itself
            if (ReferenceEquals(this, c))
            {
                Array.Copy(Data(), 0, Data(), index, index);
                Array.Copy(Data(), index + count, Data(), index * 2, Count - index);
            }
            else
            {
                c.CopyTo(Data(), index);
            }

            Count += count;
        }

        public void InsertRange(int index, in IReadOnlyCollection<T> c)
        {
            if ((uint) index > (uint) Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "out of bound");
            }

            var count = c.Count;
            if (count == 0)
            {
                return;
            }

            EnsureCapacity(Count + count);
            if (index < Count)
            {
                Array.Copy(Data(), index, Data(), index + count, Count - index);
            }

            // inserting into itself
            if (ReferenceEquals(this, c))
            {
                Array.Copy(Data(), 0, Data(), index, index);
                Array.Copy(Data(), index + count, Data(), index * 2, Count - index);
            }
            else
            {
                var i = index;
                foreach (var element in c)
                {
                    _array[i++] = element;
                }
            }

            Count += count;
        }

        public void InsertRange(int index, in IEnumerable<T> collection)
        {
            if ((uint) index > (uint) Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "out of bound");
            }

            switch (collection)
            {
                case ICollection<T> c:
                    InsertRange<ICollection<T>>(index, in c);
                    break;
                case IReadOnlyCollection<T> rc:
                    InsertRange(index, in rc);
                    break;
                default:
                {
                    foreach (var element in collection)
                    {
                        Insert(index++, element);
                    }

                    break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> GetSlice(int index, int count)
        {
            return AsSpan().Slice(index, count);
        }

        public TList GetRange<TList>(int index, int count) where TList : ArrayList<T>, new()
        {
            var span = GetSlice(index, count);
            var res = new TList();
            res.Resize(count);
            span.CopyTo(res.Data());
            return res;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArrayList<T> GetRange(int index, int count)
        {
            return GetRange<ArrayList<T>>(index, count);
        }

        public void RemoveRange(int index, int count, bool clear = true)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "negative");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "negative");
            }

            if (Count - index < count)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (count == 0)
            {
                return;
            }

            Count -= count;
            if (index < Count)
            {
                Array.Copy(Data(), index + count, Data(), index, Count - index);
            }

            if (clear)
            {
                Array.Clear(Data(), Count, Count + count);
            }
        }

        public int RemoveAll(in Predicate<T> match, bool clear = true)
        {
            var freeIndex = 0;
            
            foreach (var element in AsSpan())
            {
                if (match(element))
                {
                    break;
                }

                freeIndex++;
            }

            if (freeIndex >= Count) return 0;

            var current = freeIndex + 1;
            while (current < Count)
            {
                var span = AsSpan().Slice(current);
                foreach (var element in span)
                {
                    if (match(element))
                    {
                        break;
                    }

                    current++;
                }

                if (current < Count)
                {
                    At(freeIndex++) = At(current++);
                }
            }

            if (clear)
            {
                Array.Clear(_array, freeIndex, Count - freeIndex);
            }

            var res = Count - freeIndex;
            Count = freeIndex;
            return res;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reverse() {
            Reverse(0, Count);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reverse(int index, int count) {
            GetSlice(index, count).Reverse();
        }
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sort(IComparer<T>? comparer = null)
        {
            Sort(0, Count, comparer);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sort(int index, int count, IComparer<T>? comparer) {
            if (index + count > Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            comparer ??= Comparer<T>.Default;
            Array.Sort(_array, index, count, comparer);
        }
    }
}