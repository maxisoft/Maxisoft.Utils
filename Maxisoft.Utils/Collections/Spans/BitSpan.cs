using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Maxisoft.Utils.Collections.Spans
{
    public ref partial struct BitSpan
    {
        public readonly Span<long> Span;
        public const int LongNumBit = sizeof(long) * 8;

        public BitSpan(Span<long> span)
        {
            Span = span;
        }

        public bool this[int index]
        {
            get => Get(index);
            set => Set(index, value);
        }

        private readonly void ThrowForOutOfBounds(int index)
        {
            if ((uint) index >= LongNumBit * (uint) Span.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
        }

        public readonly bool Get(int index)
        {
            ThrowForOutOfBounds(index);
            return (Span[index / LongNumBit] & (1L << (index % LongNumBit))) != 0;
        }

        public void Set(int index, bool value)
        {
            ThrowForOutOfBounds(index);

            if (value)
            {
                Span[index / LongNumBit] |= 1L << (index % LongNumBit);
            }
            else
            {
                Span[index / LongNumBit] &= ~(1L << (index % LongNumBit));
            }
        }


        public void SetAll(bool value)
        {
            if (value)
            {
                Span.Fill(unchecked((long) ulong.MaxValue));
            }
            else
            {
                Span.Clear();
            }
        }

        public BitSpan And(in BitSpan other)
        {
            if (Span.Length < other.Span.Length)
            {
                throw new ArgumentException();
            }

            for (var i = 0; i < other.Span.Length; i++)
            {
                Span[i] &= other.Span[i];
            }

            return this;
        }

        public BitSpan Or(in BitSpan other)
        {
            if (Span.Length < other.Span.Length)
            {
                throw new ArgumentException();
            }

            for (var i = 0; i < other.Span.Length; i++)
            {
                Span[i] |= other.Span[i];
            }

            return this;
        }

        public BitSpan Xor(in BitSpan other)
        {
            if (Span.Length < other.Span.Length)
            {
                throw new ArgumentException();
            }

            for (var i = 0; i < other.Span.Length; i++)
            {
                Span[i] ^= other.Span[i];
            }

            return this;
        }

        public BitSpan Not()
        {
            for (var i = 0; i < Span.Length; i++)
            {
                Span[i] = ~Span[i];
            }

            return this;
        }

        public static implicit operator ReadOnlySpan<long>(BitSpan bs)
        {
            return bs.Span;
        }

        public static implicit operator Span<long>(BitSpan bs)
        {
            return bs.Span;
        }

        public static implicit operator BitSpan(Span<long> span)
        {
            return new BitSpan(span);
        }

        public static implicit operator BitSpan(Span<int> span)
        {
            return new BitSpan(MemoryMarshal.Cast<int, long>(span));
        }

        public static explicit operator BitArray(BitSpan span)
        {
            return span.ToBitArray();
        }

        public static implicit operator BitSpan(BitArray bitArray)
        {
            var arr = new int[ComputeLongArraySize(bitArray.Count) / (sizeof(long) / sizeof(int))];
            bitArray.CopyTo(arr, 0);
            return (Span<int>) arr;
        }

        public readonly Span<T> ASpan<T>() where T : unmanaged
        {
            return MemoryMarshal.Cast<long, T>(Span);
        }

        public readonly BitArray ToBitArray()
        {
            return new BitArray(ASpan<int>().ToArray());
        }

        public int Length => Span.Length * LongNumBit;

        public int Count => Length;

        public readonly Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public static int ComputeLongArraySize(int numBits)
        {
            var n = numBits / LongNumBit;
            if (numBits % LongNumBit != 0)
            {
                n += 1;
            }

            return n;
        }

        public static BitSpan Zeros(int numBits)
        {
            return new BitSpan(new long[ComputeLongArraySize(numBits)]);
        }
    }
}