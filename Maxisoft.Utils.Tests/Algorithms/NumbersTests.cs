using System.Collections.Generic;
using Maxisoft.Utils.Algorithms;
using Maxisoft.Utils.Objects;
using Xunit;

namespace Maxisoft.Utils.Tests.Algorithms
{
    public class NumbersTests
    {
        [Fact]
        public void Test_Clamp_Float()
        {
            //test Nan Propagation
            {
                Assert.True(float.IsNaN(float.NaN.Clamp(1, 2)));
                Assert.True(float.IsNaN(((float) 1).Clamp(float.NaN, -1)));
                Assert.True(float.IsNaN(((float) 1).Clamp(float.NaN, 2)));
                Assert.True(float.IsNaN(((float) 1).Clamp(0, float.NaN)));
                Assert.True(float.IsNaN(((float) 1).Clamp(2, float.NaN)));
                Assert.True(float.IsNaN(float.NaN.Clamp(float.NegativeInfinity, float.PositiveInfinity)));
            }


            //test +Inf Propagation
            {
                Assert.Equal(2, float.PositiveInfinity.Clamp(1, 2));
                Assert.Equal(2, ((float) 2).Clamp(-1, float.PositiveInfinity));
                Assert.Equal(float.PositiveInfinity, ((float) 2).Clamp(float.PositiveInfinity, float.PositiveInfinity));
                Assert.Equal(float.PositiveInfinity,
                    float.PositiveInfinity.Clamp(float.NegativeInfinity, float.PositiveInfinity));
            }

            //test -Inf Propagation
            {
                Assert.Equal(1, float.NegativeInfinity.Clamp(1, 2));
                Assert.Equal(2, ((float) 2).Clamp(float.NegativeInfinity, 5));
                Assert.Equal(float.NegativeInfinity, ((float) 2).Clamp(float.NegativeInfinity, float.NegativeInfinity));
                Assert.Equal(float.NegativeInfinity,
                    float.NegativeInfinity.Clamp(float.NegativeInfinity, float.PositiveInfinity));
            }

            // ReSharper disable InvokeAsExtensionMethod

            //  Inside the range, equal to the endpoints, and outside the endpoints.
            Assert.Equal(3.0f, Numbers.Clamp(3.0f, 1.0f, 10.0f));

            Assert.Equal(1.0f, Numbers.Clamp(1.0f, 1.0f, 10.0f));
            Assert.Equal(1.0f, Numbers.Clamp(0.0f, 1.0f, 10.0f));
            Assert.Equal(10.0f, Numbers.Clamp(10.0f, 1.0f, 10.0f));
            Assert.Equal(10.0f, Numbers.Clamp(11.0f, 1.0f, 10.0f));

            //  Negative numbers
            Assert.Equal(-3.0f, Numbers.Clamp(-3.0f, -10.0f, -1.0f));
            Assert.Equal(-1.0f, Numbers.Clamp(-1.0f, -10.0f, -1.0f));
            Assert.Equal(-1.0f, Numbers.Clamp(0.0f, -10.0f, -1.0f));
            Assert.Equal(-10.0f, Numbers.Clamp(-10.0f, -10.0f, -1.0f));
            Assert.Equal(-10.0f, Numbers.Clamp(-11.0f, -10.0f, -1.0f));
            //  Mixed positive and negative numbers
            Assert.Equal(5.0f, Numbers.Clamp(5.0f, -10.0f, 10.0f));
            Assert.Equal(-10.0f, Numbers.Clamp(-10.0f, -10.0f, 10.0f));
            Assert.Equal(-10.0f, Numbers.Clamp(-15.0f, -10.0f, 10.0f));
            Assert.Equal(10.0f, Numbers.Clamp(10.0f, -10.0f, 10.0f));
            Assert.Equal(10.0f, Numbers.Clamp(15.0f, -10.0f, 10.0f));
            //  Mixed (1)
            Assert.Equal(5.0f, Numbers.Clamp(5.0f, -10, 10));
            Assert.Equal(-10.0f, Numbers.Clamp(-10.0f, -10, 10));
            Assert.Equal(-10.0f, Numbers.Clamp(-15.0f, -10, 10));
            Assert.Equal(10.0f, Numbers.Clamp(10.0f, -10, 10));
            Assert.Equal(10.0f, Numbers.Clamp(15.0f, -10, 10));
            //  Mixed (2)
            Assert.Equal(5.0f, Numbers.Clamp(5.0f, -10, 10));
            Assert.Equal(-10.0f, Numbers.Clamp(-10.0f, -10, 10));
            Assert.Equal(-10.0f, Numbers.Clamp(-15.0f, -10, 10));
            Assert.Equal(10.0f, Numbers.Clamp(10.0f, -10, 10));
            Assert.Equal(10.0f, Numbers.Clamp(15.0f, -10, 10));

            // ReSharper restore InvokeAsExtensionMethod
        }

