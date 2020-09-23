using System;
using System.Collections;

namespace Maxisoft.Utils.Empties
{
    public readonly struct EmptyDictionaryEnumerator : IDictionaryEnumerator, IEmpty
    {
        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
        }

        public object Current => throw new InvalidOperationException("Empty by design");

        public DictionaryEntry Entry => throw new InvalidOperationException("Empty by design");

        public object Key => throw new InvalidOperationException("Empty by design");

        public object Value => throw new InvalidOperationException("Empty by design");
    }
}