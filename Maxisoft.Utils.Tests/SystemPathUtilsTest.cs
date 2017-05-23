using System.IO;
using Xunit;

namespace Maxisoft.Utils.Tests
{
    public class SystemPathUtilsTest
    {
        [Fact]
        public void TestPathAddBackslash()
        {
            Assert.Equal("c:\\windows\\system32\\", SystemPathUtils.PathAddBackslash("c:\\windows\\system32"));
            Assert.Equal("c:\\windows\\system32\\", SystemPathUtils.PathAddBackslash("c:\\windows\\system32\\"));
            Assert.Equal("/var/test/", SystemPathUtils.PathAddBackslash("/var/test"));
            Assert.Equal("/var\\test" + Path.AltDirectorySeparatorChar, SystemPathUtils.PathAddBackslash("/var\\test"));
            Assert.Equal("/var/test/", SystemPathUtils.PathAddBackslash("/var/test/"));
            Assert.Equal("/", SystemPathUtils.PathAddBackslash("/"));
            Assert.Equal("test" + Path.DirectorySeparatorChar, SystemPathUtils.PathAddBackslash("test"));
            Assert.Equal("\\", SystemPathUtils.PathAddBackslash("\\"));
            Assert.Equal(Path.DirectorySeparatorChar.ToString(), SystemPathUtils.PathAddBackslash(""));
        }
        
        [Fact]
        public void TestGetTemporaryDirectory()
        {
            var result = SystemPathUtils.GetTemporaryDirectory();
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(Directory.Exists(result));
        }
        
        [Fact]
        public void TestPathSlashToUnix()
        {
            Assert.Equal("c:/windows/system32", SystemPathUtils.PathSlashToUnix("c:\\windows\\system32"));
            Assert.Equal("c:/windows/system32/", SystemPathUtils.PathSlashToUnix("c:\\windows\\system32\\"));
            Assert.Equal("/var/test", SystemPathUtils.PathSlashToUnix("/var/test"));
            Assert.Equal("/var/test", SystemPathUtils.PathSlashToUnix("/var\\test"));
            Assert.Equal("/var/test/", SystemPathUtils.PathSlashToUnix("/var/test/"));
            Assert.Equal("/", SystemPathUtils.PathSlashToUnix("/"));
            Assert.Equal("test", SystemPathUtils.PathSlashToUnix("test"));
            Assert.Equal("/", SystemPathUtils.PathSlashToUnix("\\"));
            Assert.Equal("", SystemPathUtils.PathSlashToUnix(""));
        }
    }
}