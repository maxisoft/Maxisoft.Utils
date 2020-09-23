using System;
using System.Collections;
using System.Collections.Generic;

namespace Maxisoft.Utils.Empties
{
    public readonly struct NoOpList<T> : IList<T>, IEmpty
    {
        public IEnumerator<T> GetEnumerator()
        {
            return new EmptyEnumerator<T>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

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

        public int IndexOf(T item)
        {
            return -1;
        }

        public void Insert(int index, T item)
        {
        }

        public void RemoveAt(int index)
        {
        }

        public T this[int index]
        {
            get => default!;
            set { }
        }
    }

    public readonly struct NoOpList : IList, IEmpty
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

        public int Add(object value)
        {
            return 0;
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
            get => default!;
            set { }
        }
    }
}