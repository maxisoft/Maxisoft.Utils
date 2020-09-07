using System;
using System.Collections;
using System.Collections.Generic;

namespace Maxisoft.Utils.Empty
{
    public readonly struct NoOpDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IEmpty
    {
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new EmptyEnumerator<KeyValuePair<TKey, TValue>>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
        }

        public void Clear()
        {
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return false;
        }

        public int Count => 0;

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
        }

        public bool ContainsKey(TKey key)
        {
            return false;
        }

        public bool Remove(TKey key)
        {
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default!;
            return false;
        }

        public TValue this[TKey key]
        {
            get => default!;
            set { }
        }

        public ICollection<TKey> Keys => new EmptyCollection<TKey>();

        public ICollection<TValue> Values => new EmptyCollection<TValue>();
    }

    public readonly struct NoOpDictionary : IDictionary, IEmpty
    {
        public void Add(object key, object value)
        {
        }

        public void Clear()
        {
        }

        public bool Contains(object key)
        {
            return false;
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return new EmptyDictionaryEnumerator();
        }

        public void Remove(object key)
        {
        }

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        public object this[object key]
        {
            get => default!;
            set { }
        }

        public ICollection Keys => new EmptyCollection();

        public ICollection Values => new EmptyCollection();

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
    }
}