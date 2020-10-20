using System;

namespace Maxisoft.Utils.Disposables
{
    [Flags]
    internal enum DisposableManagerBehavior : byte
    {
        AutoCleanup = 1 << 0,
        DisposeOnDeletion = 1 << 1,
        ClearOnDispose = 1 << 2,
        Default = AutoCleanup | DisposeOnDeletion | ClearOnDispose
    }
}