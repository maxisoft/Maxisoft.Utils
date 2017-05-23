using System;
using System.Collections.Generic;
using System.Linq;
using Maxisoft.Utils.Disposable;
using Moq;
using Xunit;

namespace Maxisoft.Utils.Tests.Disposable
{
    public class TestDisposableManager
    {
        [Fact]
        public void TestDisposableManager_Basics()
        {
            var disposables = new Mock<LinkedList<WeakReference<IDisposable>>> {CallBase = true};
            var dm = new TDisposableManager(disposables.Object);
            var disposable = AddDisposable(dm);
            
            dm.Dispose();
            
            var count = 0;
            //wait the for the GC to cleanup thing
            //try hard mode
            while (disposables.Object.Count != 0 && count++ < 500)
            {
                dm.CleanupLinkedDisposable();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            Assert.True(disposables.Object.Count == 0, "there's linked disposables left"); 
            disposable.Verify(d => d.Dispose(), Times.Once);
        }
        
        [Fact]
        public void TestDisposableManager_WeakRef()
        {
            var disposables = new Mock<LinkedList<WeakReference<IDisposable>>> {CallBase = true};
            var dm = new TDisposableManager(disposables.Object);
            //add a disposable via another function scope
            {
                void Action() => dm.LinkDisposable(new EmptyDisposable());
                Action();
            }

            var count = 0;
            //wait the for the GC to cleanup thing, including the previously added EmptyDisposable weak ref
            //try hard mode
            while (disposables.Object.Count != 0 && count++ < 500)
            {
                dm.CleanupLinkedDisposable();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            Assert.False(count >= 500);
            Assert.Null(disposables.Object.FirstOrDefault(reference => reference.TryGetTarget(out IDisposable tmp) && tmp is EmptyDisposable));
        }

        private static Mock<IDisposable> AddDisposable(IDisposableManager disposableManager)
        {
            var disposable = new Mock<IDisposable>();
            disposableManager.LinkDisposable(disposable.Object);
            return disposable;
        }

        private class TDisposableManager : DisposableManager
        {
            protected internal TDisposableManager(LinkedList<WeakReference<IDisposable>> collection) : base(collection)
            {
            }
        }

        private class EmptyDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}