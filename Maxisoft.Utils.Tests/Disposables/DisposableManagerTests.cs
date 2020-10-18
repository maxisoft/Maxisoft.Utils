using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Maxisoft.Utils.Disposables;
using Maxisoft.Utils.Logic;
using Maxisoft.Utils.Random;
using Moq;
using Troschuetz.Random;
using Xunit;

namespace Maxisoft.Utils.Tests.Disposables
{
    public class DisposableManagerTests
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
            Assert.True(
                disposables.Object.FirstOrDefault(r => r == disposable.Object) == default,
                "there's hard linked disposables left");
            Assert.True(
                disposables.Object.FirstOrDefault(r => r == weakdisposable.Object) != default,
                "there's no weak linked disposables left");
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
                void Action()
                {
                    dm.LinkDisposable(new WeakReference<IDisposable>(new EmptyDisposable()));
                }

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
            Assert.True(disposables.Object.FirstOrDefault(reference =>
                reference.WeakReference != null && reference.WeakReference.TryGetTarget(out var tmp) &&
                tmp is EmptyDisposable) == default);
            Assert.False(dm.ContainsDisposable(dm));
        }


        [Theory]
        [ClassData(typeof(RandomSeedGenerator))]
        public void Test_Behavior_With_Fuzzing(int seed)
        {
            var disposables = new Mock<LinkedList<OptionalWeakDisposable>> {CallBase = true};
            var dm = new TDisposableManager(disposables.Object);
            var random = new TRandom(seed);
            const int limit = 64;
            dm.AutoCleanup = random.NextBoolean();

            var weakDisposableList = new List<WeakReference<IDisposable>>();
            var disposableList = new List<Mock<IDisposable>>();

            for (var i = 0; i < limit; i++)
            {
                var rnd = random.Next(3);

                if (rnd == 0)
                {
                    var disposable = AddDisposable(dm);
                    disposableList.Add(disposable);
                    weakDisposableList.Add(null);
                }
                else if (rnd == 1)
                {
                    var disposable = new WeakReference<IDisposable>(new DisposableTracker());
                    weakDisposableList.Add(disposable);
                    disposableList.Add(null);
                }
                else if (rnd == 2)
                {
                    if (!dm.Any())
                    {
                        continue;
                    }

                    var index = random.Next(dm.Count());
                    var d = dm.ToArray()[index];

                    if (!d.IsValid())
                    {
                        Assert.False(weakDisposableList[index].TryGetTarget(out _));
                        Assert.DoesNotContain(weakDisposableList, dd => dd.TryGetTarget(out var tmp) && d == tmp);
                    }
                    else
                    {
                        if (d.HardRef is {})
                        {
                            Assert.Contains(disposableList, dd => dd is {} && d == dd.Object);
                        }
                        else if (d.WeakReference.TryGetTarget(out var expected))
                        {
                            Assert.Contains(weakDisposableList,
                                dd => dd is {} && dd.TryGetTarget(out var tmp) && d == tmp);
                        }
                    }

                    if (d.HardRef is {})
                    {
                        dm.UnlinkDisposable(d.HardRef);
                        var ii = disposableList.FindIndex(m => m is {} && d == m.Object);
                        disposableList.RemoveAt(ii);
                    }
                    else if (d.WeakReference.TryGetTarget(out var tmp))
                    {
                        dm.UnlinkDisposable(tmp);
                        var ii = weakDisposableList.FindIndex(w =>
                            w is {} && w.TryGetTarget(out var tmp2) && ReferenceEquals(tmp, tmp2));
                        weakDisposableList.RemoveAt(ii);
                    }
                }
                else
                {
                    throw new InvalidOperationException($"{nameof(rnd)}={rnd} invalid value");
                }


                if (i % 8 == 0)
                {
                    if (random.NextBoolean())
                    {
                        var count = 0;
                        //wait the for the GC to cleanup things
                        //try hard mode
                        var prevCount = disposables.Object.Count;
                        while (prevCount == disposables.Object.Count && count++ < 6)
                        {
                            dm.CleanupLinkedDisposable();
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                        }
                    }


                    var disposablesArray = dm.ToArray();
                    var dispose = random.NextBoolean();
                    if (dispose)
                    {
                        dm.Dispose();
                    }

                    foreach (var disposable in disposablesArray)
                    {
                        if (disposable.HardRef is {})
                        {
                            var index = disposableList.FindIndex(m => m is {} && disposable == m.Object);
                            if (dispose)
                            {
                                disposableList[index].Verify(mock => mock.Dispose());
                            }
                            else
                            {
                                disposableList[index].Verify(mock => mock.Dispose(), Times.Never);
                            }
                        }
                        else if (disposable.WeakReference.TryGetTarget(out var tmp))
                        {
                            var index = weakDisposableList.FindIndex(w =>
                                w is {} && w.TryGetTarget(out var tmp2) && ReferenceEquals(tmp, tmp2));
                            weakDisposableList[index].TryGetTarget(out var tmp2);
                            Assert.IsType<DisposableTracker>(tmp2);
                            if (dispose)
                            {
                                Assert.True(((DisposableTracker) tmp2).Value.Value);
                            }
                            else
                            {
                                Assert.False(((DisposableTracker) tmp2).Value.Value);
                            }
                        }
                    }

                    foreach (var d in disposableList.Where(d => d is { }))
                    {
                        if (dispose)
                        {
                            d.Verify(mock => mock.Dispose());
                        }
                        else
                        {
                            d.Verify(mock => mock.Dispose(), Times.Never);
                        }
                    }

                    if (dispose)
                    {
                        weakDisposableList.Clear();
                        disposableList.Clear();
                    }
                }
            }
        }

        [Fact]
        public void TestDisposableManager_UnlinkDisposable()
        {
            var disposables = new Mock<LinkedList<OptionalWeakDisposable>> {CallBase = true};
            var dm = new TDisposableManager(disposables.Object);
            var disposable = AddDisposable(dm);

            Assert.True(dm.ContainsDisposable(disposable.Object));
            Assert.NotEmpty(dm);
            Assert.Single(dm);
            Assert.Single(disposables.Object);
            Assert.Equal(1, ((IReadOnlyCollection<OptionalWeakDisposable>) dm).Count);

            // try to remove non existing disposable
            {
                dm.UnlinkDisposable(new EmptyDisposable());
                Assert.Single(dm);
                Assert.Single(disposables.Object);
                Assert.Equal(1, ((IReadOnlyCollection<OptionalWeakDisposable>) dm).Count);
            }

            disposable.Verify(mock => mock.Dispose(), Times.Never);

            // remove the existing disposable
            {
                dm.UnlinkDisposable(disposable.Object);
                Assert.Empty(dm);
                Assert.Empty(disposables.Object);
                Assert.Equal(0, ((IReadOnlyCollection<OptionalWeakDisposable>) dm).Count);
            }

            disposable.Verify(mock => mock.Dispose(), Times.Never);
            dm.Dispose();
            disposable.Verify(mock => mock.Dispose(), Times.Never);
        }

        [Fact]
        public void TestDisposableManager_UnlinkAll()
        {
            var disposables = new Mock<LinkedList<OptionalWeakDisposable>> {CallBase = true};
            var dm = new TDisposableManager(disposables.Object);
            var disposable = AddDisposable(dm);

            Assert.True(dm.ContainsDisposable(disposable.Object));
            Assert.NotEmpty(dm);
            Assert.Single(dm);
            Assert.Single(disposables.Object);

            disposable.Verify(mock => mock.Dispose(), Times.Never);

            {
                dm.UnlinkAll();
                Assert.Empty(dm);
                Assert.Empty(disposables.Object);
            }

            disposable.Verify(mock => mock.Dispose(), Times.Never);
            dm.Dispose();
            disposable.Verify(mock => mock.Dispose(), Times.Never);
        }


        private static Mock<IDisposable> AddDisposable(IDisposableManager disposableManager)
        {
            var disposable = new Mock<IDisposable>();
            disposableManager.LinkDisposable(disposable.Object);
            return disposable;
        }

        private static Mock<IDisposable> AddWeakDisposable(DisposableManager disposableManager)
        {
            var disposable = new Mock<IDisposable>();
            disposableManager.LinkDisposableAsWeak(disposable.Object);
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


        internal class DisposableTracker : IDisposable
        {
            internal AtomicBoolean Value = new AtomicBoolean();

            public void Dispose()
            {
                Value.FalseToTrue();
            }
        }

        internal class RandomSeedGenerator : IEnumerable<object[]>
        {
            internal virtual RandomThreadSafe Random { get; } = new RandomThreadSafe();
            internal virtual int NumberOfGen => 4;

            public IEnumerator<object[]> GetEnumerator()
            {
                for (var i = 0; i < NumberOfGen; i++)
                {
                    yield return new object[] {Random.Next()};
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}