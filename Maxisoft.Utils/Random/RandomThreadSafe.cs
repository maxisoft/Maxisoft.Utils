namespace Maxisoft.Utils.Random
{
    public class RandomThreadSafe : System.Random
    {
        private readonly object _lock = new object();

        public RandomThreadSafe()
        {
        }

        public RandomThreadSafe(int seed) : base(seed)
        {
        }

        public override int Next()
        {
            lock (_lock)
            {
                return base.Next();
            }
        }

        public override int Next(int minValue, int maxValue)
        {
            lock (_lock)
                return base.Next(minValue, maxValue);
        }

        public override int Next(int maxValue)
        {
            lock (_lock)
                return base.Next(maxValue);
        }

        public override double NextDouble()
        {
            lock (_lock)
                return base.NextDouble();
        }

        public override void NextBytes(byte[] buffer)
        {
            lock (_lock)
                base.NextBytes(buffer);
        }

        public override string ToString()
        {
            lock (_lock)
                return base.ToString();
        }
    }
}