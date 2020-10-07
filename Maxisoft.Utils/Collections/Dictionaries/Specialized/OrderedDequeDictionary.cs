using System;
using System.Collections.Generic;
using System.Diagnostics;
using Maxisoft.Utils.Collections.Queues;

namespace Maxisoft.Utils.Collections.Dictionaries.Specialized
{
    public class OrderedDequeDictionary<TKey, TValue> : OrderedDictionary<TKey, TValue, Deque<TKey>, Dictionary<TKey, TValue>>, IOrderedDequeDictionary<TKey, TValue>
    {
        public OrderedDequeDictionary()
        {
        }
        
        public OrderedDequeDictionary(int capacity) : base(new Dictionary<TKey, TValue>(capacity))
        {
            
        }
        
        public OrderedDequeDictionary(IEqualityComparer<TKey> comparer) : base(new Dictionary<TKey, TValue>(comparer))
        {
        }
        
        public OrderedDequeDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(new Dictionary<TKey, TValue>(capacity, comparer))
        {
        }


        public void PushBack(in TKey key, in TValue value)
        {
            DoAdd(in key, in value);
        }

        public void PushFront(in TKey key, in TValue value)
        {
            var version = Version;
            if (Dictionary.ContainsKey(key))
            {
                throw new ArgumentException("key already exists", nameof(key));
            }

            Indexes.PushFront(key);
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
            var key = Indexes.PopBack();
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
            var key = Indexes.PopFront();
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
    }
}