        [Fact]
        public void Test_Clamp_Double()
        {
            //test Nan Propagation
            {
                Assert.True(double.IsNaN(double.NaN.Clamp(1, 2)));
                Assert.True(double.IsNaN(((double) 1).Clamp(double.NaN, -1)));
                Assert.True(double.IsNaN(((double) 1).Clamp(double.NaN, 2)));
                Assert.True(double.IsNaN(((double) 1).Clamp(0, double.NaN)));
                Assert.True(double.IsNaN(((double) 1).Clamp(2, double.NaN)));
                Assert.True(double.IsNaN(double.NaN.Clamp(double.NegativeInfinity, double.PositiveInfinity)));
            }


            //test +Inf Propagation
            {
                Assert.Equal(2, double.PositiveInfinity.Clamp(1, 2));
                Assert.Equal(2, ((double) 2).Clamp(-1, double.PositiveInfinity));
                Assert.Equal(double.PositiveInfinity,
                    ((double) 2).Clamp(double.PositiveInfinity, double.PositiveInfinity));
                Assert.Equal(double.PositiveInfinity,
                    double.PositiveInfinity.Clamp(double.NegativeInfinity, double.PositiveInfinity));
            }

            //test -Inf Propagation
            {
                Assert.Equal(1, double.NegativeInfinity.Clamp(1, 2));
                Assert.Equal(2, ((double) 2).Clamp(double.NegativeInfinity, 5));
                Assert.Equal(double.NegativeInfinity,
                    ((double) 2).Clamp(double.NegativeInfinity, double.NegativeInfinity));
                Assert.Equal(double.NegativeInfinity,
                    double.NegativeInfinity.Clamp(double.NegativeInfinity, double.PositiveInfinity));
            }

            // ReSharper disable InvokeAsExtensionMethod

            //  Inside the range, equal to the endpoints, and outside the endpoints.
            Assert.Equal(3.0, Numbers.Clamp(3.0, 1.0, 10.0));

            Assert.Equal(1.0, Numbers.Clamp(1.0, 1.0, 10.0));
            Assert.Equal(1.0, Numbers.Clamp(0.0, 1.0, 10.0));
            Assert.Equal(10.0, Numbers.Clamp(10.0, 1.0, 10.0));
            Assert.Equal(10.0, Numbers.Clamp(11.0, 1.0, 10.0));

            //  Negative numbers
            Assert.Equal(-3.0, Numbers.Clamp(-3.0, -10.0, -1.0));
            Assert.Equal(-1.0, Numbers.Clamp(-1.0, -10.0, -1.0));
            Assert.Equal(-1.0, Numbers.Clamp(0.0, -10.0, -1.0));
            Assert.Equal(-10.0, Numbers.Clamp(-10.0, -10.0, -1.0));
            Assert.Equal(-10.0, Numbers.Clamp(-11.0, -10.0, -1.0));
            //  Mixed positive and negative numbers
            Assert.Equal(5.0, Numbers.Clamp(5.0, -10.0, 10.0));
            Assert.Equal(-10.0, Numbers.Clamp(-10.0, -10.0, 10.0));
            Assert.Equal(-10.0, Numbers.Clamp(-15.0, -10.0, 10.0));
            Assert.Equal(10.0, Numbers.Clamp(10.0, -10.0, 10.0));
            Assert.Equal(10.0, Numbers.Clamp(15.0, -10.0, 10.0));
            //  Mixed (1)
            Assert.Equal(5.0, Numbers.Clamp(5.0, -10, 10));
            Assert.Equal(-10.0, Numbers.Clamp(-10.0, -10, 10));
            Assert.Equal(-10.0, Numbers.Clamp(-15.0, -10, 10));
            Assert.Equal(10.0, Numbers.Clamp(10.0, -10, 10));
            Assert.Equal(10.0, Numbers.Clamp(15.0, -10, 10));
            //  Mixed (2)
            Assert.Equal(5.0, Numbers.Clamp(5.0, -10, 10));
            Assert.Equal(-10.0, Numbers.Clamp(-10.0, -10, 10));
            Assert.Equal(-10.0, Numbers.Clamp(-15.0, -10, 10));
            Assert.Equal(10.0, Numbers.Clamp(10.0, -10, 10));
            Assert.Equal(10.0, Numbers.Clamp(15.0, -10, 10));

            // ReSharper restore InvokeAsExtensionMethod
        }

