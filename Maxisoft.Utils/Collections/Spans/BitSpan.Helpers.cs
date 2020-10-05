using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Maxisoft.Utils.Collections.Spans
{
    public ref partial struct BitSpan
    {

        public ref struct Enumerator
        {
            /// <summary>The span being enumerated.</summary>
            private readonly BitSpan _bitSpan;
            /// <summary>The next index to yield.</summary>
            private int _index;

            /// <summary>Initialize the enumerator.</summary>
            /// <param name="dict">The dict to enumerate.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(BitSpan bitSpan)
            {
                _bitSpan = bitSpan;
                _index = -1;
            }

            /// <summary>Advances the enumerator to the next element of the dict.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                var index = _index + 1;
                if (index >= _bitSpan.Count)
                {
                    return false;
                }

                _index = index;
                return true;
            }

            /// <summary>Gets the element at the current position of the enumerator.</summary>
            public bool  Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _bitSpan.Get(_index);
            }
        }
        
        
    }
}