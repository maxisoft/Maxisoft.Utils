using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Maxisoft.Utils.Collection
{
    public partial class Deque<T>
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        protected internal struct UpdateGuard : IDisposable
        {
            internal readonly int Version;
            internal readonly Deque<T> Deque;
            public bool Increment { get; set; }

            public UpdateGuard(Deque<T> deque)
            {
                Version = deque._version;
                Deque = deque;
                Increment = false;
            }

            internal readonly void Check()
            {
                if (Deque._version != Version)
                {
                    throw new InvalidOperationException("Concurrent modification detected");
                }
            }

            public void Dispose()
            {
                Check();
                if (!Increment)
                {
                    return;
                }

                var res = Interlocked.Increment(ref Deque._version);
                if (res != Version + 1)
                {
                    throw new InvalidOperationException("Concurrent modification detected");
                }
            }
        }
    }
}