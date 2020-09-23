using System;
using System.Collections;
using System.Collections.Generic;

namespace Maxisoft.Utils.Empties
{
    public readonly struct EmptyEnumerator : IEnumerator, IEmpty
    {
        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
        }

        public object Current => throw new InvalidOperationException("Empty by design");
    }

    public readonly struct EmptyEnumerator<T> : IEnumerator<T>, IEmpty
    {
        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
        }

        public T Current => throw new InvalidOperationException("Empty by design");

        object IEnumerator.Current => Current!;

        public void Dispose()
        {
        }
    }
}