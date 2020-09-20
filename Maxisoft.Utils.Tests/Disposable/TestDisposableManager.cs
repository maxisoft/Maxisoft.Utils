using System;
using System.Collections.Generic;
using System.Linq;
using Maxisoft.Utils.Disposable;
using Moq;
using Optional.Collections;
using Xunit;

namespace Maxisoft.Utils.Tests.Disposable
{
    public class TestDisposableManager
    {
        [Fact]
        public void TestDisposableManager_Basics()
        {
            var disposables = new Mock<LinkedList<OptionalWeakDisposable>> {CallBase = true};
            var dm = new TDisposableManager(disposables.Object);
            var disposable = AddDisposable(dm);
            
            dm.Dispose();
            // hold the weak ref target object
            var weakdisposable = AddWeakDisposable(dm);
            
            var count = 0;
            //wait the for the GC to cleanup thing
            //try hard mode
            while (disposables.Object.Count > 1 && count++ < 500)
            {
                dm.CleanupLinkedDisposable();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            dm.CleanupLinkedDisposable();
            Assert.True(disposables.Object.Count == 1, "there's a weak linked disposables left");
            Assert.True(disposables.Object.FirstOrDefault(r => r == disposable.Object) == default(OptionalWeakDisposable), "there's hard linked disposables left");
            Assert.True(disposables.Object.FirstOrDefault(r => r == weakdisposable.Object) != default(OptionalWeakDisposable), "there's no weak linked disposables left");
            disposable.Verify(d => d.Dispose(), Times.Once);
            weakdisposable.Verify(d => d.Dispose(), Times.Never);
        }
        
        [Fact]
        public void TestDisposableManager_WeakRef()
        {
            var disposables = new Mock<LinkedList<OptionalWeakDisposable>> {CallBase = true};
            var dm = new TDisposableManager(disposables.Object);
            //add a disposable via another function scope
            {
                void Action() => dm.LinkDisposable(new WeakReference<IDisposable>(new EmptyDisposable()));
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
            Assert.True(disposables.Object.FirstOrDefault(reference => reference.WeakReference != null && reference.WeakReference.TryGetTarget(out var tmp) && tmp is EmptyDisposable) == default(OptionalWeakDisposable));
        }

        private static Mock<IDisposable> AddDisposable(IDisposableManager disposableManager)
        {
            var disposable = new Mock<IDisposable>();
            disposableManager.LinkDisposable(disposable.Object);
            return disposable;
        }
        
        private static Mock<IDisposable> AddWeakDisposable(IDisposableManager disposableManager)
        {
            var disposable = new Mock<IDisposable>();
            disposableManager.LinkDisposable(new WeakReference<IDisposable>(disposable.Object));
            return disposable;
        }

        private class TDisposableManager : DisposableManager
        {
            protected internal TDisposableManager(LinkedList<OptionalWeakDisposable> collection) : base(collection)
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