        [Fact]
        public void Test_Clamp_Decimal()
        {
            // ReSharper disable InvokeAsExtensionMethod

            //  Inside the range, equal to the endpoints, and outside the endpoints.
            Assert.Equal((decimal) 3.0, Numbers.Clamp((decimal) 3.0, (decimal) 1.0, (decimal) 10.0));

            Assert.Equal((decimal) 1.0, Numbers.Clamp((decimal) 1.0, (decimal) 1.0, (decimal) 10.0));
            Assert.Equal((decimal) 1.0, Numbers.Clamp((decimal) 0.0, (decimal) 1.0, (decimal) 10.0));
            Assert.Equal((decimal) 10.0, Numbers.Clamp((decimal) 10.0, (decimal) 1.0, (decimal) 10.0));
            Assert.Equal((decimal) 10.0, Numbers.Clamp((decimal) 11.0, (decimal) 1.0, (decimal) 10.0));

            //  Negative numbers
            Assert.Equal(-(decimal) 3.0, Numbers.Clamp(-(decimal) 3.0, -(decimal) 10.0, -(decimal) 1.0));
            Assert.Equal(-(decimal) 1.0, Numbers.Clamp(-(decimal) 1.0, -(decimal) 10.0, -(decimal) 1.0));
            Assert.Equal(-(decimal) 1.0, Numbers.Clamp((decimal) 0.0, -(decimal) 10.0, -(decimal) 1.0));
            Assert.Equal(-(decimal) 10.0, Numbers.Clamp(-(decimal) 10.0, -(decimal) 10.0, -(decimal) 1.0));
            Assert.Equal(-(decimal) 10.0, Numbers.Clamp(-(decimal) 11.0, -(decimal) 10.0, -(decimal) 1.0));
            //  Mixed positive and negative numbers
            Assert.Equal((decimal) 5.0, Numbers.Clamp((decimal) 5.0, -(decimal) 10.0, (decimal) 10.0));
            Assert.Equal(-(decimal) 10.0, Numbers.Clamp(-(decimal) 10.0, -(decimal) 10.0, (decimal) 10.0));
            Assert.Equal(-(decimal) 10.0, Numbers.Clamp(-(decimal) 15.0, -(decimal) 10.0, (decimal) 10.0));
            Assert.Equal((decimal) 10.0, Numbers.Clamp((decimal) 10.0, -(decimal) 10.0, (decimal) 10.0));
            Assert.Equal((decimal) 10.0, Numbers.Clamp((decimal) 15.0, -(decimal) 10.0, (decimal) 10.0));
            //  Mixed (1)
            Assert.Equal((decimal) 5.0, Numbers.Clamp((decimal) 5.0, -10, 10));
            Assert.Equal(-(decimal) 10.0, Numbers.Clamp(-(decimal) 10.0, -10, 10));
            Assert.Equal(-(decimal) 10.0, Numbers.Clamp(-(decimal) 15.0, -10, 10));
            Assert.Equal((decimal) 10.0, Numbers.Clamp((decimal) 10.0, -10, 10));
            Assert.Equal((decimal) 10.0, Numbers.Clamp((decimal) 15.0, -10, 10));
            //  Mixed (2)
            Assert.Equal((decimal) 5.0, Numbers.Clamp((decimal) 5.0, -10, 10));
            Assert.Equal(-(decimal) 10.0, Numbers.Clamp(-(decimal) 10.0, -10, 10));
            Assert.Equal(-(decimal) 10.0, Numbers.Clamp(-(decimal) 15.0, -10, 10));
            Assert.Equal((decimal) 10.0, Numbers.Clamp((decimal) 10.0, -10, 10));
            Assert.Equal((decimal) 10.0, Numbers.Clamp((decimal) 15.0, -10, 10));

            // ReSharper restore InvokeAsExtensionMethod
        }


