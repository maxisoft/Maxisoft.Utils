using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Maxisoft.Utils.Collections.Queue
{
    public partial class Deque<T>
    {
        [DebuggerNonUserCode]
        private sealed class DebuggerTypeProxyImpl
        {
            private readonly Deque<T> _deque;

            public DebuggerTypeProxyImpl(Deque<T> deque)
            {
                _deque = deque;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
            public long Count => _deque.LongLength;

            [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
            public long ChunkSize => _deque.ChunkSize;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public LinkedList<T[]> Map => _deque._map;

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public T[] Items => _deque.ToArray();
        }
    }
}