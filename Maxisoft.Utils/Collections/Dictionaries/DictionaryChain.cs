using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Maxisoft.Utils.Empty;

namespace Maxisoft.Utils.Collections.Dictionaries
{
    public abstract class DictionaryChain<TKey, TValue, TDictionary> : IDictionary<TKey, TValue>
        where TDictionary : class, IDictionary<TKey, TValue>
    {
        private readonly IEqualityComparer<TKey> _equalityComparer;
        internal readonly TDictionary[] Dictionaries;
        internal readonly TDictionary Modifiable;

        public DictionaryChain(TDictionary[] dictionaries, TDictionary modifiable,
            IEqualityComparer<TKey>? equalityComparer = null)
        {
            if (Array.IndexOf(dictionaries, modifiable) != 0)
            {
                throw new ArgumentException("modifiable is not the 1st dictionary", nameof(modifiable));
            }

            Dictionaries = dictionaries;
            Modifiable = modifiable;
            _equalityComparer = equalityComparer ?? EqualityComparer<TKey>.Default;
        }

        public DictionaryChain(IEnumerable<TDictionary> dictionaries, TDictionary modifiable,
            IEqualityComparer<TKey>? equalityComparer = null) : this(dictionaries.ToArray(), modifiable,
            equalityComparer)
        {
        }


        public DictionaryChain(params TDictionary[] dictionaries) : this(dictionaries, dictionaries[0])
        {
        }

        public DictionaryChain(TDictionary[] dictionaries, IEqualityComparer<TKey>? equalityComparer = null) : this(
            dictionaries, dictionaries[0], equalityComparer)
        {
        }

        public DictionaryChain(IEnumerable<TDictionary> dictionaries, IEqualityComparer<TKey>? equalityComparer = null)
            : this(dictionaries.ToArray(), equalityComparer)
        {
        }

        public ReadOnlyMemory<TDictionary> DictionariesChain => new ReadOnlyMemory<TDictionary>(Dictionaries);

        public int CountAll => Dictionaries.Sum(dictionary => dictionary.Count);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var hs = new HashSet<TKey>(_equalityComparer);
            return Dictionaries.SelectMany(dict => dict.Where(pair => hs.Add(pair.Key))).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Modifiable.Add(item);
        }

        public void Clear()
        {
            if (Dictionaries.Where(dict => !ReferenceEquals(Modifiable, dict))
                .Any(dictionary => dictionary.Count > 0))
            {
                throw new InvalidOperationException("cannot clear non modifiable dictionaries");
            }

            Modifiable.Clear();
        }


        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            var hs = new HashSet<TKey>(_equalityComparer);
            return Dictionaries.SelectMany(dict => dict.Where(pair => hs.Add(pair.Key))).Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "Out of bounds");
            }

            var count = Count;
            if (array.Length - arrayIndex < count)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "Out of bounds");
            }

            var c = arrayIndex;
            var hs = new HashSet<TKey>(_equalityComparer);
            foreach (var dictionary in Dictionaries)
            {
                foreach (var pair in dictionary)
                {
                    if (hs.Add(pair.Key))
                    {
                        array[c++] = pair;
                    }
                }
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Modifiable.Remove(item);
        }

        public int Count => CountElements();

        public bool IsReadOnly => Modifiable.IsReadOnly;

        public void Add(TKey key, TValue value)
        {
            Modifiable.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return Dictionaries.Any(dictionary => dictionary.ContainsKey(key));
        }

        public bool Remove(TKey key)
        {
            return Modifiable.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            foreach (var dictionary in Dictionaries)
            {
                if (dictionary.TryGetValue(key, out value))
                {
                    return true;
                }
            }

            value = default!;
            return false;
        }

        public TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out var result))
                {
                    return result;
                }

                throw new KeyNotFoundException();
            }
            set => Modifiable[key] = value;
        }

        public ICollection<TKey> Keys => new KeyCollection(this);

        public ICollection<TValue> Values => new ValuesCollection(this);

        private int CountElements()
        {
            var hs = new HashSet<TKey>(_equalityComparer);
            return Dictionaries.Sum(dictionary => dictionary.Count(pair => hs.Add(pair.Key)));
        }

        public void AddAll(in KeyValuePair<TKey, TValue> item)
        {
            foreach (var dictionary in Dictionaries)
            {
                dictionary.Add(item);
            }
        }

        public void UpdateAll(in TKey key, in TValue value, bool upsert = true)
        {
            foreach (var dictionary in Dictionaries)
            {
                if (upsert || dictionary.ContainsKey(key))
                {
                    dictionary[key] = value;
                }
            }
        }

        public void ClearAll()
        {
            foreach (var dictionary in Dictionaries)
            {
                dictionary.Clear();
            }
        }

        public bool ContainsAny(KeyValuePair<TKey, TValue> item)
        {
            return Dictionaries.Any(dictionary => dictionary.Contains(item));
        }

        public int RemoveAll(KeyValuePair<TKey, TValue> item)
        {
            return Dictionaries.Sum(dictionary => dictionary.Remove(item) ? 1 : 0);
        }

        public void AddAll(in TKey key, in TValue value)
        {
            foreach (var dictionary in Dictionaries)
            {
                dictionary.Add(key, value);
            }
        }

        public int RemoveAll(TKey key)
        {
            return Dictionaries.Sum(dictionary => dictionary.Remove(key) ? 1 : 0);
        }


