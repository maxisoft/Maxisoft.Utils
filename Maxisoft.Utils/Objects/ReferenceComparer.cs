using System.Collections.Generic;

namespace Maxisoft.Utils.Objects
{
    public readonly struct ReferenceComparer<T> : IComparer<T> where T : class
    {
        public int Compare(T x, T y)
        {
            return PointerHelper.GetPointer(x).ToInt64().CompareTo(PointerHelper.GetPointer(y).ToInt64());
        }
    }
}