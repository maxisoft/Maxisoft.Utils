using System;
using Maxisoft.Utils.Objects;
using Xunit;

namespace Maxisoft.Utils.Tests.Objects
{
    public class PointerHelperTests
    {
        [Fact]
        public unsafe void Test_GetPointer()
        {
            var obj = new object();
            var @ref = __makeref(obj);
            var expected = **(IntPtr**) (&@ref);

            var actual = PointerHelper.GetPointer(obj);
            Assert.NotEqual(IntPtr.Zero, actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public unsafe void Test_GetPointer_Null()
        {
            object obj = null;
            var @ref = __makeref(obj);
            var expected = **(IntPtr**) (&@ref);

            var actual = PointerHelper.GetPointer(obj);
            Assert.Equal(IntPtr.Zero, actual);
            Assert.Equal(expected, actual);
        }
    }
}