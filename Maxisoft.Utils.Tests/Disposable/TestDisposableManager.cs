using System;
using Maxisoft.Utils.Disposable;
using Moq;
using Xunit;

namespace Maxisoft.Utils.Tests.Disposable
{
    public class TestDisposableManager
    {
        [Fact]
        public void TestDisposableManagerBasic()
        {
            var dm = new DisposableManager();
            var disposable = new Mock<IDisposable>();
            dm.LinkDisposable(disposable.Object);
            
        }
    }
}