        [Fact]
        public void Test_Clamp_byte()
        {
            // ReSharper disable InvokeAsExtensionMethod

            //  Inside the range, equal to the endpobytes, and outside the endpobytes.
            Assert.Equal((byte) 3.0, Numbers.Clamp((byte) 3.0, (byte) 1.0, (byte) 10.0));

            Assert.Equal((byte) 1.0, Numbers.Clamp((byte) 1.0, (byte) 1.0, (byte) 10.0));
            Assert.Equal((byte) 1.0, Numbers.Clamp((byte) 0.0, (byte) 1.0, (byte) 10.0));
            Assert.Equal((byte) 10.0, Numbers.Clamp((byte) 10.0, (byte) 1.0, (byte) 10.0));
            Assert.Equal((byte) 10.0, Numbers.Clamp((byte) 11.0, (byte) 1.0, (byte) 10.0));

            //  Negative numbers
            Assert.Equal(-(byte) 3.0, Numbers.Clamp(-(byte) 3.0, -(byte) 10.0, -(byte) 1.0));
            Assert.Equal(-(byte) 1.0, Numbers.Clamp(-(byte) 1.0, -(byte) 10.0, -(byte) 1.0));
            Assert.Equal(-(byte) 10.0, Numbers.Clamp(-(byte) 10.0, -(byte) 10.0, -(byte) 1.0));
            Assert.Equal(-(byte) 10.0, Numbers.Clamp(-(byte) 11.0, -(byte) 10.0, -(byte) 1.0));
            //  Mixed (1)
            Assert.Equal(-(byte) 10.0, Numbers.Clamp(-(byte) 10.0, -10, 10));
            Assert.Equal(-(byte) 10.0, Numbers.Clamp(-(byte) 15.0, -10, 10));
            //  Mixed (2)
            Assert.Equal(-(byte) 10.0, Numbers.Clamp(-(byte) 10.0, -10, 10));
            Assert.Equal(-(byte) 10.0, Numbers.Clamp(-(byte) 15.0, -10, 10));

            // ReSharper restore InvokeAsExtensionMethod
        }


        [Fact]
        public void Test_Clamp_sbyte()
        {
            // ReSharper disable InvokeAsExtensionMethod

            //  Inside the range, equal to the endposbytes, and outside the endposbytes.
            Assert.Equal((sbyte) 3.0, Numbers.Clamp((sbyte) 3.0, (sbyte) 1.0, (sbyte) 10.0));

            Assert.Equal((sbyte) 1.0, Numbers.Clamp((sbyte) 1.0, (sbyte) 1.0, (sbyte) 10.0));
            Assert.Equal((sbyte) 1.0, Numbers.Clamp((sbyte) 0.0, (sbyte) 1.0, (sbyte) 10.0));
            Assert.Equal((sbyte) 10.0, Numbers.Clamp((sbyte) 10.0, (sbyte) 1.0, (sbyte) 10.0));
            Assert.Equal((sbyte) 10.0, Numbers.Clamp((sbyte) 11.0, (sbyte) 1.0, (sbyte) 10.0));

            //  Negative numbers
            Assert.Equal(-(sbyte) 3.0, Numbers.Clamp(-(sbyte) 3.0, -(sbyte) 10.0, -(sbyte) 1.0));
            Assert.Equal(-(sbyte) 1.0, Numbers.Clamp(-(sbyte) 1.0, -(sbyte) 10.0, -(sbyte) 1.0));
            Assert.Equal(-(sbyte) 10.0, Numbers.Clamp(-(sbyte) 10.0, -(sbyte) 10.0, -(sbyte) 1.0));
            Assert.Equal(-(sbyte) 10.0, Numbers.Clamp(-(sbyte) 11.0, -(sbyte) 10.0, -(sbyte) 1.0));
            //  Mixed positive and negative numbers
            //  Mixed (1)
            Assert.Equal(-(sbyte) 10.0, Numbers.Clamp(-(sbyte) 10.0, -10, 10));
            Assert.Equal(-(sbyte) 10.0, Numbers.Clamp(-(sbyte) 15.0, -10, 10));
            //  Mixed (2)
            Assert.Equal(-(sbyte) 10.0, Numbers.Clamp(-(sbyte) 10.0, -10, 10));
            Assert.Equal(-(sbyte) 10.0, Numbers.Clamp(-(sbyte) 15.0, -10, 10));

            // ReSharper restore InvokeAsExtensionMethod
        }

        [Fact]
        public void Test_Clamp_short()
        {
            // ReSharper disable InvokeAsExtensionMethod

            //  Inside the range, equal to the endposhorts, and outside the endposhorts.
            Assert.Equal((short) 3.0, Numbers.Clamp((short) 3.0, (short) 1.0, (short) 10.0));

            Assert.Equal((short) 1.0, Numbers.Clamp((short) 1.0, (short) 1.0, (short) 10.0));
            Assert.Equal((short) 1.0, Numbers.Clamp((short) 0.0, (short) 1.0, (short) 10.0));
            Assert.Equal((short) 10.0, Numbers.Clamp((short) 10.0, (short) 1.0, (short) 10.0));
            Assert.Equal((short) 10.0, Numbers.Clamp((short) 11.0, (short) 1.0, (short) 10.0));

            //  Negative numbers
            Assert.Equal(-(short) 3.0, Numbers.Clamp(-(short) 3.0, -(short) 10.0, -(short) 1.0));
            Assert.Equal(-(short) 1.0, Numbers.Clamp(-(short) 1.0, -(short) 10.0, -(short) 1.0));
            Assert.Equal(-(short) 10.0, Numbers.Clamp(-(short) 10.0, -(short) 10.0, -(short) 1.0));
            Assert.Equal(-(short) 10.0, Numbers.Clamp(-(short) 11.0, -(short) 10.0, -(short) 1.0));
            //  Mixed (1)
            Assert.Equal(-(short) 10.0, Numbers.Clamp(-(short) 10.0, -10, 10));
            Assert.Equal(-(short) 10.0, Numbers.Clamp(-(short) 15.0, -10, 10));
            //  Mixed (2)
            Assert.Equal(-(short) 10.0, Numbers.Clamp(-(short) 10.0, -10, 10));
            Assert.Equal(-(short) 10.0, Numbers.Clamp(-(short) 15.0, -10, 10));

            // ReSharper restore InvokeAsExtensionMethod
        }

