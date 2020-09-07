using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Maxisoft.Utils.Empty
{
    /// <summary>
    ///     Empty collection preventing any modification
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="NoOpCollection{T}" />
    public readonly struct EmptyCollection<T> : ICollection<T>, IEmpty
    {
        public IEnumerator<T> GetEnumerator()
        {
            return new EmptyEnumerator<T>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Instantly throws <c>InvalidOperationException</c>
        /// </summary>
        /// <param name="item">Ignored</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Add(T item)
        {
            Enumerable.Empty<T>();
            throw new InvalidOperationException("This collection must remain empty by design");
        }

        public void Clear()
        {
        }

        public bool Contains(T item)
        {
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
        }

        public bool Remove(T item)
        {
            return false;
        }

        public int Count => 0;

        public bool IsReadOnly => false;
    }
}