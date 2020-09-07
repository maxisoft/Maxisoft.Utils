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

    /// <summary>
    ///     Empty collection silently ignoring any modifications
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="EmptyCollection{T}" />
    public readonly struct NoOpCollection<T> : ICollection<T>, IEmpty
    {
#pragma warning disable 649
        private readonly EmptyCollection<T> _impl;
#pragma warning restore 649

        public IEnumerator<T> GetEnumerator()
        {
            return _impl.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _impl).GetEnumerator();
        }

        /// <summary>
        ///     Doing nothing silently
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
        }

        public void Clear()
        {
            _impl.Clear();
        }

        public bool Contains(T item)
        {
            return _impl.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _impl.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _impl.Remove(item);
        }

        public int Count => _impl.Count;

        public bool IsReadOnly => _impl.IsReadOnly;
    }
}