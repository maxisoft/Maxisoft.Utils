using System;
using Maxisoft.Utils.Disposables;
using Maxisoft.Utils.Empties;
using Moq;
using Xunit;

namespace Maxisoft.Utils.Tests.Disposables
{
    public class OptionalWeakDisposableTests
    {
        [Fact]
        public void Test_Dispose()
        {
            // hard ref
            {
                var disposableMock = new Mock<DisposableManagerTests.DisposableTracker> {CallBase = true};
                var optionalWeakDisposable = new OptionalWeakDisposable(disposableMock.Object);
                Assert.NotNull(optionalWeakDisposable.HardRef);
                Assert.Null(optionalWeakDisposable.WeakReference);
                Assert.True(optionalWeakDisposable.IsValid());

                Assert.True(optionalWeakDisposable != new EmptyDisposable());
                Assert.False(optionalWeakDisposable != disposableMock.Object);
                Assert.True(optionalWeakDisposable == disposableMock.Object);
                // ReSharper disable once EqualExpressionComparison
                Assert.True(optionalWeakDisposable == optionalWeakDisposable);
                Assert.False(optionalWeakDisposable == new OptionalWeakDisposable());
                Assert.True(optionalWeakDisposable != new OptionalWeakDisposable());

                Assert.Equal((IDisposable) optionalWeakDisposable, optionalWeakDisposable);

                Assert.True(optionalWeakDisposable.Equals((object) optionalWeakDisposable));
                Assert.False(optionalWeakDisposable.Equals(((object) null)!));
                // ReSharper disable once SuspiciousTypeConversion.Global
                Assert.False(optionalWeakDisposable.Equals((object) new EmptyDisposable()));

                Assert.Equal(disposableMock.Object.GetHashCode(), optionalWeakDisposable.GetHashCode());

                disposableMock.Verify(mock => mock.Dispose(), Times.Never);
                disposableMock.Invocations.Clear();
                optionalWeakDisposable.Dispose();

                disposableMock.Verify(mock => mock.Dispose());
                disposableMock.VerifyNoOtherCalls();
            }

            // weak ref
            {
                var disposableMock = new Mock<DisposableManagerTests.DisposableTracker> {CallBase = true};
                var optionalWeakDisposable =
                    new OptionalWeakDisposable(new WeakReference<IDisposable>(disposableMock.Object));
                Assert.Null(optionalWeakDisposable.HardRef);
                Assert.NotNull(optionalWeakDisposable.WeakReference);
                Assert.True(optionalWeakDisposable.IsValid());

                Assert.True(optionalWeakDisposable != new EmptyDisposable());
                Assert.False(optionalWeakDisposable != disposableMock.Object);
                Assert.True(optionalWeakDisposable == disposableMock.Object);
                // ReSharper disable once EqualExpressionComparison
                Assert.True(optionalWeakDisposable == optionalWeakDisposable);
                Assert.False(optionalWeakDisposable == new OptionalWeakDisposable());
                Assert.True(optionalWeakDisposable != new OptionalWeakDisposable());

                Assert.Equal((IDisposable) optionalWeakDisposable, optionalWeakDisposable);

                Assert.True(optionalWeakDisposable.Equals((object) optionalWeakDisposable));
                Assert.False(optionalWeakDisposable.Equals(((object) null)!));
                // ReSharper disable once SuspiciousTypeConversion.Global
                Assert.False(optionalWeakDisposable.Equals((object) new EmptyDisposable()));

                Assert.NotEqual(0, optionalWeakDisposable.GetHashCode());

                disposableMock.Verify(mock => mock.Dispose(), Times.Never);
                disposableMock.Invocations.Clear();
                optionalWeakDisposable.Dispose();
            }
        }
    }
}