#pragma warning disable 693
        public TDictionary BuildFlatDictionary<TDictionary>() where TDictionary : IDictionary<TKey, TValue>, new()
#pragma warning restore 693
        {
            var res = new TDictionary();
            foreach (var pair in this)
            {
                res.Add(pair);
            }

            return res;
        }

        private sealed class KeyCollection : ICollection<TKey>
        {
            private readonly DictionaryChain<TKey, TValue, TDictionary> _dictionary;

            internal KeyCollection(DictionaryChain<TKey, TValue, TDictionary> dictionary)
            {
                _dictionary = dictionary;
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                var hs = new HashSet<TKey>(_dictionary._equalityComparer);
                return _dictionary.Dictionaries.SelectMany(dict => dict.Keys.Where(key => hs.Add(key))).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(TKey item)
            {
                throw new InvalidOperationException("readonly");
            }

            public void Clear()
            {
                throw new InvalidOperationException("readonly");
            }

            public bool Contains(TKey item)
            {
                return _dictionary.ContainsKey(item);
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                if (arrayIndex < 0 || arrayIndex > array.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "Out of bounds");
                }

                if (array.Length - arrayIndex < _dictionary.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "Out of bounds");
                }

                var c = arrayIndex;
                foreach (var pair in _dictionary)
                {
                    array[c++] = pair.Key;
                }
            }

            public bool Remove(TKey item)
            {
                throw new InvalidOperationException("readonly");
            }

            public int Count => _dictionary.Count;

            public bool IsReadOnly => true;
        }

        private sealed class ValuesCollection : ICollection<TValue>
        {
            private readonly DictionaryChain<TKey, TValue, TDictionary> _dictionary;

            internal ValuesCollection(DictionaryChain<TKey, TValue, TDictionary> dictionary)
            {
                _dictionary = dictionary;
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                var hs = new HashSet<TKey>(_dictionary._equalityComparer);
                return _dictionary.Dictionaries
                    .SelectMany(dict => dict.Where(pair => hs.Add(pair.Key)).Select(pair => pair.Value))
                    .GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(TValue item)
            {
                throw new InvalidOperationException("readonly");
            }

            public void Clear()
            {
                throw new InvalidOperationException("readonly");
            }

            public bool Contains(TValue item)
            {
                var hs = new HashSet<TKey>(_dictionary._equalityComparer);
                return _dictionary.Dictionaries
                    .SelectMany(dict => dict.Where(pair => hs.Add(pair.Key)).Select(pair => pair.Value))
                    .Contains(item);
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                if (arrayIndex < 0 || arrayIndex > array.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "Out of bounds");
                }

                if (array.Length - arrayIndex < _dictionary.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "Out of bounds");
                }

                var c = arrayIndex;
                foreach (var pair in _dictionary)
                {
                    array[c++] = pair.Value;
                }
            }

            public bool Remove(TValue item)
            {
                throw new InvalidOperationException("readonly");
            }

            public int Count => _dictionary.Count;

            public bool IsReadOnly => true;
        }
    }

    public class DictionaryChain<TKey, TValue> : DictionaryChain<TKey, TValue, IDictionary<TKey, TValue>>
    {
        public DictionaryChain(IDictionary<TKey, TValue>[] dictionaries, IDictionary<TKey, TValue> modifiable,
            IEqualityComparer<TKey>? equalityComparer = null) : base(dictionaries, modifiable, equalityComparer)
        {
        }

        public DictionaryChain(IEnumerable<IDictionary<TKey, TValue>> dictionaries,
            IDictionary<TKey, TValue> modifiable, IEqualityComparer<TKey>? equalityComparer = null) : base(dictionaries,
            modifiable, equalityComparer)
        {
        }

        public DictionaryChain() : base(new EmptyDictionary<TKey, TValue>())
        {
            throw new InvalidOperationException("");
        }

        public DictionaryChain(params IDictionary<TKey, TValue>[] dictionaries) : base(dictionaries)
        {
        }

        public DictionaryChain(IDictionary<TKey, TValue>[] dictionaries,
            IEqualityComparer<TKey>? equalityComparer = null) : base(dictionaries, equalityComparer)
        {
        }

        public DictionaryChain(IEnumerable<IDictionary<TKey, TValue>> dictionaries,
            IEqualityComparer<TKey>? equalityComparer = null) : base(dictionaries, equalityComparer)
        {
        }
    }
}