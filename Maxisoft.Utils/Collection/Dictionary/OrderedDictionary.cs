using System.Collections.Generic;

namespace Maxisoft.Utils.Collection.Dictionary
{
    public class
        OrderedDictionary<TKey, TValue> : BaseOrderedDictionary<TKey, TValue, List<TKey>, Dictionary<TKey, TValue>>
    {
        public OrderedDictionary()
        {
        }

        public OrderedDictionary(int capacity) : base(new Dictionary<TKey, TValue>(capacity))
        {
        }

        public OrderedDictionary(IEqualityComparer<TKey> comparer) : base(new Dictionary<TKey, TValue>(comparer))
        {
        }

        public OrderedDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(
            new Dictionary<TKey, TValue>(capacity, comparer))
        {
        }

        public override void Move(int fromIndex, int toIndex)
        {
            CheckForOutOfBounds(fromIndex);
            CheckForOutOfBounds(toIndex);
            if (fromIndex == toIndex)
            {
                return;
            }

            using var ug = Version.CreateGuard(true);

            var tmp = Indexes[fromIndex];
            if (fromIndex < toIndex)
            {
                for (var i = fromIndex; i < toIndex; i++)
                {
                    Indexes[i] = Indexes[i + 1];
                }
            }
            else
            {
                for (var i = fromIndex; i > toIndex; i--)
                {
                    Indexes[i] = Indexes[i - 1];
                }
            }

            Indexes[toIndex] = tmp;
        }
    }
}