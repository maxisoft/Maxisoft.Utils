﻿using System.Collections.Generic;

 namespace Maxisoft.Utils.Collections.Queue
{
    public interface IDeque<T> : ICollection<T>
    {
        /// <summary>
        ///     Access element at <paramref name="index">index</paramref> like an array
        /// </summary>
        /// <param name="index"></param>
        /// <returns>The <c>reference</c> to the element found at
        ///     <param name="index">index</param>
        /// </returns>
        ref T At(long index);

        /// <summary>
        ///     Access the first element
        /// </summary>
        /// <remarks>This is equivalent to <code>At(0)</code></remarks>
        /// <seealso cref="At" />
        /// <returns>The <c>reference</c> to the first element</returns>
        ref T Front();

        /// <summary>
        ///     Try to retrieve first element from the deque
        /// </summary>
        /// <param name="result">out param to store the first element</param>
        /// <returns><c>true</c> if there was such element, <c>false</c> in the other case</returns>
        bool TryPeekFront(out T result);

        /// <summary>
        ///     Access the last element
        /// </summary>
        /// <remarks>This is equivalent to <code>At(this.Count - 1)</code></remarks>
        /// <seealso cref="At" />
        /// <returns>The <c>reference</c> to the last element</returns>
        ref T Back();

        /// <summary>
        ///     Try to retrieve last element from the deque
        /// </summary>
        /// <param name="result">out param to store the last element</param>
        /// <returns><c>true</c> if there was such element, <c>false</c> in the other case</returns>
        bool TryPeekBack(out T result);

        /// <summary>
        ///     Add an <paramref name="element">element</paramref> at the end
        /// </summary>
        /// <param name="element">The element to add</param>
        /// <remarks>May be an alias for <c>ICollection.Add</c> </remarks>
        /// <seealso cref="ICollection{T}.Add" />
        void PushBack(in T element);

        /// <summary>
        ///     Add an <paramref name="element">element</paramref> at beginning
        /// </summary>
        /// <param name="element">The element to add</param>
        void PushFront(in T element);

        /// <summary>
        ///     Delete and retrieve last element
        /// </summary>
        /// <returns>the former last element</returns>
        T PopBack();

        /// <summary>
        ///     Delete and retrieve first element
        /// </summary>
        /// <returns>the former first element</returns>
        T PopFront();

        /// <summary>
        ///     Try to retrieve and remove last element from the deque
        /// </summary>
        /// <param name="result">out param to store the last element</param>
        /// <returns><c>true</c> if there was such element, <c>false</c> in the other case</returns>
        bool TryPopBack(out T result);

        /// <summary>
        ///     Try to retrieve and remove first element from the deque
        /// </summary>
        /// <param name="result"><c>out</c> param to store the first element</param>
        /// <returns><c>true</c> if there was such element, <c>false</c> in the other case</returns>
        bool TryPopFront(out T result);
    }
}