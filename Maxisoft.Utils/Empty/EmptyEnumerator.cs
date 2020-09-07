using System;
using System.Collections;
using System.Collections.Generic;

namespace Maxisoft.Utils.Empty
{
    public struct EmptyEnumerator : IEnumerator, IEmpty
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

    public struct EmptyEnumerator<T> : IEnumerator<T>, IEmpty
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