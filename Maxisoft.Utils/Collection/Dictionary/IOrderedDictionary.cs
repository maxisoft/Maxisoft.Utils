﻿using System.Collections.Generic;

namespace Maxisoft.Utils.Collection.Dictionary
{
    public interface IOrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        public TValue this[int index] { get; set; }
        public void Insert (int index, in TKey key, in TValue value);
        public void RemoveAt (int index);
    }
}