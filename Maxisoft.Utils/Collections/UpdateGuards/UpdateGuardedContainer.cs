using System;
using System.Runtime.CompilerServices;

namespace Maxisoft.Utils.Collections.UpdateGuards
{

    public class BaseUpdateGuardedContainer : IUpdateGuarded
    {
        private int _version;

        protected internal ref int Version
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _version;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ref int IUpdateGuarded.GetInternalVersionCounter()
        {
            return ref Version;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal void CheckForConcurrentModification(int previousVersion)
        {
            if (previousVersion == _version)
            {
                return;
            }

            throw new InvalidOperationException("Concurrent modification detected");;
        }
    }
    
    [Serializable]
    public class UpdateGuardedContainer : IUpdateGuarded
    {
        private int _version;

        protected internal int Version
        {
            get => _version;
            set => _version = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ref int IUpdateGuarded.GetInternalVersionCounter()
        {
            return ref GetInternalVersionCounter();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal ref int GetInternalVersionCounter()
        {
            return ref _version;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal static UpdateGuard<TUpdateGuardedContainer> CreateGuard<TUpdateGuardedContainer>(TUpdateGuardedContainer container, bool increment = false) where TUpdateGuardedContainer : UpdateGuardedContainer
        {
            return new UpdateGuard<TUpdateGuardedContainer>(container) {PostIncrementVersionCounter = increment};
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal UpdateGuard<UpdateGuardedContainer> CreateGuard(bool increment = false)
        {
            return CreateGuard<UpdateGuardedContainer>(this, increment);
        }
    }
}