        [Fact]
        public void Test_Clamp_ushort()
        {
            // ReSharper disable InvokeAsExtensionMethod

            //  Inside the range, equal to the endpoushorts, and outside the endpoushorts.
            Assert.Equal((ushort) 3.0, Numbers.Clamp((ushort) 3.0, (ushort) 1.0, (ushort) 10.0));

            Assert.Equal((ushort) 1.0, Numbers.Clamp((ushort) 1.0, (ushort) 1.0, (ushort) 10.0));
            Assert.Equal((ushort) 1.0, Numbers.Clamp((ushort) 0.0, (ushort) 1.0, (ushort) 10.0));
            Assert.Equal((ushort) 10.0, Numbers.Clamp((ushort) 10.0, (ushort) 1.0, (ushort) 10.0));
            Assert.Equal((ushort) 10.0, Numbers.Clamp((ushort) 11.0, (ushort) 1.0, (ushort) 10.0));

            //  Negative numbers
            Assert.Equal(-(ushort) 3.0, Numbers.Clamp(-(ushort) 3.0, -(ushort) 10.0, -(ushort) 1.0));
            Assert.Equal(-(ushort) 1.0, Numbers.Clamp(-(ushort) 1.0, -(ushort) 10.0, -(ushort) 1.0));
            Assert.Equal(-(ushort) 1.0, Numbers.Clamp((ushort) 0.0, -(ushort) 10.0, -(ushort) 1.0));
            Assert.Equal(-(ushort) 10.0, Numbers.Clamp(-(ushort) 10.0, -(ushort) 10.0, -(ushort) 1.0));
            Assert.Equal(-(ushort) 10.0, Numbers.Clamp(-(ushort) 11.0, -(ushort) 10.0, -(ushort) 1.0));
            //  Mixed positive and negative numbers
            Assert.Equal((ushort) 5.0, Numbers.Clamp((ushort) 5.0, -(ushort) 10.0, (ushort) 10.0));
            Assert.Equal(-(ushort) 10.0, Numbers.Clamp(-(ushort) 10.0, -(ushort) 10.0, (ushort) 10.0));
            Assert.Equal(-(ushort) 10.0, Numbers.Clamp(-(ushort) 15.0, -(ushort) 10.0, (ushort) 10.0));
            Assert.Equal((ushort) 10.0, Numbers.Clamp((ushort) 10.0, -(ushort) 10.0, (ushort) 10.0));
            Assert.Equal((ushort) 10.0, Numbers.Clamp((ushort) 15.0, -(ushort) 10.0, (ushort) 10.0));
            //  Mixed (1)
            Assert.Equal((ushort) 5.0, Numbers.Clamp((ushort) 5.0, -10, 10));
            Assert.Equal(-(ushort) 10.0, Numbers.Clamp(-(ushort) 10.0, -10, 10));
            Assert.Equal(-(ushort) 10.0, Numbers.Clamp(-(ushort) 15.0, -10, 10));
            Assert.Equal((ushort) 10.0, Numbers.Clamp((ushort) 10.0, -10, 10));
            Assert.Equal((ushort) 10.0, Numbers.Clamp((ushort) 15.0, -10, 10));
            //  Mixed (2)
            Assert.Equal((ushort) 5.0, Numbers.Clamp((ushort) 5.0, -10, 10));
            Assert.Equal(-(ushort) 10.0, Numbers.Clamp(-(ushort) 10.0, -10, 10));
            Assert.Equal(-(ushort) 10.0, Numbers.Clamp(-(ushort) 15.0, -10, 10));
            Assert.Equal((ushort) 10.0, Numbers.Clamp((ushort) 10.0, -10, 10));
            Assert.Equal((ushort) 10.0, Numbers.Clamp((ushort) 15.0, -10, 10));

            // ReSharper restore InvokeAsExtensionMethod
        }

