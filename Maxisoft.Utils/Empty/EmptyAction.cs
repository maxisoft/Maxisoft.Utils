using System;

// ReSharper disable MemberCanBePrivate.Global

namespace Maxisoft.Utils.Empty
{
    public readonly struct EmptyAction : IEmpty
    {
        public static void Call()
        {
        }

        public static implicit operator Action(EmptyAction _)
        {
            return Call;
        }
    }

    public readonly struct EmptyAction<T1> : IEmpty
    {
        public static void Call(T1 _)
        {
        }

        public static implicit operator Action<T1>(EmptyAction<T1> _)
        {
            return Call;
        }
    }

    #region Generated EmptyAction

    public readonly struct EmptyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : IEmpty
    {
        public static void Call(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11,
            T12 _12, T13 _13, T14 _14, T15 _15, T16 _16)
        {
        }

        public static implicit operator Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
            EmptyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> _)
        {
            return Call;
        }
    }

    public readonly struct EmptyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : IEmpty
    {
        public static void Call(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11,
            T12 _12, T13 _13, T14 _14, T15 _15)
        {
        }

        public static implicit operator Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
            EmptyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> _)
        {
            return Call;
        }
    }

    public readonly struct EmptyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : IEmpty
    {
        public static void Call(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11,
            T12 _12, T13 _13, T14 _14)
        {
        }

        public static implicit operator Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
            EmptyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> _)
        {
            return Call;
        }
    }

    public readonly struct EmptyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : IEmpty
    {
        public static void Call(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11,
            T12 _12, T13 _13)
        {
        }

        public static implicit operator Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
            EmptyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> _)
        {
            return Call;
        }
    }

    public readonly struct EmptyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : IEmpty
    {
        public static void Call(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11,
            T12 _12)
        {
        }

        public static implicit operator Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            EmptyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> _)
        {
            return Call;
        }
    }

    public readonly struct EmptyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : IEmpty
    {
        public static void Call(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11)
        {
        }

        public static implicit operator Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
            EmptyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> _)
        {
            return Call;
        }
    }

    public readonly struct EmptyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IEmpty
    {
        public static void Call(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10)
        {
        }

        public static implicit operator Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            EmptyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> _)
        {
            return Call;
        }
    }

    public readonly struct EmptyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> : IEmpty
    {
        public static void Call(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9)
        {
        }

        public static implicit operator Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            EmptyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> _)
        {
            return Call;
        }
    }

    public readonly struct EmptyAction<T1, T2, T3, T4, T5, T6, T7, T8> : IEmpty
    {
        public static void Call(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8)
        {
        }

        public static implicit operator Action<T1, T2, T3, T4, T5, T6, T7, T8>(
            EmptyAction<T1, T2, T3, T4, T5, T6, T7, T8> _)
        {
            return Call;
        }
    }

    public readonly struct EmptyAction<T1, T2, T3, T4, T5, T6, T7> : IEmpty
    {
        public static void Call(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7)
        {
        }

        public static implicit operator Action<T1, T2, T3, T4, T5, T6, T7>(EmptyAction<T1, T2, T3, T4, T5, T6, T7> _)
        {
            return Call;
        }
    }

    public readonly struct EmptyAction<T1, T2, T3, T4, T5, T6> : IEmpty
    {
        public static void Call(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6)
        {
        }

        public static implicit operator Action<T1, T2, T3, T4, T5, T6>(EmptyAction<T1, T2, T3, T4, T5, T6> _)
        {
            return Call;
        }
    }

    public readonly struct EmptyAction<T1, T2, T3, T4, T5> : IEmpty
    {
        public static void Call(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5)
        {
        }

        public static implicit operator Action<T1, T2, T3, T4, T5>(EmptyAction<T1, T2, T3, T4, T5> _)
        {
            return Call;
        }
    }

    public readonly struct EmptyAction<T1, T2, T3, T4> : IEmpty
    {
        public static void Call(T1 _1, T2 _2, T3 _3, T4 _4)
        {
        }

        public static implicit operator Action<T1, T2, T3, T4>(EmptyAction<T1, T2, T3, T4> _)
        {
            return Call;
        }
    }

    public readonly struct EmptyAction<T1, T2, T3> : IEmpty
    {
        public static void Call(T1 _1, T2 _2, T3 _3)
        {
        }

        public static implicit operator Action<T1, T2, T3>(EmptyAction<T1, T2, T3> _)
        {
            return Call;
        }
    }

    public readonly struct EmptyAction<T1, T2> : IEmpty
    {
        public static void Call(T1 _1, T2 _2)
        {
        }

        public static implicit operator Action<T1, T2>(EmptyAction<T1, T2> _)
        {
            return Call;
        }
    }

    #endregion
}