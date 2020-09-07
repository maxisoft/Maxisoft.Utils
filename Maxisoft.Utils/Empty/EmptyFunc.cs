using System;

#pragma warning disable 8603
// ReSharper disable MemberCanBePrivate.Global

namespace Maxisoft.Utils.Empty
{
    public struct EmptyFunc<TResult>: IEmpty
    {
        public static TResult Call()
        {
            return default;
        }

        public static implicit operator Func<TResult>(EmptyFunc<TResult> _)
        {
            return Call;
        }
    }

    public struct EmptyFunc<T1, TResult>: IEmpty
    {
        public static TResult Call(T1 _)
        {
            return default;
        }

        public static implicit operator Func<T1, TResult>(EmptyFunc<T1, TResult> _)
        {
            return Call;
        }
    }

    #region Generated EmptyFunc

    public struct EmptyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>: IEmpty
    {
        public static TResult Call(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11,
            T12 _12, T13 _13, T14 _14, T15 _15, T16 _16)
        {
            return default;
        }

        public static implicit operator
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(
                EmptyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> _)
        {
            return Call;
        }
    }

    public struct EmptyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>: IEmpty
    {
        public static TResult Call(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11,
            T12 _12, T13 _13, T14 _14, T15 _15)
        {
            return default;
        }

        public static implicit operator
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(
                EmptyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> _)
        {
            return Call;
        }
    }

    public struct EmptyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>: IEmpty
    {
        public static TResult Call(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11,
            T12 _12, T13 _13, T14 _14)
        {
            return default;
        }

        public static implicit operator Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(
            EmptyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> _)
        {
            return Call;
        }
    }

    public struct EmptyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>: IEmpty
    {
        public static TResult Call(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11,
            T12 _12, T13 _13)
        {
            return default;
        }

        public static implicit operator Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(
            EmptyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> _)
        {
            return Call;
        }
    }

    public struct EmptyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>: IEmpty
    {
        public static TResult Call(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11,
            T12 _12)
        {
            return default;
        }

        public static implicit operator Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(
            EmptyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> _)
        {
            return Call;
        }
    }

    public struct EmptyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>: IEmpty
    {
        public static TResult Call(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11)
        {
            return default;
        }

        public static implicit operator Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(
            EmptyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> _)
        {
            return Call;
        }
    }

    public struct EmptyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>: IEmpty
    {
        public static TResult Call(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10)
        {
            return default;
        }

        public static implicit operator Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(
            EmptyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> _)
        {
            return Call;
        }
    }

    public struct EmptyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>: IEmpty
    {
        public static TResult Call(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9)
        {
            return default;
        }

        public static implicit operator Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
            EmptyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> _)
        {
            return Call;
        }
    }

    public struct EmptyFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>: IEmpty
    {
        public static TResult Call(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8)
        {
            return default;
        }

        public static implicit operator Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
            EmptyFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult> _)
        {
            return Call;
        }
    }

    public struct EmptyFunc<T1, T2, T3, T4, T5, T6, T7, TResult>: IEmpty
    {
        public static TResult Call(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7)
        {
            return default;
        }

        public static implicit operator Func<T1, T2, T3, T4, T5, T6, T7, TResult>(
            EmptyFunc<T1, T2, T3, T4, T5, T6, T7, TResult> _)
        {
            return Call;
        }
    }

    public struct EmptyFunc<T1, T2, T3, T4, T5, T6, TResult>: IEmpty
    {
        public static TResult Call(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6)
        {
            return default;
        }

        public static implicit operator Func<T1, T2, T3, T4, T5, T6, TResult>(
            EmptyFunc<T1, T2, T3, T4, T5, T6, TResult> _)
        {
            return Call;
        }
    }

    public struct EmptyFunc<T1, T2, T3, T4, T5, TResult>: IEmpty
    {
        public static TResult Call(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5)
        {
            return default;
        }

        public static implicit operator Func<T1, T2, T3, T4, T5, TResult>(EmptyFunc<T1, T2, T3, T4, T5, TResult> _)
        {
            return Call;
        }
    }

    public struct EmptyFunc<T1, T2, T3, T4, TResult>: IEmpty
    {
        public static TResult Call(T1 _1, T2 _2, T3 _3, T4 _4)
        {
            return default;
        }

        public static implicit operator Func<T1, T2, T3, T4, TResult>(EmptyFunc<T1, T2, T3, T4, TResult> _)
        {
            return Call;
        }
    }

    public struct EmptyFunc<T1, T2, T3, TResult>: IEmpty
    {
        public static TResult Call(T1 _1, T2 _2, T3 _3)
        {
            return default;
        }

        public static implicit operator Func<T1, T2, T3, TResult>(EmptyFunc<T1, T2, T3, TResult> _)
        {
            return Call;
        }
    }

    public struct EmptyFunc<T1, T2, TResult>: IEmpty
    {
        public static TResult Call(T1 _1, T2 _2)
        {
            return default;
        }

        public static implicit operator Func<T1, T2, TResult>(EmptyFunc<T1, T2, TResult> _)
        {
            return Call;
        }
    }

    #endregion
}