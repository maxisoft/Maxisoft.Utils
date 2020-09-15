﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Maxisoft.Utils.Collection.UpdateGuard;

namespace Maxisoft.Utils.Collection.Dictionary
{
    public abstract class BaseOrderedDictionary<TKey, TValue, TList, TDictionary> : IOrderedDictionary<TKey, TValue>
        where TList : IList<TKey>, new() where TDictionary : class, IDictionary<TKey, TValue>, new()
    {
        private readonly UpdateGuardedContainer _version = new UpdateGuardedContainer();

        public BaseOrderedDictionary()
        {
        }

        public BaseOrderedDictionary(in TDictionary initial)
        {
            Dictionary = initial;
            foreach (var value in initial)
            {
                Indexes.Add(value.Key);
            }
        }

        protected TDictionary Dictionary { get; } = new TDictionary();
        protected TList Indexes { get; } = new TList();


        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            using var _ = _version.CreateGuard();
            foreach (var key in Indexes)
            {
                var res = Dictionary.TryGetValue(key, out var value);
                Debug.Assert(res);
                yield return new KeyValuePair<TKey, TValue>(key, value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            DoAdd(item.Key, item.Value);
        }

        public void Clear()
        {
            using var _ = _version.CreateGuard(true);
            Indexes.Clear();
            Dictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            var comparer = EqualityComparer<TValue>.Default;
            return Dictionary.TryGetValue(item.Key, out var value) && comparer.Equals(value, item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "Out of bounds");
            }

            if (array.Length - arrayIndex < Indexes.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "Out of bounds");
            }

            using var ug = _version.CreateGuard();
            var c = 0;
            foreach (var index in Indexes)
            {
                var res = Dictionary.TryGetValue(index, out var value);
                Debug.Assert(res);
                array[c + arrayIndex] = new KeyValuePair<TKey, TValue>(index, value);
                c++;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public int Count => Indexes.Count;

        public bool IsReadOnly => Indexes.IsReadOnly;

        public void Add(TKey key, TValue value)
        {
            DoAdd(in key, in value);
        }

        public bool ContainsKey(TKey key)
        {
            return Dictionary.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            using var ug = _version.CreateGuard(true);
            if (Dictionary.Remove(key))
            {
                var removed = Indexes.Remove(key);
                if (!removed)
                {
                    throw new InvalidOperationException();
                }

                return removed;
            }

            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get => Dictionary[key];
            set => DoAdd(in key, in value, true);
        }

        public virtual ICollection<TKey> Keys =>
            new KeyCollection<BaseOrderedDictionary<TKey, TValue, TList, TDictionary>>(this);

        public virtual ICollection<TValue> Values =>
            new ValuesCollection<BaseOrderedDictionary<TKey, TValue, TList, TDictionary>>(this);

        public TValue this[int index]
        {
            get => At(index).Value;
            set => UpdateAt(index, value);
        }

        public void Insert(int index, in TKey key, in TValue value)
        {
            CheckForOutOfBounds(index);

            if (index == Indexes.Count)
            {
                Add(key, value);
                return;
            }

            using var ug = _version.CreateGuard(true);
            Indexes.Insert(index, key);
            DoUpdate(in key, in value, false);
        }

        public void RemoveAt(int index)
        {
            CheckForOutOfBounds(index);

            using var ug = _version.CreateGuard(true);
            var key = Indexes[index];
            Indexes.RemoveAt(index);
            if (!Dictionary.Remove(key))
            {
                throw new InvalidOperationException();
            }
        }

        public KeyValuePair<TKey, TValue> At(int index)
        {
            var key = Indexes[index];
            return At(in key);
        }

        public KeyValuePair<TKey, TValue> At(in TKey key)
        {
            var res = Dictionary.TryGetValue(key, out var value);
            if (!res)
            {
                throw new KeyNotFoundException();
            }

            return new KeyValuePair<TKey, TValue>(key, value);
        }

        public TKey UpdateAt(in TKey key, in TValue value)
        {
            using var ug = _version.CreateGuard(true);
            DoUpdate(in key, in value);
            return key;
        }


        public TKey UpdateAt(int index, in TValue value)
        {
            CheckForOutOfBounds(index);

            using var ug = _version.CreateGuard(true);
            var key = Indexes[index];

            DoUpdate(in key, in value);
            return key;
        }

        protected void DoUpdate(in TKey key, in TValue value, bool ensureExists = true)
        {
            if (ensureExists && !Dictionary.ContainsKey(key))
            {
                throw new KeyNotFoundException();
            }

            Dictionary[key] = value;
        }

        protected void DoAdd(in TKey key, in TValue value, bool upsert = false)
        {
            using var ug = _version.CreateGuard(true);
            if (Dictionary.ContainsKey(key))
            {
                if (!upsert)
                {
                    throw new ArgumentException("key already exists", nameof(key));
                }

                DoUpdate(in key, in value, false);
                return;
            }

            Indexes.Add(key);
            try
            {
                Dictionary.Add(key, value);
            }
            catch (Exception)
            {
                ug.Check();
                Indexes.RemoveAt(Indexes.Count - 1);
                throw;
            }
        }

        private void CheckForOutOfBounds(int index)
        {
            if (index < 0 || index > Indexes.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "");
            }
        }


        protected class KeyCollection<TDict> : ICollection<TKey>
            where TDict : BaseOrderedDictionary<TKey, TValue, TList, TDictionary>
        {
            internal readonly TDict Dictionary;

            protected internal KeyCollection(TDict dictionary)
            {
                Dictionary = dictionary;
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                return Dictionary.Indexes.GetEnumerator();
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
                return Dictionary.Indexes.Contains(item);
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                Dictionary.Indexes.CopyTo(array, arrayIndex);
            }

            public bool Remove(TKey item)
            {
                throw new InvalidOperationException("readonly");
            }

            public int Count => Dictionary.Indexes.Count;

            public bool IsReadOnly => true;
        }

        protected class ValuesCollection<TDict> : ICollection<TValue>
            where TDict : BaseOrderedDictionary<TKey, TValue, TList, TDictionary>
        {
            protected internal readonly TDict Dictionary;

            protected internal ValuesCollection(TDict dictionary)
            {
                Dictionary = dictionary;
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                using var ug = Dictionary._version.CreateGuard();

                foreach (var pair in Dictionary)
                {
                    ug.Check();
                    yield return pair.Value;
                }
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
                return Dictionary.Dictionary.Values.Contains(item);
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                if (arrayIndex < 0 || arrayIndex > array.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "Out of bounds");
                }

                if (array.Length - arrayIndex < Dictionary.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "Out of bounds");
                }

                using var ug = Dictionary._version.CreateGuard();
                var c = 0;
                foreach (var index in Dictionary.Indexes)
                {
                    var res = Dictionary.TryGetValue(index, out var value);
                    Debug.Assert(res);
                    array[c + arrayIndex] = value;
                    c++;
                }
            }

            public bool Remove(TValue item)
            {
                throw new InvalidOperationException("readonly");
            }

            public int Count => Dictionary.Count;

            public bool IsReadOnly => true;
        }
    }
}