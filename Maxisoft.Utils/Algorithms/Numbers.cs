using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Maxisoft.Utils.Algorithms
{
    public static class Numbers
    {
        public static T Clamp<T>(this T x, in T min, in T max) where T : IComparable<T>
        {
            return x.CompareTo(min) < 0 ? min : x.CompareTo(max) > 0 ? max : x;
        }

        public static T Clamp<T, TComparer>(T x, in T min, in T max, TComparer comparer) where TComparer: IComparer<T>
        {
            return comparer.Compare(x, min) < 0 ? min : comparer.Compare(x, max) > 0 ? max : x;
        }

        #region Clamp generated

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Clamp(this byte x, byte min, byte max)
        {
            Debug.Assert(min <= max);
            return x < min ? min : max < x ? max : x;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte Clamp(this sbyte x, sbyte min, sbyte max)
        {
            Debug.Assert(min <= max);
            return x < min ? min : max < x ? max : x;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short Clamp(this short x, short min, short max)
        {
            Debug.Assert(min <= max);
            return x < min ? min : max < x ? max : x;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Clamp(this ushort x, ushort min, ushort max)
        {
            Debug.Assert(min <= max);
            return x < min ? min : max < x ? max : x;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(this int x, int min, int max)
        {
            Debug.Assert(min <= max);
            return x < min ? min : max < x ? max : x;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Clamp(this uint x, uint min, uint max)
        {
            Debug.Assert(min <= max);
            return x < min ? min : max < x ? max : x;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Clamp(this long x, long min, long max)
        {
            Debug.Assert(min <= max);
            return x < min ? min : max < x ? max : x;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Clamp(this ulong x, ulong min, ulong max)
        {
            Debug.Assert(min <= max);
            return x < min ? min : max < x ? max : x;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(this float x, float min, float max)
        {
            return Math.Max(min, Math.Min(x, max));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp(this double x, double min, double max)
        {
            return Math.Max(min, Math.Min(x, max));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal Clamp(this decimal x, decimal min, decimal max)
        {
            return Math.Max(min, Math.Min(x, max));
        }

        #endregion
    }
}