using System;

namespace Maxisoft.Utils.Disposable
{
    public interface IDisposableManager : IDisposable
    {
        void LinkDisposable(IDisposable disposable);
        void LinkDisposable(WeakReference<IDisposable> disposable);
        void UnlinkDisposable(IDisposable disposable);
        void CleanupLinkedDisposable();
    }
}