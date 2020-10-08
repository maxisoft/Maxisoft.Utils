using System;
using System.Collections;
using System.Collections.Generic;

namespace Maxisoft.Utils.Empties
{
    public readonly struct EmptyList<T> : IList<T>, IEmpty
    {
        
        public EmptyEnumerator<T> GetEnumerator()
        {
            return new EmptyEnumerator<T>();
        }
        
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            throw new InvalidOperationException("This list must remain empty by design");
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

        public int IndexOf(T item)
        {
            return -1;
        }

        public void Insert(int index, T item)
        {
            throw new InvalidOperationException("This list must remain empty by design");
        }

        public void RemoveAt(int index)
        {
        }

        public T this[int index]
        {
            get => throw new InvalidOperationException("This list is empty by design");
            set => throw new InvalidOperationException("This list is empty by design");
        }
    }

    public readonly struct EmptyList : IList, IEmpty
    {
        
        // ReSharper disable once MemberCanBeMadeStatic.Global
        public EmptyEnumerator GetEnumerator()
        {
            return new EmptyEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
        }

        public int Count => 0;

        public bool IsSynchronized => false;

        public object SyncRoot => this;

        public int Add(object value)
        {
            throw new InvalidOperationException("This list must remain empty by design");
        }

        public void Clear()
        {
        }

        public bool Contains(object value)
        {
            return false;
        }

        public int IndexOf(object value)
        {
            return -1;
        }

        public void Insert(int index, object value)
        {
            throw new InvalidOperationException("This list must remain empty by design");
        }

        public void Remove(object value)
        {
        }

        public void RemoveAt(int index)
        {
        }

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        public object this[int index]
        {
            get => throw new InvalidOperationException("This list is empty by design");
            set => throw new InvalidOperationException("This list is empty by design");
        }
    }
}