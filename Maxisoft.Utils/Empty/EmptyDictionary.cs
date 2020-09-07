using System;
using System.Collections;
using System.Collections.Generic;

namespace Maxisoft.Utils.Empty
{
    public readonly struct EmptyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IEmpty
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
            throw new InvalidOperationException("This dictionary must remain empty by design");
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
            throw new InvalidOperationException("This dictionary must remain empty by design");
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
            throw new InvalidOperationException("This dictionary is empty by design");
        }

        public TValue this[TKey key]
        {
            get => throw new InvalidOperationException("This dictionary is empty by design");
            set => throw new InvalidOperationException("This dictionary is empty by design");
        }

        public ICollection<TKey> Keys => new EmptyCollection<TKey>();

        public ICollection<TValue> Values => new EmptyCollection<TValue>();
    }

    public readonly struct EmptyDictionary : IDictionary, IEmpty
    {
        public void Add(object key, object value)
        {
            throw new InvalidOperationException("This dictionary must remain empty by design");
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
            get => throw new InvalidOperationException("This dictionary is empty by design");
            set => throw new InvalidOperationException("This dictionary is empty by design");
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