        [Fact]
        public void Test_Clamp_uint()
        {
            // ReSharper disable InvokeAsExtensionMethod

            //  Inside the range, equal to the endpouints, and outside the endpouints.
            Assert.Equal((uint) 3.0, Numbers.Clamp((uint) 3.0, (uint) 1.0, (uint) 10.0));

            Assert.Equal((uint) 1.0, Numbers.Clamp((uint) 1.0, (uint) 1.0, (uint) 10.0));
            Assert.Equal((uint) 1.0, Numbers.Clamp((uint) 0.0, (uint) 1.0, (uint) 10.0));
            Assert.Equal((uint) 10.0, Numbers.Clamp((uint) 10.0, (uint) 1.0, (uint) 10.0));
            Assert.Equal((uint) 10.0, Numbers.Clamp((uint) 11.0, (uint) 1.0, (uint) 10.0));

            //  Negative numbers
            Assert.Equal(-(uint) 3.0, Numbers.Clamp(-(uint) 3.0, -(uint) 10.0, -(uint) 1.0));
            Assert.Equal(-(uint) 1.0, Numbers.Clamp(-(uint) 1.0, -(uint) 10.0, -(uint) 1.0));
            Assert.Equal(-(uint) 1.0, Numbers.Clamp((uint) 0.0, -(uint) 10.0, -(uint) 1.0));
            Assert.Equal(-(uint) 10.0, Numbers.Clamp(-(uint) 10.0, -(uint) 10.0, -(uint) 1.0));
            Assert.Equal(-(uint) 10.0, Numbers.Clamp(-(uint) 11.0, -(uint) 10.0, -(uint) 1.0));
            //  Mixed positive and negative numbers
            Assert.Equal((uint) 5.0, Numbers.Clamp((uint) 5.0, -(uint) 10.0, (uint) 10.0));
            Assert.Equal(-(uint) 10.0, Numbers.Clamp(-(uint) 10.0, -(uint) 10.0, (uint) 10.0));
            Assert.Equal(-(uint) 10.0, Numbers.Clamp(-(uint) 15.0, -(uint) 10.0, (uint) 10.0));
            Assert.Equal((uint) 10.0, Numbers.Clamp((uint) 10.0, -(uint) 10.0, (uint) 10.0));
            Assert.Equal((uint) 10.0, Numbers.Clamp((uint) 15.0, -(uint) 10.0, (uint) 10.0));
            //  Mixed (1)
            Assert.Equal((uint) 5.0, Numbers.Clamp((uint) 5.0, -10, 10));
            Assert.Equal(-(uint) 10.0, Numbers.Clamp(-(uint) 10.0, -10, 10));
            Assert.Equal(-(uint) 10.0, Numbers.Clamp(-(uint) 15.0, -10, 10));
            Assert.Equal((uint) 10.0, Numbers.Clamp((uint) 10.0, -10, 10));
            Assert.Equal((uint) 10.0, Numbers.Clamp((uint) 15.0, -10, 10));
            //  Mixed (2)
            Assert.Equal((uint) 5.0, Numbers.Clamp((uint) 5.0, -10, 10));
            Assert.Equal(-(uint) 10.0, Numbers.Clamp(-(uint) 10.0, -10, 10));
            Assert.Equal(-(uint) 10.0, Numbers.Clamp(-(uint) 15.0, -10, 10));
            Assert.Equal((uint) 10.0, Numbers.Clamp((uint) 10.0, -10, 10));
            Assert.Equal((uint) 10.0, Numbers.Clamp((uint) 15.0, -10, 10));

            // ReSharper restore InvokeAsExtensionMethod
        }

