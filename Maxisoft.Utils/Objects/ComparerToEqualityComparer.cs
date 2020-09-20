using System.Collections.Generic;

namespace Maxisoft.Utils.Objects
{
    public readonly struct ComparerToEqualityComparer<T, TComparer> : IEqualityComparer<T>, IComparer<T>
        where TComparer : IComparer<T>
    {
        public readonly TComparer Comparer;

        public ComparerToEqualityComparer(TComparer comparer)
        {
            Comparer = comparer;
        }

        public bool Equals(T x, T y)
        {
            return Comparer.Compare(x, y) == 0;
        }

        public int GetHashCode(T obj)
        {
            return EqualityComparer<T>.Default.GetHashCode(obj);
        }

        public int Compare(T x, T y)
        {
            return Comparer.Compare(x, y);
        }
    }

    public readonly struct ComparerToEqualityComparer<T> : IEqualityComparer<T>, IComparer<T>
    {
        private readonly ComparerToEqualityComparer<T, IComparer<T>> _implementation;

        public ComparerToEqualityComparer(IComparer<T> comparer) : this(
            new ComparerToEqualityComparer<T, IComparer<T>>(comparer))
        {
        }

        private ComparerToEqualityComparer(ComparerToEqualityComparer<T, IComparer<T>> comparer)
        {
            _implementation = comparer;
        }

        public bool Equals(T x, T y)
        {
            return _implementation.Equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return _implementation.GetHashCode(obj);
        }

        public static implicit operator ComparerToEqualityComparer<T>(Comparer<T> comparer)
        {
            return new ComparerToEqualityComparer<T>(comparer);
        }

        public static readonly ComparerToEqualityComparer<T> Default = Comparer<T>.Default;

        public int Compare(T x, T y)
        {
            return _implementation.Compare(x, y);
        }
    }
}