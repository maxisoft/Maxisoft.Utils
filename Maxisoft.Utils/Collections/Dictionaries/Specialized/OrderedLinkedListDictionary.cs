using System;
using System.Collections.Generic;
using System.Diagnostics;
using Maxisoft.Utils.Collections.LinkedLists;

namespace Maxisoft.Utils.Collections.Dictionaries.Specialized
{
    public class OrderedLinkedListDictionary<TKey, TValue> : OrderedDictionary<TKey, TValue, LinkedListAsIList<TKey>, Dictionary<TKey, TValue>>, IOrderedDequeDictionary<TKey, TValue>
    {
        public OrderedLinkedListDictionary()
        {
        }
        
        public OrderedLinkedListDictionary(int capacity) : base(new Dictionary<TKey, TValue>(capacity))
        {
            
        }
        
        public OrderedLinkedListDictionary(IEqualityComparer<TKey> comparer) : base(new Dictionary<TKey, TValue>(comparer))
        {
        }
        
        public OrderedLinkedListDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(new Dictionary<TKey, TValue>(capacity, comparer))
        {
        }
        
        public void PushBack(in TKey key, in TValue value)
        {
            Add(key, value);
        }

        public void PushFront(in TKey key, in TValue value)
        {
            var version = Version;
            if (Dictionary.ContainsKey(key))
            {
                throw new ArgumentException("key already exists", nameof(key));
            }

            Indexes.AddFirst(key);
            try
            {
                Dictionary.Add(key, value);
            }
            catch (Exception)
            {
                CheckForConcurrentModification(version);
                Indexes.RemoveAt(0);
                throw;
            }
            finally
            {
                Version += 1;
                Debug.Assert(Dictionary.Count == Indexes.Count);
            }
        }

        public bool TryPopBack(out KeyValuePair<TKey, TValue> result)
        {
            if (Count == 0)
            {
                result = default;
                return false;
            }
            var key = Indexes.Last.Value;
            Indexes.RemoveLast();
            if (!Dictionary.TryGetValue(key, out var value))
            {
                throw new InvalidOperationException();
            }
            if (!Dictionary.Remove(key))
            {
                throw new InvalidOperationException();
            }
            Debug.Assert(Dictionary.Count == Indexes.Count);
            result = new KeyValuePair<TKey, TValue>(key, value);
            Version += 1;
            return true;
        }

        public bool TryPopFront(out KeyValuePair<TKey, TValue> result)
        {
            if (Count == 0)
            {
                result = default;
                return false;
            }
            var key = Indexes.First.Value;
            Indexes.RemoveFirst();
            if (!Dictionary.TryGetValue(key, out var value))
            {
                throw new InvalidOperationException();
            }

            if (!Dictionary.Remove(key))
            {
                throw new InvalidOperationException();
            }
            Debug.Assert(Dictionary.Count == Indexes.Count);
            result = new KeyValuePair<TKey, TValue>(key, value);
            Version += 1;
            return true;
        }

        public override void Swap(int firstIndex, int secondIndex)
        {
            Indexes.Swap(firstIndex, secondIndex);
            Version += 1;
        }
    }
}