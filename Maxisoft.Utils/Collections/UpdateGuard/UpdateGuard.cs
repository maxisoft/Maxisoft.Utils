using System;
using System.Threading;

namespace Maxisoft.Utils.Collections.UpdateGuard
{
    public struct UpdateGuard<T> : IDisposable where T : class, IUpdateGuarded
    {
        public readonly int Version;
        public bool PostIncrementVersionCounter { get; set; }
        public bool CancelChecks { get; set; }

        public readonly T Guarded;

        public UpdateGuard(T guarded)
        {
            Version = guarded.GetInternalVersionCounter();
            Guarded = guarded;
            PostIncrementVersionCounter = false;
            CancelChecks = false;
        }

        public readonly void Check(bool force = true)
        {
            if ((!CancelChecks || force) && Guarded.GetInternalVersionCounter() != Version)
            {
                throw new InvalidOperationException("Concurrent modification detected");
            }
        }

        public void Dispose()
        {
            Check(false);
            if (!PostIncrementVersionCounter)
            {
                return;
            }

            var res = Interlocked.Increment(ref Guarded.GetInternalVersionCounter());
            if (res != Version + 1)
            {
                throw new InvalidOperationException("Concurrent modification detected");
            }
        }
    }
}