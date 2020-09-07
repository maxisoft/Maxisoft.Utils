using System.Collections;
using System.Collections.Generic;

namespace Maxisoft.Utils.Empty
{
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