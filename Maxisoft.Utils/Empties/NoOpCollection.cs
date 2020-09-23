using System;
using System.Collections;
using System.Collections.Generic;

namespace Maxisoft.Utils.Empties
{
    /// <summary>
    ///     Empty collection silently ignoring any modifications
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="EmptyCollection{T}" />
    public readonly struct NoOpCollection<T> : ICollection<T>, IEmpty
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