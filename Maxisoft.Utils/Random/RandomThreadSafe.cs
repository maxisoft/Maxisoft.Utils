namespace Maxisoft.Utils.Random
{
    public class RandomThreadSafe
    {
        private readonly System.Random _random;

        public RandomThreadSafe()
        {
            _random = new System.Random();
        }

        public RandomThreadSafe(int seed)
        {
            _random = new System.Random(seed);
        }

        public int Next()
        {
            lock (_random)
            {
                return _random.Next();
            }
        }

        public int Next(int minValue, int maxValue)
        {
            lock (_random)
                return _random.Next(minValue, maxValue);
        }

        public int Next(int maxValue)
        {
            lock (_random)
                return _random.Next(maxValue);
        }

        public double NextDouble()
        {
            lock (_random)
                return _random.NextDouble();
        }

        public void NextBytes(byte[] buffer)
        {
            lock (_random)
                _random.NextBytes(buffer);
        }

        public override string ToString()
        {
            lock (_random)
                return _random.ToString();
        }

        public override bool Equals(object obj)
        {
            lock (_random)
                return _random.Equals(obj);
        }

        public override int GetHashCode()
        {
            lock (_random)
                return _random.GetHashCode();
        }
    }
}