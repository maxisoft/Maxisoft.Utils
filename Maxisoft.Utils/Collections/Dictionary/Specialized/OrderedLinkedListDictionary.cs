using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Maxisoft.Utils.Collections.Dictionary.Specialized
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
            using var ug = Version.CreateGuard(true);
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
            return true;
        }

        public override void Swap(int firstIndex, int secondIndex)
        {
            CheckForOutOfBounds(firstIndex);
            CheckForOutOfBounds(secondIndex);
            
            using var ug = Version.CreateGuard(true);
            var firstNode = Indexes.At(firstIndex);
            var secondNode = Indexes.At(secondIndex);
            var tmp = firstNode.Value;
            firstNode.Value = secondNode.Value;
            secondNode.Value = tmp;
        }
    }
}