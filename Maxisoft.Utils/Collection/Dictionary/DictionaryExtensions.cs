using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Maxisoft.Utils.Collection.Dictionary
{
    public static class DictionaryExtensions
    {
        public static IEnumerable<KeyValuePair<TKey, TValue>> GetKeyValuePairEnumerator<TKey, TValue>(
            // ReSharper disable once SuggestBaseTypeForParameter
            this IOrderedDictionary dictionary)
        {
            return dictionary.Cast<DictionaryEntry>().Select(entry =>
                new KeyValuePair<TKey, TValue>((TKey) entry.Key, (TValue) entry.Value));
        }
        
        public static OrderedDictionary<TKey, TValue> ToOrderedDictionary<TKey, TValue>(
            this IOrderedDictionary dictionary)
        {
            var res = new OrderedDictionary<TKey, TValue>(dictionary.Count);
            foreach (var pair in GetKeyValuePairEnumerator<TKey, TValue>(dictionary))
            {
                res.Add(pair);
            }

            return res;
        }
        
        public static OrderedDictionary<TKey, TValue> ToOrderedDictionary<TKey, TValue>(
            this IOrderedDictionary dictionary, EqualityComparer<TKey> comparer)
        {
            var res = new OrderedDictionary<TKey, TValue>(dictionary.Count, comparer);
            foreach (var pair in GetKeyValuePairEnumerator<TKey, TValue>(dictionary))
            {
                res.Add(pair);
            }

            return res;
        }
        
        public static OrderedDictionary<TKey, TValue> ToOrderedDictionary<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
        {
            var res = new OrderedDictionary<TKey, TValue>();
            foreach (var pair in enumerable)
            {
                res.Add(pair);
            }

            return res;
        }
        
        public static OrderedDictionary<TKey, TValue> ToOrderedDictionary<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> enumerable, EqualityComparer<TKey> comparer)
        {
            var res = new OrderedDictionary<TKey, TValue>(comparer);
            foreach (var pair in enumerable)
            {
                res.Add(pair);
            }

            return res;
        }
        
        public static OrderedDictionary<TKey, TValue> ToOrderedDictionary<TKey, TValue>(
            this ICollection<KeyValuePair<TKey, TValue>> collection)
        {
            var res = new OrderedDictionary<TKey, TValue>(collection.Count);
            foreach (var pair in collection)
            {
                res.Add(pair);
            }

            return res;
        }
        
        public static OrderedDictionary<TKey, TValue> ToOrderedDictionary<TKey, TValue>(
            this ICollection<KeyValuePair<TKey, TValue>> collection, EqualityComparer<TKey> comparer)
        {
            var res = new OrderedDictionary<TKey, TValue>(collection.Count, comparer);
            foreach (var pair in collection)
            {
                res.Add(pair);
            }

            return res;
        }
        
        public static OrderedDictionary<TKey, TValue> ToOrderedDictionary<TKey, TValue>(
            this IEnumerable<ValueTuple<TKey, TValue>> enumerable, EqualityComparer<TKey> comparer)
        {
            var res = new OrderedDictionary<TKey, TValue>(comparer);
            foreach (var (key, value) in enumerable)
            {
                res.Add(key, value);
            }

            return res;
        }
        
        
        public static OrderedDictionary<TKey, TValue> ToOrderedDictionary<TKey, TValue>(
            this IEnumerable<Tuple<TKey, TValue>> enumerable, EqualityComparer<TKey> comparer)
        {
            var res = new OrderedDictionary<TKey, TValue>(comparer);
            foreach (var (key, value) in enumerable)
            {
                res.Add(key, value);
            }

            return res;
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, in TKey key, in TValue value)
        {
            if (dictionary.TryGetValue(key, out var existing))
            {
                return existing;
            }
            dictionary.Add(key, value);
            return dictionary[key];
        }
        
        
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, in TKey key, in Func<TValue> valueFactory)
        {
            if (dictionary.TryGetValue(key, out var existing))
            {
                return existing;
            }
            dictionary.Add(key, valueFactory());
            return dictionary[key];
        }
        
        public static async Task<TValue> GetOrAddAsync<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Task<TValue> valueFactory)
        {
            if (dictionary.TryGetValue(key, out var existing))
            {
                return existing;
            }
            dictionary.Add(key, await valueFactory);
            return dictionary[key];
        }
    }
}