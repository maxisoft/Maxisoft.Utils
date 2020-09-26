using System;

namespace Maxisoft.Utils.Collections.Queues
{
    /// <summary>
    /// Defines the 3 commons usages of a <see cref="Deque{T}"/>
    /// </summary>
    /// <remarks>Used by the <see cref="Deque{T}"/> constructors in order to optimise wasted space.</remarks>
    public enum DequeInitialUsage
    {
        /// <summary>
        /// The deque is intended to be used as a lifo (Last in, First out).
        /// </summary>
        Lifo = 1,

        /// <summary>
        /// The deque is intended to be used as a Fifo (First in, Last out).
        /// </summary>
        Fifo = 1 << 1,

        /// <summary>
        /// The deque is intended to be used as a deque (ie both lifo and fifo operations are expected to be called).
        /// </summary>
        Both = Lifo | Fifo
    }

    public static class DequeInitialUsageExtensions
    {
        /// <summary>
        /// Convert the <see cref="DequeInitialUsage"/> into a float ratio.
        /// </summary>
        /// <param name="usage"></param>
        /// <returns>A float ratio.</returns>
        public static float ToRatio(this DequeInitialUsage usage)
        {
            return usage switch
            {
                DequeInitialUsage.Lifo => 0,
                DequeInitialUsage.Fifo => 1,
                DequeInitialUsage.Both => 0.5f,
                _ => throw new ArgumentOutOfRangeException(nameof(usage), usage, null)
            };
        }
    }
}