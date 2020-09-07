using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Maxisoft.Utils.Empty
{
    public struct EmptyEnumerable : IEnumerable, IEmpty
    {
        public IEnumerator GetEnumerator()
        {
            return new EmptyEnumerator();
        }
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Enumerable.Empty{TResult}" />
    public struct EmptyEnumerable<T> : IEnumerable<T>, IEmpty
    {
        public IEnumerator<T> GetEnumerator()
        {
            return new EmptyEnumerator<T>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}