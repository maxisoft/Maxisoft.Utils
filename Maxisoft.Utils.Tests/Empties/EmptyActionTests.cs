using System;
using Maxisoft.Utils.Empties;
using Xunit;

namespace Maxisoft.Utils.Tests.Empties
{
    public class EmptyActionTests
    {
        [Fact]
        public void Test_Action()
        {
            Action f = new EmptyAction();
            f();
        }

        [Fact]
        public void Test_Action1()
        {
            Action<int> f = new EmptyAction<int>();
            f(0x2adc);
        }

        [Fact]
        public void Test_Action2()
        {
            Action<int, int> f = new EmptyAction<int, int>();
            f(0x1b9b, 0x44fe);
        }

        [Fact]
        public void Test_Action3()
        {
            Action<int, int, int> f = new EmptyAction<int, int, int>();
            f(0x20bb, 0x73ab, 0xc843);
        }

        [Fact]
        public void Test_Action4()
        {
            Action<int, int, int, int> f = new EmptyAction<int, int, int, int>();
            f(0x6b01, 0x73c5, 0x5fc, 0x5a5d);
        }

        [Fact]
        public void Test_Action5()
        {
            Action<int, int, int, int, int> f = new EmptyAction<int, int, int, int, int>();
            f(0x2520, 0x4963, 0x608a, 0x8ba3, 0xd3b9);
        }

        [Fact]
        public void Test_Action6()
        {
            Action<int, int, int, int, int, int> f = new EmptyAction<int, int, int, int, int, int>();
            f(0xea9f, 0x5b2, 0xb6cd, 0x94d5, 0xfee7, 0x81bf);
        }

        [Fact]
        public void Test_Action7()
        {
            Action<int, int, int, int, int, int, int> f = new EmptyAction<int, int, int, int, int, int, int>();
            f(0x124b, 0x60bc, 0x19ac, 0x9ca5, 0x1379, 0x1a3f, 0x44e6);
        }

        [Fact]
        public void Test_Action8()
        {
            Action<int, int, int, int, int, int, int, int>
                f = new EmptyAction<int, int, int, int, int, int, int, int>();
            f(0x5aa6, 0xf456, 0x4296, 0x520, 0x3e76, 0xc668, 0x8b46, 0xbdc);
        }

        [Fact]
        public void Test_Action9()
        {
            Action<int, int, int, int, int, int, int, int, int> f =
                new EmptyAction<int, int, int, int, int, int, int, int, int>();
            f(0x3d92, 0x58c2, 0xe249, 0x2a81, 0x9e56, 0xd83e, 0x3b77, 0x5bb1, 0x68cb);
        }

        [Fact]
        public void Test_Action10()
        {
            Action<int, int, int, int, int, int, int, int, int, int> f =
                new EmptyAction<int, int, int, int, int, int, int, int, int, int>();
            f(0x2318, 0x23bc, 0x373a, 0xba7a, 0xd1c2, 0xee9a, 0x2d5a, 0x1b19, 0x84fa, 0xe321);
        }

        [Fact]
        public void Test_Action11()
        {
            Action<int, int, int, int, int, int, int, int, int, int, int> f =
                new EmptyAction<int, int, int, int, int, int, int, int, int, int, int>();
            f(0x2355, 0xfab9, 0xe378, 0x2a51, 0x67fe, 0xc641, 0x1a49, 0x2173, 0x97a5, 0xb5df, 0xfea4);
        }

        [Fact]
        public void Test_Action12()
        {
            Action<int, int, int, int, int, int, int, int, int, int, int, int> f =
                new EmptyAction<int, int, int, int, int, int, int, int, int, int, int, int>();
            f(0x7a26, 0x2529, 0xbd02, 0x43f4, 0x1673, 0x9b9, 0xd076, 0xf090, 0xb97a, 0x5c74, 0xc386, 0x3d7a);
        }

        [Fact]
        public void Test_Action13()
        {
            Action<int, int, int, int, int, int, int, int, int, int, int, int, int> f =
                new EmptyAction<int, int, int, int, int, int, int, int, int, int, int, int, int>();
            f(0x435d, 0x4700, 0xead6, 0x88b2, 0xe3d0, 0xa946, 0x93af, 0x3078, 0xb89a, 0x7df8, 0xd6c0, 0xf776, 0x6fbb);
        }

        [Fact]
        public void Test_Action14()
        {
            Action<int, int, int, int, int, int, int, int, int, int, int, int, int, int> f =
                new EmptyAction<int, int, int, int, int, int, int, int, int, int, int, int, int, int>();
            f(0xb64, 0x7813, 0xdf0f, 0xb6cb, 0x7074, 0xf1e7, 0x3d04, 0x6324, 0x3c3b, 0xc11c, 0x58fc, 0x533e, 0x6eea,
                0x9d54);
        }

        [Fact]
        public void Test_Action15()
        {
            Action<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int> f =
                new EmptyAction<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>();
            f(0x7801, 0x6788, 0x540f, 0x239d, 0xc487, 0x227e, 0x1990, 0xa9bb, 0x4f50, 0xdac9, 0x84e5, 0xa53, 0x4138,
                0xd660, 0x2603);
        }
    }
}