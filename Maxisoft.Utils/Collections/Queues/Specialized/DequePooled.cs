using System;
using System.Buffers;
using System.Runtime.InteropServices;
using Maxisoft.Utils.Collections.Allocators;

namespace Maxisoft.Utils.Collections.Queues.Specialized
{
    /// <summary>
    ///     A <see cref="Deque{T}" /> using <see cref="ArrayPool{T}" />'s arrays as base storage
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Deque{T}" />
    public class DequePooled<T> : Deque<T>, IDisposable
    {
        /// <summary>
        ///     Construct a new <see cref="Deque{T}" /> using the <see cref="DefaultPool" />
        /// </summary>
        public DequePooled() : this(ComputeOptimalChunkSize())
        {
        }

        public DequePooled(ArrayPool<T> pool) : base(new PooledAllocator<T>(pool))
        {
            TrimOnDeletion = true;
        }

        public DequePooled(ArrayPool<T> pool, long chunkSize, DequeInitialUsage usage = DequeInitialUsage.Both) : base(chunkSize,
            usage.ToRatio(), new PooledAllocator<T>(pool))
        {
            TrimOnDeletion = true;
        }

        public DequePooled(long chunkSize, DequeInitialUsage usage = DequeInitialUsage.Both) : base(chunkSize, usage.ToRatio(), new PooledAllocator<T>(DefaultPool.Value))
        {
            TrimOnDeletion = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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

        public override long OptimalChunkSize()
        {
            return ComputeOptimalChunkSize();
        }

        private void ReleaseUnmanagedResources()
        {
            Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
        }

        ~DequePooled()
        {
            Dispose(false);
        }

        // ReSharper disable MemberCanBePrivate.Global
        public const int MaxArrayByteLength = 65536;

        public const int MaxArraysPerBucket = 16;

        /// <summary>
        ///     The default <see cref="ArrayPool{T}" />
        /// </summary>
        /// <remarks>
        ///     Use <see cref="Lazy{T}" /> in order to <b>prevent</b> the <see cref="ArrayPool{T}" /> to be created as soon as
        ///     this assembly is loaded
        /// </remarks>
        public static readonly Lazy<ArrayPool<T>> DefaultPool =
            new Lazy<ArrayPool<T>>(() => ArrayPool<T>.Create(ComputeOptimalChunkSize(), MaxArraysPerBucket));
        
        // ReSharper restore MemberCanBePrivate.Global
    }
}