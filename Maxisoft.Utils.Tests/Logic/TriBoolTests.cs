using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text.Json;
using Maxisoft.Utils.Logic;
using Xunit;

namespace Maxisoft.Utils.Tests.Logic
{
    public class TriBoolTests
    {
        [Fact]
        public void Test_DefaultConstructible_SetToFalse()
        {
            var tri = new TriBool();

            Assert.True(tri.IsFalse);
            Assert.False(tri.IsTrue);
            Assert.False(tri.IsIndeterminate);
            Assert.False(tri);
            Assert.True(!tri);

            Assert.True(tri == default);
            Assert.Equal(default, tri);

            bool? b = false;
            Assert.True(b == tri);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [InlineData(null)]
        public void Test_LogicalNegation_Adversarial(bool? x)
        {
            var adversarial = new TriBool(!x);
            var computed = !new TriBool(x);
            Assert.Equal(adversarial, computed);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        [InlineData(null, true)]
        [InlineData(true, null)]
        [InlineData(null, false)]
        [InlineData(false, null)]
        [InlineData(null, null)]
        public void Test_LogicalOr_Adversarial(bool? x, bool? y)
        {
            var adversarial = new TriBool(x | y);
            var computed = new TriBool(x) | new TriBool(y);
            Assert.Equal(adversarial, computed);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        [InlineData(null, true)]
        [InlineData(true, null)]
        [InlineData(null, false)]
        [InlineData(false, null)]
        [InlineData(null, null)]
        public void Test_LogicalAnd_Adversarial(bool? x, bool? y)
        {
            var adversarial = new TriBool(x & y);
            var computed = new TriBool(x) & new TriBool(y);
            Assert.Equal(adversarial, computed);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        [InlineData(null, true)]
        [InlineData(true, null)]
        [InlineData(null, false)]
        [InlineData(false, null)]
        [InlineData(null, null)]
        public void Test_LogicalXor_Adversarial(bool? x, bool? y)
        {
            var adversarial = new TriBool(x ^ y);
            var computed = new TriBool(x) ^ new TriBool(y);
            Assert.Equal(adversarial, computed);
        }

        [SkippableTheory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        [InlineData(null, true)]
        [InlineData(true, null)]
        [InlineData(null, false)]
        [InlineData(false, null)]
        [InlineData(null, null)]
        public void Test_ConditionalLogicalOr_Adversarial(bool? x, bool? y)
        {
            Skip.If(!x.HasValue || !y.HasValue, "not testable");
            var adversarial = new TriBool(x.Value || y.Value);
            var computed = new TriBool(x) || new TriBool(y);
            Assert.Equal(adversarial, computed);
        }

        [SkippableTheory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        [InlineData(null, true)]
        [InlineData(true, null)]
        [InlineData(null, false)]
        [InlineData(false, null)]
        [InlineData(null, null)]
        public void Test_ConditionalLogicalAnd_Adversarial(bool? x, bool? y)
        {
            Skip.If(!x.HasValue || !y.HasValue, "not testable");
            var adversarial = new TriBool(x.Value && y.Value);
            var computed = new TriBool(x) && new TriBool(y);
            Assert.Equal(adversarial, computed);
        }

        [Fact]
        [SuppressMessage("ReSharper", "EqualExpressionComparison")]
        public void Test()
        {
            // adapted from https://www.boost.org/doc/libs/1_59_0/libs/logic/test/tribool_test.cpp
            static bool indeterminate(TriBool x)
            {
                return x.TriValue == TriBoolValue.Indeterminate;
            }

            TriBool x = default; // false
            TriBool y = true; // true
            var z = TriBool.Indeterminate; // indeterminate

            Assert.True(!x);
            Assert.True(x == false);
            Assert.True(false == x);
            Assert.True(x != true);
            Assert.True(true != x);
            Assert.True((x == TriBool.Indeterminate).IsIndeterminate);
            Assert.True((TriBool.Indeterminate == x).IsIndeterminate);
            Assert.True((x != TriBool.Indeterminate).IsIndeterminate);
            Assert.True((TriBool.Indeterminate != x).IsIndeterminate);
            Assert.True(x == x);
            Assert.True(!(x != x));
            Assert.True(!(x && true));
            Assert.True(!(true && x));
            Assert.True(x || true);
            Assert.True(true || x);

            Assert.True(y);
            Assert.True(y == true);
            Assert.True(true == y);
            Assert.True(y != false);
            Assert.True(false != y);
            Assert.True(indeterminate(y == TriBool.Indeterminate));
            Assert.True(indeterminate(TriBool.Indeterminate == y));
            Assert.True(indeterminate(y != TriBool.Indeterminate));
            Assert.True(indeterminate(TriBool.Indeterminate != y));
            Assert.True(y == y);
            Assert.True(!(y != y));

            Assert.True(indeterminate(z || !z));
            Assert.True(indeterminate(z == true));
            Assert.True(indeterminate(true == z));
            Assert.True(indeterminate(z == false));
            Assert.True(indeterminate(false == z));
            Assert.True(indeterminate(z == TriBool.Indeterminate));
            Assert.True(indeterminate(TriBool.Indeterminate == z));
            Assert.True(indeterminate(z != TriBool.Indeterminate));
            Assert.True(indeterminate(TriBool.Indeterminate != z));
            Assert.True(indeterminate(z == z));
            Assert.True(indeterminate(z != z));

            Assert.True(!(x == y));
            Assert.True(x != y);
            Assert.True(indeterminate(x == z));
            Assert.True(indeterminate(x != z));
            Assert.True(indeterminate(y == z));
            Assert.True(indeterminate(y != z));

            Assert.True(!(x && y));
            Assert.True(x || y);
            Assert.True(!(x && z));
            Assert.True((y && z).IsIndeterminate);
            Assert.True((z && z).IsIndeterminate);
            Assert.True((z || z).IsIndeterminate);
            Assert.True((x || z).IsIndeterminate);
            Assert.True(y || z);

            Assert.True((y && TriBool.Indeterminate).IsIndeterminate);
            Assert.True((TriBool.Indeterminate && y).IsIndeterminate);
            Assert.True(!(x && TriBool.Indeterminate));
            Assert.True(!(TriBool.Indeterminate && x));

            Assert.True(TriBool.Indeterminate | y);
            Assert.True(y || TriBool.Indeterminate);
            Assert.True((x || TriBool.Indeterminate).IsIndeterminate);
            Assert.True((TriBool.Indeterminate || x).IsIndeterminate);

            // Test the if (z) ... else (!z) ... else ... idiom
            if (z)
            {
                Assert.True(false);
            }
            else if (!z)
            {
                Assert.True(false);
            }
            else
            {
                Assert.True(true);
            }

            z = true;
            if (z)
            {
                Assert.True(true);
            }
            else if (!z)
            {
                Assert.True(false);
            }
            else
            {
                Assert.True(false);
            }

            z = false;
            if (z)
            {
                Assert.True(false);
            }
            else if (!z)
            {
                Assert.True(true);
            }
            else
            {
                Assert.True(false);
            }
        }

        [Theory]
        [InlineData(null, false, null)]
        [InlineData("", false, null)]
        [InlineData("True", true, true)]
        [InlineData("False", true, false)]
        [InlineData("true", true, true)]
        [InlineData("false", true, false)]
        [InlineData("null", true, null)]
        [InlineData("    true    ", true, true)]
        [InlineData("0", false, null)]
        [InlineData("1", false, null)]
        [InlineData("-1", false, null)]
        [InlineData("string", false, false)]
        public void Test_TryParse(string s, bool success, bool? expected)
        {
            var res = TriBool.TryParse(s, out var actual);
            Assert.Equal(success, res);
            if (res)
            {
                Assert.Equal(new TriBool(expected), actual);
            }
            else
            {
                Assert.Equal(TriBool.Indeterminate, actual);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(null)]
        public void Test_Serializable(bool? value)
        {
            using var stream = new MemoryStream();
            var formatter = new SoapFormatter();
            var expected = new TriBool(value);

            formatter.Serialize(stream, expected);
            stream.Seek(0, 0);

            formatter = new SoapFormatter();

            var actual = (TriBool) formatter.Deserialize(stream);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(null)]
        public void Test_JsonSerializable(bool? value)
        {
            var expected = new TriBool(value);
            var data = JsonSerializer.Serialize(expected);
            var adversarial = JsonSerializer.Serialize(value);
            Assert.Equal(adversarial, data);

            var actual = JsonSerializer.Deserialize<TriBool>(data);

            Assert.Equal(expected, actual);
        }
    }
}