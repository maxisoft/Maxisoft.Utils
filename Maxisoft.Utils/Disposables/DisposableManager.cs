using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Maxisoft.Utils.Collections.LinkedLists;

namespace Maxisoft.Utils.Disposables
{
    public class DisposableManager : IDisposableManager, IReadOnlyCollection<OptionalWeakDisposable>
    {
        internal readonly LinkedList<OptionalWeakDisposable> LinkedDisposables;
        private DisposableManagerBehavior _behavior = DisposableManagerBehavior.Default;
        private LinkedListNode<OptionalWeakDisposable>? _p;

        public DisposableManager() : this(new LinkedList<OptionalWeakDisposable>())
        {
        }

        public DisposableManager(IEnumerable<IDisposable> disposables) : this()
        {
            foreach (var disposable in disposables)
            {
                LinkDisposable(disposable);
            }
        }

        public DisposableManager(params IDisposable[] disposables) : this((IEnumerable<IDisposable>) disposables)
        {
        }

        protected DisposableManager(LinkedList<OptionalWeakDisposable> linkedDisposables)
        {
            LinkedDisposables = linkedDisposables;
        }

        public bool AutoCleanup
        {
            get => _behavior.HasFlag(DisposableManagerBehavior.AutoCleanup);
            set
            {
                if (value)
                {
                    _behavior |= DisposableManagerBehavior.AutoCleanup;
                }
                else
                {
                    _behavior &= ~DisposableManagerBehavior.AutoCleanup;
                }
            }
        }

        public bool ClearOnDispose
        {
            get => _behavior.HasFlag(DisposableManagerBehavior.ClearOnDispose);
            set
            {
                if (value)
                {
                    _behavior |= DisposableManagerBehavior.ClearOnDispose;
                }
                else
                {
                    _behavior &= ~DisposableManagerBehavior.ClearOnDispose;
                }
            }
        }

        public bool DisposeOnDeletion
        {
            get => _behavior.HasFlag(DisposableManagerBehavior.DisposeOnDeletion);
            set
            {
                if (value)
                {
                    _behavior |= DisposableManagerBehavior.DisposeOnDeletion;
                }
                else
                {
                    _behavior &= ~DisposableManagerBehavior.DisposeOnDeletion;
                }
            }
        }

        public void LinkDisposable(IDisposable disposable)
        {
            if (ReferenceEquals(disposable, this))
            {
                return;
            }

            lock (LinkedDisposables)
            {
                MaintainDisposablesNoLock();
                LinkedDisposables.AddLast(new OptionalWeakDisposable(disposable));
            }
        }

        public void UnlinkDisposable(IDisposable disposable)
        {
            if (ReferenceEquals(disposable, this))
            {
                return;
            }

            lock (LinkedDisposables)
            {
                MaintainDisposablesNoLock();
                LinkedDisposables.RemoveAll(d => d == disposable);
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        IEnumerator<OptionalWeakDisposable> IEnumerable<OptionalWeakDisposable>.GetEnumerator()
        {
            return LinkedDisposables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return LinkedDisposables.GetEnumerator();
        }

        int IReadOnlyCollection<OptionalWeakDisposable>.Count => LinkedDisposables.Count;

        public void LinkDisposable(WeakReference<IDisposable> disposable)
        {
            if (!disposable.TryGetTarget(out var tmp) || ReferenceEquals(tmp, this))
            {
                return;
            }

            lock (LinkedDisposables)
            {
                MaintainDisposablesNoLock();
                LinkedDisposables.AddLast(new OptionalWeakDisposable(disposable));
            }
        }


        public void LinkDisposableAsWeak<T>(T disposable) where T : class, IDisposable
        {
            LinkDisposable(new WeakReference<IDisposable>(disposable));
        }

        public bool ContainsDisposable(IDisposable disposable)
        {
            return LinkedDisposables.Any(d => d == disposable);
        }

        public void UnlinkAll()
        {
            lock (LinkedDisposables)
            {
                _p = null;
                LinkedDisposables.Clear();
            }
        }

        public void CleanupLinkedDisposable()
        {
            lock (LinkedDisposables)
            {
                LinkedDisposables.RemoveAll(d => !d.IsValid());
                _p = null;
            }
        }

        private void MaintainDisposablesNoLock()
        {
            if (!AutoCleanup)
            {
                return;
            }

            _p ??= LinkedDisposables.First;

            var c = LinkedDisposables.Count > 2 ? Math2.Log2(LinkedDisposables.Count) : 1;
            while (_p is {} && c-- > 0)
            {
                if (!_p.Value.IsValid())
                {
                    LinkedDisposables.Remove(_p);
                }

                _p = _p.Next;
            }
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposing && !DisposeOnDeletion)
            {
                return;
            }

            lock (LinkedDisposables)
            {
                foreach (var disposable in LinkedDisposables.ReversedIterator())
                {
                    disposable.Dispose();
                }

                _p = null;
                if (ClearOnDispose)
                {
                    LinkedDisposables.Clear();
                }
            }
        }

        ~DisposableManager()
        {
            Dispose(false);
        }
    }
}