        [Fact]
        public void Test_Clamp_int()
        {
            // ReSharper disable InvokeAsExtensionMethod

            //  Inside the range, equal to the endpoints, and outside the endpoints.
            Assert.Equal((int) 3.0, Numbers.Clamp((int) 3.0, (int) 1.0, (int) 10.0));

            Assert.Equal((int) 1.0, Numbers.Clamp((int) 1.0, (int) 1.0, (int) 10.0));
            Assert.Equal((int) 1.0, Numbers.Clamp((int) 0.0, (int) 1.0, (int) 10.0));
            Assert.Equal((int) 10.0, Numbers.Clamp((int) 10.0, (int) 1.0, (int) 10.0));
            Assert.Equal((int) 10.0, Numbers.Clamp((int) 11.0, (int) 1.0, (int) 10.0));

            //  Negative numbers
            Assert.Equal(-(int) 3.0, Numbers.Clamp(-(int) 3.0, -(int) 10.0, -(int) 1.0));
            Assert.Equal(-(int) 1.0, Numbers.Clamp(-(int) 1.0, -(int) 10.0, -(int) 1.0));
            Assert.Equal(-(int) 1.0, Numbers.Clamp((int) 0.0, -(int) 10.0, -(int) 1.0));
            Assert.Equal(-(int) 10.0, Numbers.Clamp(-(int) 10.0, -(int) 10.0, -(int) 1.0));
            Assert.Equal(-(int) 10.0, Numbers.Clamp(-(int) 11.0, -(int) 10.0, -(int) 1.0));
            //  Mixed positive and negative numbers
            Assert.Equal((int) 5.0, Numbers.Clamp((int) 5.0, -(int) 10.0, (int) 10.0));
            Assert.Equal(-(int) 10.0, Numbers.Clamp(-(int) 10.0, -(int) 10.0, (int) 10.0));
            Assert.Equal(-(int) 10.0, Numbers.Clamp(-(int) 15.0, -(int) 10.0, (int) 10.0));
            Assert.Equal((int) 10.0, Numbers.Clamp((int) 10.0, -(int) 10.0, (int) 10.0));
            Assert.Equal((int) 10.0, Numbers.Clamp((int) 15.0, -(int) 10.0, (int) 10.0));
            //  Mixed (1)
            Assert.Equal((int) 5.0, Numbers.Clamp((int) 5.0, -10, 10));
            Assert.Equal(-(int) 10.0, Numbers.Clamp(-(int) 10.0, -10, 10));
            Assert.Equal(-(int) 10.0, Numbers.Clamp(-(int) 15.0, -10, 10));
            Assert.Equal((int) 10.0, Numbers.Clamp((int) 10.0, -10, 10));
            Assert.Equal((int) 10.0, Numbers.Clamp((int) 15.0, -10, 10));
            //  Mixed (2)
            Assert.Equal((int) 5.0, Numbers.Clamp((int) 5.0, -10, 10));
            Assert.Equal(-(int) 10.0, Numbers.Clamp(-(int) 10.0, -10, 10));
            Assert.Equal(-(int) 10.0, Numbers.Clamp(-(int) 15.0, -10, 10));
            Assert.Equal((int) 10.0, Numbers.Clamp((int) 10.0, -10, 10));
            Assert.Equal((int) 10.0, Numbers.Clamp((int) 15.0, -10, 10));

            // ReSharper restore InvokeAsExtensionMethod
        }

        [Fact]
        public void Test_Clamp_long()
        {
            // ReSharper disable InvokeAsExtensionMethod

            //  Inside the range, equal to the endpolongs, and outside the endpolongs.
            Assert.Equal((long) 3.0, Numbers.Clamp((long) 3.0, (long) 1.0, (long) 10.0));

            Assert.Equal((long) 1.0, Numbers.Clamp((long) 1.0, (long) 1.0, (long) 10.0));
            Assert.Equal((long) 1.0, Numbers.Clamp((long) 0.0, (long) 1.0, (long) 10.0));
            Assert.Equal((long) 10.0, Numbers.Clamp((long) 10.0, (long) 1.0, (long) 10.0));
            Assert.Equal((long) 10.0, Numbers.Clamp((long) 11.0, (long) 1.0, (long) 10.0));

            //  Negative numbers
            Assert.Equal(-(long) 3.0, Numbers.Clamp(-(long) 3.0, -(long) 10.0, -(long) 1.0));
            Assert.Equal(-(long) 1.0, Numbers.Clamp(-(long) 1.0, -(long) 10.0, -(long) 1.0));
            Assert.Equal(-(long) 1.0, Numbers.Clamp((long) 0.0, -(long) 10.0, -(long) 1.0));
            Assert.Equal(-(long) 10.0, Numbers.Clamp(-(long) 10.0, -(long) 10.0, -(long) 1.0));
            Assert.Equal(-(long) 10.0, Numbers.Clamp(-(long) 11.0, -(long) 10.0, -(long) 1.0));
            //  Mixed positive and negative numbers
            Assert.Equal((long) 5.0, Numbers.Clamp((long) 5.0, -(long) 10.0, (long) 10.0));
            Assert.Equal(-(long) 10.0, Numbers.Clamp(-(long) 10.0, -(long) 10.0, (long) 10.0));
            Assert.Equal(-(long) 10.0, Numbers.Clamp(-(long) 15.0, -(long) 10.0, (long) 10.0));
            Assert.Equal((long) 10.0, Numbers.Clamp((long) 10.0, -(long) 10.0, (long) 10.0));
            Assert.Equal((long) 10.0, Numbers.Clamp((long) 15.0, -(long) 10.0, (long) 10.0));
            //  Mixed (1)
            Assert.Equal((long) 5.0, Numbers.Clamp((long) 5.0, -10, 10));
            Assert.Equal(-(long) 10.0, Numbers.Clamp(-(long) 10.0, -10, 10));
            Assert.Equal(-(long) 10.0, Numbers.Clamp(-(long) 15.0, -10, 10));
            Assert.Equal((long) 10.0, Numbers.Clamp((long) 10.0, -10, 10));
            Assert.Equal((long) 10.0, Numbers.Clamp((long) 15.0, -10, 10));
            //  Mixed (2)
            Assert.Equal((long) 5.0, Numbers.Clamp((long) 5.0, -10, 10));
            Assert.Equal(-(long) 10.0, Numbers.Clamp(-(long) 10.0, -10, 10));
            Assert.Equal(-(long) 10.0, Numbers.Clamp(-(long) 15.0, -10, 10));
            Assert.Equal((long) 10.0, Numbers.Clamp((long) 10.0, -10, 10));
            Assert.Equal((long) 10.0, Numbers.Clamp((long) 15.0, -10, 10));

            // ReSharper restore InvokeAsExtensionMethod
        }

