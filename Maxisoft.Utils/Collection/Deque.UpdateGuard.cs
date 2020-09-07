using System;
using System.Threading;

namespace Maxisoft.Utils.Collection
{
    public partial class Deque<T>
    {
        internal struct UpdateGuard : IDisposable
        {
            internal readonly int Version;
            internal readonly Deque<T> Deque;
            internal bool Increment { get; set; }

            public UpdateGuard(Deque<T> deque)
            {
                Version = deque._version;
                Deque = deque;
                Increment = false;
            }

            internal void Check()
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