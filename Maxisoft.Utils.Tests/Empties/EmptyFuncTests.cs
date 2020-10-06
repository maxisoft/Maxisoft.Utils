using System;
using Maxisoft.Utils.Empties;
using Xunit;

namespace Maxisoft.Utils.Tests.Empties
{
    public class EmptyFuncTests
    {
        [Fact]
        public void Test_Func0()
        {
            Func<object> f = new EmptyFunc<object>();
            Assert.Equal(default, f());
        }

        [Fact]
        public void Test_Func1()
        {
            Func<int, object> f = new EmptyFunc<int, object>();
            Assert.Equal(default, f(0xff8f));
        }

        [Fact]
        public void Test_Func2()
        {
            Func<int, int, object> f = new EmptyFunc<int, int, object>();
            Assert.Equal(default, f(0xf0d2, 0xc09));
        }

        [Fact]
        public void Test_Func3()
        {
            Func<int, int, int, object> f = new EmptyFunc<int, int, int, object>();
            Assert.Equal(default, f(0x4ed6, 0x58cb, 0x6fad));
        }

        [Fact]
        public void Test_Func4()
        {
            Func<int, int, int, int, object> f = new EmptyFunc<int, int, int, int, object>();
            Assert.Equal(default, f(0x1480, 0x7670, 0xe2fe, 0x5be9));
        }

        [Fact]
        public void Test_Func5()
        {
            Func<int, int, int, int, int, object> f = new EmptyFunc<int, int, int, int, int, object>();
            Assert.Equal(default, f(0xd13d, 0xfaa6, 0xd939, 0xf84d, 0xe23e));
        }

        [Fact]
        public void Test_Func6()
        {
            Func<int, int, int, int, int, int, object> f = new EmptyFunc<int, int, int, int, int, int, object>();
            Assert.Equal(default, f(0x8617, 0x4ccb, 0x4662, 0x8283, 0x6339, 0x4d11));
        }

        [Fact]
        public void Test_Func7()
        {
            Func<int, int, int, int, int, int, int, object> f =
                new EmptyFunc<int, int, int, int, int, int, int, object>();
            Assert.Equal(default, f(0xca68, 0x4ebf, 0x12c4, 0x40cf, 0x742, 0x175, 0xb019));
        }

        [Fact]
        public void Test_Func8()
        {
            Func<int, int, int, int, int, int, int, int, object> f =
                new EmptyFunc<int, int, int, int, int, int, int, int, object>();
            Assert.Equal(default, f(0xd701, 0x6414, 0x9c41, 0x3916, 0x901f, 0x8e40, 0x9d70, 0xb8cd));
        }

        [Fact]
        public void Test_Func9()
        {
            Func<int, int, int, int, int, int, int, int, int, object> f =
                new EmptyFunc<int, int, int, int, int, int, int, int, int, object>();
            Assert.Equal(default, f(0x59fd, 0x55da, 0x5552, 0x1af4, 0x2465, 0x6f1a, 0xcf66, 0x8a09, 0x6ee3));
        }

        [Fact]
        public void Test_Func10()
        {
            Func<int, int, int, int, int, int, int, int, int, int, object> f =
                new EmptyFunc<int, int, int, int, int, int, int, int, int, int, object>();
            Assert.Equal(default,
                f(0x983a, 0x73bd, 0xc0ec, 0x4f64, 0x550a, 0x5964, 0x4f36, 0x3a69, 0x5dfb, 0x9bfb));
        }

        [Fact]
        public void Test_Func11()
        {
            Func<int, int, int, int, int, int, int, int, int, int, int, object> f =
                new EmptyFunc<int, int, int, int, int, int, int, int, int, int,
                    int, object>();
            Assert.Equal(default,
                f(0xac46, 0x4b9d, 0x75a2, 0xc2f2, 0xd2e6, 0xf992, 0x685, 0x80fd, 0xd7d1, 0xc5f6, 0x8050));
        }

        [Fact]
        public void Test_Func12()
        {
            Func<int, int, int, int, int, int, int, int, int, int, int, int, object> f =
                new EmptyFunc<int, int, int, int, int, int, int, int, int,
                    int, int, int, object>();
            Assert.Equal(default,
                f(0x2d0a, 0xf13f, 0x400a, 0xa1e6, 0x6fc5, 0xbbde, 0x3854, 0xea0, 0x9ce6, 0x4623, 0x8b43, 0x1b86));
        }

        [Fact]
        public void Test_Func13()
        {
            Func<int, int, int, int, int, int, int, int, int, int, int, int, int, object> f =
                new EmptyFunc<int, int, int, int, int, int, int, int,
                    int, int, int, int, int, object>();
            Assert.Equal(default,
                f(0xfa03, 0xca58, 0x35fd, 0x16ab, 0xc76a, 0x6cf5, 0x6882, 0x7748, 0x78a2, 0x78d9, 0x1536, 0x776c,
                    0x63fc));
        }

        [Fact]
        public void Test_Func14()
        {
            Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int, object> f =
                new EmptyFunc<int, int, int, int, int, int, int,
                    int, int, int, int, int, int, int, object>();
            Assert.Equal(default,
                f(0x8231, 0x10bb, 0xf0fa, 0xbf42, 0x9dd3, 0xadde, 0x37a9, 0x2038, 0xee, 0x419c, 0x94e, 0xcba3, 0x9ed3,
                    0x4512));
        }

        [Fact]
        public void Test_Func15()
        {
            Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, object> f =
                new EmptyFunc<int, int, int, int, int, int,
                    int, int, int, int, int, int, int, int, int, object>();
            Assert.Equal(default,
                f(0x7433, 0xccf1, 0xa17b, 0x31df, 0x98d9, 0xdac0, 0x30e0, 0x65c1, 0xd6b1, 0x8a0e, 0xe0c, 0xc8c9, 0xc937,
                    0xd34f, 0x1944));
        }
    }
}