﻿using System;

namespace Maxisoft.Utils.Disposable
{
    public readonly struct OptionalWeakDisposable : IDisposable, IEquatable<OptionalWeakDisposable>, IEquatable<IDisposable>
    {
        internal readonly IDisposable? HardRef;

        internal readonly WeakReference<IDisposable>? WeakReference;

        public OptionalWeakDisposable(IDisposable hardRef)
        {
            HardRef = hardRef;
            WeakReference = null;
        }

        public OptionalWeakDisposable(WeakReference<IDisposable> weakReference)
        {
            HardRef = null;
            WeakReference = weakReference;
        }

        public void Dispose()
        {
            HardRef?.Dispose();
            if (!ReferenceEquals(null, WeakReference) && WeakReference.TryGetTarget(out var d))
            {
                d.Dispose();
            }
        }
            
        public bool IsValid()
        {
            if (!ReferenceEquals(null, WeakReference))
            {
                return WeakReference.TryGetTarget(out _);
            }

            return !ReferenceEquals(null, HardRef);
        }

        public static bool operator ==(OptionalWeakDisposable left, OptionalWeakDisposable right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(OptionalWeakDisposable left, OptionalWeakDisposable right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(OptionalWeakDisposable left, IDisposable right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(OptionalWeakDisposable left, IDisposable right)
        {
            return !left.Equals(right);
        }

        public bool Equals(OptionalWeakDisposable other)
        {
            return Equals(HardRef, other.HardRef) && Equals(WeakReference, other.WeakReference);
        }

        public bool Equals(IDisposable other)
        {
            if (other is OptionalWeakDisposable disposable)
            {
                return Equals(disposable);
            }

            return ReferenceEquals(other, HardRef) ||
                   WeakReference is {} && WeakReference.TryGetTarget(out var d) && ReferenceEquals(other, d);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj is OptionalWeakDisposable disposable)
            {
                return Equals(disposable);
            }
            return obj is IDisposable && Equals((IDisposable) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((HardRef != null ? HardRef.GetHashCode() : 0) * 397) ^ (WeakReference != null ? WeakReference.GetHashCode() : 0);
            }
        }
    }
}