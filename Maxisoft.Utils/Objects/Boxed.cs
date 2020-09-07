namespace Maxisoft.Utils.Objects
{
    public sealed class Boxed<T>
    {
        private T _value;

        public Boxed(in T value)
        {
            _value = value;
        }

        public T Value
        {
            get => _value;
            set => _value = value;
        }

        public static implicit operator T(Boxed<T> boxed)
        {
            return boxed.Value;
        }
        
        public static implicit operator Boxed<T>(T value)
        {
            return new Boxed<T>(value);
        }

        public ref T Ref()
        {
            return ref _value;
        }

        public bool IsNull() => ReferenceEquals(null, _value);
    }
}