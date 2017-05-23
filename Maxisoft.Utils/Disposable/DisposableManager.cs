using System;
using System.Collections.Generic;
using System.Linq;

namespace Maxisoft.Utils.Disposable
{
    public class DisposableManager : IDisposableManager
    {
        private readonly LinkedList<WeakReference<IDisposable>> _linkedDisposable =
            new LinkedList<WeakReference<IDisposable>>();

        public void LinkDisposable(IDisposable disposable)
        {
            if (ReferenceEquals(disposable, this)) return;
            lock (_linkedDisposable)
            {
                CleanupLinkedDisposable();
                _linkedDisposable.AddLast(new WeakReference<IDisposable>(disposable));
            }
        }
        
        public void UnlinkDisposable(IDisposable disposable)
        {
            if (ReferenceEquals(disposable, this)) return;
            lock (_linkedDisposable)
            {
                CleanupLinkedDisposable();
                _linkedDisposable.RemoveAll(reference => reference.TryGetTarget(out IDisposable tmp) && ReferenceEquals(tmp, disposable));
            }
        }

        public void CleanupLinkedDisposable()
        {
            lock (_linkedDisposable)
            {
                _linkedDisposable.RemoveAll(reference => !reference.TryGetTarget(out IDisposable tmp) ||
                                                         ReferenceEquals(null, tmp));
            }
        }


        protected virtual void Dispose(bool disposing)
        {
            lock (_linkedDisposable)
            {
                foreach (var wearref in _linkedDisposable.Reverse())
                {
                    if (wearref.TryGetTarget(out IDisposable disposable))
                    {
                        disposable?.Dispose();
                    }
                }
                _linkedDisposable.Clear();
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DisposableManager()
        {
            Dispose(false);
        }
    }
}