using System;
using System.IO;
using Xunit;

namespace Maxisoft.Utils.Tests
{
    public class TemporaryDirectoryTests : IDisposable
    {
        [Fact]
        public void TestTemporaryDirectory()
        {
            var result = new TemporaryDirectory();
            Assert.NotNull(result);
            Assert.NotEmpty(result.Path);
            Assert.True(Directory.Exists(result.Path));
            result.Dispose();
            Assert.False(Directory.Exists(result.Path));
        }


        [Fact]
        public void TestTemporaryDirectory_Dispose2x()
        {
            var result = new TemporaryDirectory();
            result.Dispose();
            Assert.Throws<ObjectDisposedException>(() => result.Dispose());
        }

        public void Dispose()
        {
        }
    }
}