using System;
using System.Diagnostics;
using System.Threading;

namespace Maxisoft.Utils.Time
{
    public readonly partial struct MonotonicTimestamp
    {
        internal interface IEnvironment
        {
            int TickCount { get; }
        }

        internal readonly struct DefaultEnvironment : IEnvironment
        {
            public int TickCount => Environment.TickCount;
        }

        /// <summary>
        /// Provide a TickCount measurement with overflow support
        /// </summary>
        /// <typeparam name="T">Specify a TickCount implementation</typeparam>
        /// <seealso cref="Environment.TickCount"/>
        internal struct TickCountProvider<T> where T : struct, IEnvironment
        {
            internal long LastTick
            {
                get => _lastTick;
                set
                {
                    checked
                    {
                        _lastTick = (uint) value;
                    }
                }
            }


            internal volatile int Overflow;
            private SpinLock _lock;
            private long _lastTick;
            private uint _firstTick;

            internal static TickCountProvider<T> Now
            {
                get
                {
                    var tickCount = new T().TickCount;
                    return new TickCountProvider<T>
                    {
                        FirstTick = tickCount & int.MaxValue,
                        LastTick = tickCount & int.MaxValue,
                    };
                }
            }

            public long FirstTick
            {
                get => _firstTick;
                set
                {
                    checked
                    {
                        _firstTick = (uint) value;
                    }
                }
            }

            internal long TickCount()
            {
                //TODO use TickCount64 when available
                var tick = (uint) new T().TickCount;
                var lastTick = LastTick;
                var lockTaken = false;
                try
                {
                    if (tick < lastTick) // handle overflow
                    {
                        while (!lockTaken)
                        {
                            _lock.Enter(ref lockTaken);
                        }


                        lastTick = LastTick;
                        if (tick < lastTick) // 2x check
                        {
                            Interlocked.Increment(ref Overflow);
                            LastTick = tick;
                        }
                    }
                    else
                    {
                        while (tick > lastTick)
                        {
                            while (!lockTaken)
                            {
                                _lock.Enter(ref lockTaken);
                            }

                            lastTick = Interlocked.CompareExchange(ref _lastTick, tick, lastTick);
                        }
                    }

                    return (Overflow * (1L << 32)) | tick;
                }
                finally
                {
                    if (lockTaken)
                    {
                        _lock.Exit(true);
                    }
                }
            }
        }
    }
}