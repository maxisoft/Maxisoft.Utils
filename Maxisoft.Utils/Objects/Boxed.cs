using System;
using System.Collections.Generic;

namespace Maxisoft.Utils.Objects
{
    public sealed class Boxed<T> : IEquatable<Boxed<T>>, IComparable<Boxed<T>>, IComparable
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

        public int CompareTo(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return 1;
            }

            if (ReferenceEquals(this, obj))
            {
                return 0;
            }

            return obj is Boxed<T> other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(Boxed<T>)}");
        }

        public int CompareTo(Boxed<T>? other)
        {
            return other is { } ? Comparer<T>.Default.Compare(_value, other._value) : 1;
        }


        public bool Equals(Boxed<T>? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return EqualityComparer<T>.Default.Equals(_value, other._value);
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

        public bool IsNull()
        {
            return ReferenceEquals(null, _value);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is Boxed<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return typeof(Boxed<T>).GetHashCode() ^ EqualityComparer<T>.Default.GetHashCode(_value);
            }
        }

        public static bool operator ==(Boxed<T>? left, Boxed<T>? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Boxed<T>? left, Boxed<T>? right)
        {
            return !Equals(left, right);
        }

        public static bool operator <(Boxed<T>? left, Boxed<T>? right)
        {
            return Comparer<Boxed<T>>.Default.Compare(left!, right!) < 0;
        }

        public static bool operator >(Boxed<T>? left, Boxed<T>? right)
        {
            return Comparer<Boxed<T>>.Default.Compare(left!, right!) > 0;
        }

        public static bool operator <=(Boxed<T>? left, Boxed<T>? right)
        {
            return Comparer<Boxed<T>>.Default.Compare(left!, right!) <= 0;
        }

        public static bool operator >=(Boxed<T>? left, Boxed<T>? right)
        {
            return Comparer<Boxed<T>>.Default.Compare(left!, right!) >= 0;
        }
    }
}