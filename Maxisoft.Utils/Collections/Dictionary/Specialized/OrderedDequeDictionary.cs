using System;
using System.Collections.Generic;
using System.Diagnostics;
using Maxisoft.Utils.Collections.Queue;

namespace Maxisoft.Utils.Collections.Dictionary.Specialized
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
            using var ug = Version.CreateGuard(true);
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
                ug.Check();
                Indexes.RemoveAt(0);
                throw;
            }
            finally
            {
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
            using var ug = Version.CreateGuard(true);
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
            return true;
        }

        public bool TryPopFront(out KeyValuePair<TKey, TValue> result)
        {
            if (Count == 0)
            {
                result = default;
                return false;
            }
            using var ug = Version.CreateGuard(true);
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
            return true;
        }
    }
}