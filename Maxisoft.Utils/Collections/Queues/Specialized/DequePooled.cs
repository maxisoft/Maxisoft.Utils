using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace Maxisoft.Utils.Collections.Queues.Specialized
{
    /// <summary>
    /// A <see cref="Deque{T}"/> using <see cref="ArrayPool"/>'s arrays as base storage
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Deque{T}"/>
    public class DequePooled<T> : Deque<T>
    {
        // ReSharper disable MemberCanBePrivate.Global
        public const int MaxArrayByteLength = 65536;

        public const int MaxArraysPerBucket = 16;

        /// <summary>
        /// The default <see cref="ArrayPool"/>
        /// </summary>
        /// <remarks>Use <see cref="Lazy{T}"/> in order to <b>prevent</b> the <see cref="ArrayPool"/> to be created as soon as this assembly is loaded</remarks>
        public static readonly Lazy<ArrayPool<T>> DefaultPool =
            new Lazy<ArrayPool<T>>(() => ArrayPool<T>.Create(ComputeOptimalChunkSize(), MaxArraysPerBucket));

        
        public readonly ArrayPool<T> ArrayPool;
        // ReSharper restore MemberCanBePrivate.Global

        /// <summary>
        /// Construct a new <see cref="Deque{T}"/> using the <see cref="DefaultPool"/>
        /// </summary>
        public DequePooled() : this(ComputeOptimalChunkSize())
        {
        }

        public DequePooled(ArrayPool<T> pool)
        {
            ArrayPool = pool;
            TrimOnDeletion = true;
        }

        public DequePooled(ArrayPool<T> pool, long chunkSize) : base(chunkSize)
        {
            ArrayPool = pool;
            TrimOnDeletion = true;
        }

        private DequePooled(long chunkSize) : base(chunkSize)
        {
            ArrayPool = DefaultPool.Value;
            TrimOnDeletion = true;
        }

        internal static int ComputeOptimalChunkSize()
        {
            var @sizeof = IntPtr.Size;
            try
            {
                @sizeof = Marshal.SizeOf<T>();
            }
            catch (ArgumentException)
            {
            }

            return MaxArrayByteLength / Math.Max(@sizeof, IntPtr.Size);
        }

        protected override T[] Alloc(long size)
        {
            var intSize = checked((int) size);
            return ArrayPool.Rent(intSize);
        }

        protected override void Free(T[] data)
        {
            ArrayPool.Return(data);
        }

        public override long OptimalChunkSize()
        {
            return ComputeOptimalChunkSize();
        }
    }
}