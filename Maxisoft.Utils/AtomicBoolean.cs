using System.Threading;

namespace Maxisoft.Utils
{
    public class AtomicBoolean
    {
        private const int TrueValue = 1;
        private const int FalseValue = 0;
        private int _value = FalseValue;
        
        private static int BoolToInt(bool value)
        {
            return value ? TrueValue : FalseValue;
        }

        public AtomicBoolean()
            : this(false)
        {
        }

        public AtomicBoolean(bool initialValue)
        {
            Value = initialValue;
        }

        /// <summary>
        ///     non-thread-safe boolean value
        /// </summary>
        public bool Value
        {
            get => _value != FalseValue;
            set => _value = BoolToInt(value);
        }

        /// <summary>
        ///     Attempt changing the backing value from true to false.
        /// </summary>
        /// <returns>Whether the value was (atomically) changed from false to true.</returns>
        public bool FalseToTrue()
        {
            return CompareExchange(true, false);
        }

        /// <summary>
        ///     Attempt changing the backing value from false to true.
        /// </summary>
        /// <returns>Whether the value was (atomically) changed from true to false.</returns>
        public bool TrueToFalse()
        {
            return CompareExchange(false, true);
        }

        /// <summary>
        ///     Attempt changing from "whenValue" to "setToValue".
        ///     Fails if this.Value is not "whenValue".
        /// </summary>
        /// <param name="setToValue"></param>
        /// <param name="whenValue"></param>
        /// <returns></returns>
        public bool CompareExchange(bool setToValue, bool whenValue)
        {
            var comparand = BoolToInt(whenValue);
            var result = Interlocked.CompareExchange(ref _value, BoolToInt(setToValue), comparand);
            var originalValue = result == TrueValue;
            return originalValue == whenValue;
        }

        public static implicit operator bool(AtomicBoolean ab)
        {
            return ab.Value;
        }
    }
}