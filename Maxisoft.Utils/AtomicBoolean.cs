using System.Threading;

namespace Maxisoft.Utils
{
    public class AtomicBoolean
    {
        private const int TrueValue = 1;
        private const int FalseValue = 0;
        private int _zeroOrOne = FalseValue;

        public AtomicBoolean()
            : this(false)
        {
        }

        public AtomicBoolean(bool initialValue)
        {
            Value = initialValue;
        }

        /// <summary>
        ///     Provides (non-thread-safe) access to the backing value
        /// </summary>
        public bool Value
        {
            get { return _zeroOrOne == TrueValue; }
            set { _zeroOrOne = value ? TrueValue : FalseValue; }
        }

        /// <summary>
        ///     Attempt changing the backing value from true to false.
        /// </summary>
        /// <returns>Whether the value was (atomically) changed from false to true.</returns>
        public bool FalseToTrue()
        {
            return SetWhen(true, false);
        }

        /// <summary>
        ///     Attempt changing the backing value from false to true.
        /// </summary>
        /// <returns>Whether the value was (atomically) changed from true to false.</returns>
        public bool TrueToFalse()
        {
            return SetWhen(false, true);
        }

        /// <summary>
        ///     Attempt changing from "whenValue" to "setToValue".
        ///     Fails if this.Value is not "whenValue".
        /// </summary>
        /// <param name="setToValue"></param>
        /// <param name="whenValue"></param>
        /// <returns></returns>
        public bool SetWhen(bool setToValue, bool whenValue)
        {
            var comparand = whenValue ? TrueValue : FalseValue;
            var result = Interlocked.CompareExchange(ref _zeroOrOne, setToValue ? TrueValue : FalseValue, comparand);
            var originalValue = result == TrueValue;
            return originalValue == whenValue;
        }
    }
}