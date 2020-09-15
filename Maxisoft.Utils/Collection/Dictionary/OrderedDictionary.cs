﻿using System.Collections.Generic;

namespace Maxisoft.Utils.Collection.Dictionary
{
    public class OrderedDictionary<TKey, TValue> : BaseOrderedDictionary<TKey, TValue, List<TKey>, Dictionary<TKey, TValue>>
    {
        public OrderedDictionary()
        {
        }
        
        public OrderedDictionary(IEqualityComparer<TKey> comparer) : base(new Dictionary<TKey, TValue>(comparer))
        {
        }
    }
}