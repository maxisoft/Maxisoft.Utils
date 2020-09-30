using System.Collections.Generic;

namespace Maxisoft.Utils.Objects
{
    public readonly struct ReverseComparer<T, TComparer> : IComparer<T> where TComparer: IComparer<T>
    {
        private readonly TComparer _comparer;

        public ReverseComparer(in TComparer comparer)
        {
            _comparer = comparer;
        }

        public int Compare(T x, T y)
        {
            return _comparer.Compare(y, x);
        }
    }

    public readonly struct ReverseComparer<T> : IComparer<T>
    {
        private readonly ReverseComparer<T, IComparer<T>> _comparer;
        
        public static readonly ReverseComparer<T, Comparer<T>> Default = new ReverseComparer<T, Comparer<T>>(Comparer<T>.Default);

        public ReverseComparer(IComparer<T> comparer)
        {
            _comparer = new ReverseComparer<T, IComparer<T>>(in comparer);
        }

        public int Compare(T x, T y)
        {
            return _comparer.Compare(x, y);
        }
    }
}