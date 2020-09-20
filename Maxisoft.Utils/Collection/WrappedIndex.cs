﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Maxisoft.Utils.Collection
{

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Prefer .NET Standard 2.1 <see cref="System.Index"/> when available</remarks>
    public readonly struct WrappedIndex
    {
        public readonly int Value;

        public WrappedIndex(int value)
        {
            Value = value;
        }

        public static implicit operator WrappedIndex(int value)
        {
            return new WrappedIndex(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Resolve(int size)
        {
            return Value < 0 ? size + Value : size;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Resolve<T, TCollection>(in TCollection collection) where TCollection: ICollection<T>
        {
            return Resolve(collection.Count);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Resolve<TCollection>(in TCollection collection) where TCollection: ICollection
        {
            return Resolve(collection.Count);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Resolve(in Array array)
        {
            return Resolve(array.Length);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Resolve<T>(in T[] array)
        {
            return Resolve(array.Length);
        }
    }
}