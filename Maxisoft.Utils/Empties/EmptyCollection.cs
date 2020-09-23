using System;
using System.Collections;
using System.Collections.Generic;

namespace Maxisoft.Utils.Empties
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

    public readonly struct EmptyCollection : ICollection, IEmpty
    {
        public IEnumerator GetEnumerator()
        {
            return new EmptyEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
        }

        public int Count => 0;

        public bool IsSynchronized => false;

        public object SyncRoot => this;
    }
}