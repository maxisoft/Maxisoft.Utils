using System;
using System.Collections.Generic;
using System.Linq;

namespace Maxisoft.Utils.Disposable
{
    public class DisposableManager : IDisposableManager
    {
        private readonly LinkedList<OptinalWeakDisposable> _linkedDisposable;

        public DisposableManager() : this(new LinkedList<IDisposable>())
        {
        }

        public DisposableManager(IEnumerable<IDisposable> disposables) : this(new LinkedList<IDisposable>())
        {
            foreach (var disposable in disposables)
            {
                LinkDisposable(disposable);
            }
        }

        protected DisposableManager(LinkedList<IDisposable> collection) : this(new LinkedList<OptinalWeakDisposable>())
        {
            foreach (var disposable in collection)
            {
                LinkDisposable(disposable);
            }
        }

        protected DisposableManager(LinkedList<OptinalWeakDisposable> linkedDisposable)
        {
            _linkedDisposable = linkedDisposable;
        }

        public void LinkDisposable(IDisposable disposable)
        {
            if (ReferenceEquals(disposable, this)) return;
            lock (_linkedDisposable)
            {
                CleanupLinkedDisposable();
                _linkedDisposable.AddLast(new OptinalWeakDisposable(disposable));
            }
        }
        
        public void LinkDisposable(WeakReference<IDisposable> disposable)
        {
            if (!disposable.TryGetTarget(out var tmp) || ReferenceEquals(tmp, this)) return;
            lock (_linkedDisposable)
            {
                CleanupLinkedDisposable();
                _linkedDisposable.AddLast(new OptinalWeakDisposable(disposable));
            }
        }

        public void UnlinkDisposable(IDisposable disposable)
        {
            if (ReferenceEquals(disposable, this)) return;
            lock (_linkedDisposable)
            {
                CleanupLinkedDisposable();
                _linkedDisposable.RemoveAll(d => d == disposable);
            }
        }

        public void CleanupLinkedDisposable()
        {
            lock (_linkedDisposable)
            {
                _linkedDisposable.RemoveAll(d => !d.IsValid());
            }
        }


        protected virtual void Dispose(bool disposing)
        {
            lock (_linkedDisposable)
            {
                foreach (var disposable in _linkedDisposable.ReversedIterator())
                {
                    disposable.Dispose();
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