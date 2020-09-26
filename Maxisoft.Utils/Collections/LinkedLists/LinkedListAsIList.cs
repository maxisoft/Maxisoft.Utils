using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Maxisoft.Utils.Collections.LinkedLists
{
    /// <summary>
    ///     A linked list which implement <see cref="IList{T}"/>
    /// </summary>
    /// <remarks>All <see cref="IList{T}"/>'s methods added here are in <c>O(n)</c> time</remarks>
    /// <typeparam name="T"></typeparam>
    /// <see cref="LinkedList{T}" />
    /// <see cref="LinkedListExtensions"/>
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class LinkedListAsIList<T> : LinkedList<T>, IList<T>, IReadOnlyList<T>
    {
        public int IndexOf(T item)
        {
            return LinkedListExtensions.IndexOf(this, item);
        }

        public void Insert(int index, T item)
        {
            LinkedListExtensions.Insert(this, index, item);
        }

        public void RemoveAt(int index)
        {
            LinkedListExtensions.RemoveAt(this, index);
        }

        [SuppressMessage("ReSharper", "InvokeAsExtensionMethod")]
        public T this[int index]
        {
            get => LinkedListExtensions.At(this, index).Value;
            set => LinkedListExtensions.At(this, index).Value = value;
        }
    }
}