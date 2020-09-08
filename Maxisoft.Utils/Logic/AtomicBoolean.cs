using System.Threading;

namespace Maxisoft.Utils.Logic
{
    public sealed class AtomicBoolean
    {
        private const int TrueValue = 1;
        private const int FalseValue = 0;
        private volatile int _value = FalseValue;

        public AtomicBoolean()
            : this(false)
        {
        }

        public AtomicBoolean(bool initialValue)
        {
            Value = initialValue;
        }

        /// <summary>
        ///     best effort thread-safe boolean value
        /// </summary>
        public bool Value
        {
            get => _value != FalseValue;
            set
            {
                Interlocked.MemoryBarrier();
                _value = BoolToInt(value);
            }
        }

        private static int BoolToInt(bool value)
        {
            return value ? TrueValue : FalseValue;
        }

        /// <summary>
        ///     Attempt changing the backing value from true to false.
        /// </summary>
        /// <returns><c>true</c> on success</returns>
        public bool FalseToTrue()
        {
            return CompareExchange(true, false);
        }

        /// <summary>
        ///     Attempt changing the backing value from false to true.
        /// </summary>
        /// <returns><c>true</c> on success</returns>
        public bool TrueToFalse()
        {
            return CompareExchange(false, true);
        }

        /// <summary>
        ///     Attempt changing from "when" to "set".
        ///     Fails if the current value was not "when".
        /// </summary>
        /// <param name="set"></param>
        /// <param name="when"></param>
        /// <returns></returns>
        public bool CompareExchange(bool set, bool when)
        {
            var comparand = BoolToInt(when);
            var result = Interlocked.CompareExchange(ref _value, BoolToInt(set), comparand);
            var originalValue = result == TrueValue;
            return originalValue == when;
        }

        public static explicit operator bool(AtomicBoolean ab)
        {
            return ab.Value;
        }
        
        public static explicit operator AtomicBoolean(bool value)
        {
            return new AtomicBoolean(value);
        }
        
        public static explicit operator int(AtomicBoolean value)
        {
            return BoolToInt((bool) value);
        }
        
        public static bool operator true(AtomicBoolean ab)
        {
            return ab._value == TrueValue;
        }

        public static bool operator false(AtomicBoolean ab)
        {
            return ab._value == FalseValue;
        }
    }
}