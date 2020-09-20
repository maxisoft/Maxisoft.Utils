using System.Collections.Generic;

namespace Maxisoft.Utils.Collection.Dictionary.Specialized
{
    public interface IOrderedDequeDictionary<TKey, TValue> : IOrderedDictionary<TKey, TValue>
    {
        /// <summary>
        ///     Add a key value pair at the end.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <remarks>May be an alias for <c>ICollection.Add</c> </remarks>
        /// <seealso cref="ICollection{T}.Add" />
        void PushBack(in TKey key, in TValue value);

        /// <summary>
        ///     Add a key value pair at beginning
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void PushFront(in TKey key, in TValue value);
        
        
        /// <summary>
        ///     Try to retrieve and remove last pair from the dictionary
        /// </summary>
        /// <param name="result">out param to store the last element</param>
        /// <returns><c>true</c> if there was such element, <c>false</c> in the other case</returns>
        bool TryPopBack(out KeyValuePair<TKey, TValue> result);

        /// <summary>
        ///     Try to retrieve and remove first pair from the deque
        /// </summary>
        /// <param name="result"><c>out</c> param to store the first element</param>
        /// <returns><c>true</c> if there was such element, <c>false</c> in the other case</returns>
        bool TryPopFront(out KeyValuePair<TKey, TValue> result);
    }
}