        [Fact]
        public void Test_Clamp_ulong()
        {
            // ReSharper disable InvokeAsExtensionMethod

            //  Inside the range, equal to the endpoulongs, and outside the endpoulongs.
            Assert.Equal((ulong) 3.0, Numbers.Clamp((ulong) 3.0, (ulong) 1.0, (ulong) 10.0));

            Assert.Equal((ulong) 1.0, Numbers.Clamp((ulong) 1.0, (ulong) 1.0, (ulong) 10.0));
            Assert.Equal((ulong) 1.0, Numbers.Clamp((ulong) 0.0, (ulong) 1.0, (ulong) 10.0));
            Assert.Equal((ulong) 10.0, Numbers.Clamp((ulong) 10.0, (ulong) 1.0, (ulong) 10.0));
            Assert.Equal((ulong) 10.0, Numbers.Clamp((ulong) 11.0, (ulong) 1.0, (ulong) 10.0));

            // ReSharper restore InvokeAsExtensionMethod
        }

        [Fact]
        public void Test_Clamp_Boxed_ulong()
        {
            // ReSharper disable InvokeAsExtensionMethod

            //  Inside the range, equal to the endpoulongs, and outside the endpoulongs.
            Assert.Equal((Boxed<ulong>) 3.0,
                Numbers.Clamp<Boxed<ulong>>((Boxed<ulong>) 3.0, (Boxed<ulong>) 1.0, (ulong) 10.0));

            Assert.Equal((Boxed<ulong>) 1.0,
                Numbers.Clamp<Boxed<ulong>>((Boxed<ulong>) 1.0, (Boxed<ulong>) 1.0, (Boxed<ulong>) 10.0));
            Assert.Equal((Boxed<ulong>) 1.0,
                Numbers.Clamp<Boxed<ulong>>((Boxed<ulong>) 0.0, (Boxed<ulong>) 1.0, (Boxed<ulong>) 10.0));
            Assert.Equal((Boxed<ulong>) 10.0,
                Numbers.Clamp<Boxed<ulong>>((Boxed<ulong>) 10.0, (Boxed<ulong>) 1.0, (Boxed<ulong>) 10.0));
            Assert.Equal((Boxed<ulong>) 10.0,
                Numbers.Clamp<Boxed<ulong>>((Boxed<ulong>) 11.0, (Boxed<ulong>) 1.0, (Boxed<ulong>) 10.0));


            Assert.Equal((ulong) 3.0, Numbers.Clamp((Boxed<ulong>) 3.0, (Boxed<ulong>) 1.0, (ulong) 10.0));

            Assert.Equal((Boxed<ulong>) 1.0,
                Numbers.Clamp((Boxed<ulong>) 1.0, (Boxed<ulong>) 1.0, (Boxed<ulong>) 10.0,
                    Comparer<Boxed<ulong>>.Default));
            Assert.Equal((Boxed<ulong>) 1.0,
                Numbers.Clamp((Boxed<ulong>) 0.0, (Boxed<ulong>) 1.0, (Boxed<ulong>) 10.0,
                    Comparer<Boxed<ulong>>.Default));
            Assert.Equal((Boxed<ulong>) 10.0,
                Numbers.Clamp((Boxed<ulong>) 10.0, (Boxed<ulong>) 1.0, (Boxed<ulong>) 10.0,
                    Comparer<Boxed<ulong>>.Default));
            Assert.Equal((Boxed<ulong>) 10.0,
                Numbers.Clamp((Boxed<ulong>) 11.0, (Boxed<ulong>) 1.0, (Boxed<ulong>) 10.0,
                    Comparer<Boxed<ulong>>.Default));

            // ReSharper restore InvokeAsExtensionMethod
        }
    }
}