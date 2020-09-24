using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Maxisoft.Utils.Strings;
using Xunit;

namespace Maxisoft.Utils.Tests.Strings
{
    public class StringExtensionTests
    {
        [Fact]
        public void FormatWithDictionaryTest_Nominal()
        {
            Assert.Equal("1", "{x}".FormatWithDictionary(new Dictionary<string, object> {{"x", 1}}));
            Assert.Equal("1 ystr",
                "{x} {y}".FormatWithDictionary(new Dictionary<string, object> {{"x", 1}, {"y", "ystr"}}));
            Assert.Equal("1 y ystr",
                "{x} y {y}".FormatWithDictionary(new Dictionary<string, object> {{"x", 1}, {"y", "ystr"}}));
            Assert.Throws<FormatException>(
                () => "{x} y {y}".FormatWithDictionary(new Dictionary<string, object> {{"x", 1}}));
            Assert.Throws<FormatException>(() =>
                "{x}".FormatWithDictionary(new Dictionary<string, object> {{"y", "ystr"}}));
            Assert.Equal("ystr",
                "{y}".FormatWithDictionary(new Dictionary<string, object> {{"x", 1}, {"y", "ystr"}}));
        }


        [Theory]
        [InlineData("", "")]
        [InlineData("a", "A")]
        [InlineData("z", "Z")]
        [InlineData("Z", "Z")]
        [InlineData("Zebra", "Zebra")]
        [InlineData("zebra", "Zebra")]
        [InlineData("a zebra", "A zebra")]
        [InlineData("a Fast zebra", "A Fast zebra")]
        [SuppressMessage("ReSharper", "InvokeAsExtensionMethod")]
        public void Test_Capitalize(string input, string expected)
        {
            Assert.Equal(expected, StringExtensions.Capitalize(input));
            Assert.Equal(expected, StringExtensions.Capitalize(input, CultureInfo.InvariantCulture));

            ReadOnlySpan<char> inputSpan = input;
            const int buffSize = 16;
            Assert.True(buffSize > input.Length);
            Span<char> buff = stackalloc char[buffSize];
            buff.Fill('X');
            StringExtensions.Capitalize(inputSpan, buff.Slice(0, input.Length));
            Assert.Equal(expected, buff.Slice(0, input.Length).ToString());
            buff.Fill('X');
            StringExtensions.Capitalize(inputSpan, buff, CultureInfo.InvariantCulture);
            Assert.Equal(expected, buff.Slice(0, input.Length).ToString());
            buff.Fill('X');
            inputSpan.CopyTo(buff);
            StringExtensions.Capitalize(buff);
            Assert.Equal(expected, buff.Slice(0, input.Length).ToString());
            buff.Fill('X');
            inputSpan.CopyTo(buff);
            StringExtensions.Capitalize(buff.Slice(0, input.Length), CultureInfo.InvariantCulture);
            Assert.Equal(expected, buff.Slice(0, input.Length).ToString());
        }

        [Theory]
        [InlineData("", 0, "")]
        [InlineData("", 1, "")]
        [InlineData("", 3, "")]
        [InlineData("a", 0, "")]
        [InlineData("a", 1, "a")]
        [InlineData("a", 5, "aaaaa")]
        [InlineData("Zebra", 3, "ZebraZebraZebra")]
        [InlineData("a Fast zebra", 0, "")]
        [InlineData("A Fast zebra", 1, "A Fast zebra")]
        [InlineData("a Fast zebra ", 5, "a Fast zebra a Fast zebra a Fast zebra a Fast zebra a Fast zebra ")]
        [SuppressMessage("ReSharper", "InvokeAsExtensionMethod")]
        public void Test_Repeat(string input, int count, string expected)
        {
            Assert.Equal(expected, StringExtensions.Repeat(input, count));
            if (input.Length == 1)
            {
                Assert.Equal(expected, StringExtensions.Repeat(input[0], count));
            }

            ReadOnlySpan<char> inputSpan = input;
            const int buffSize = 96;
            Assert.True(buffSize > expected.Length);
            Span<char> buff = stackalloc char[buffSize];
            buff.Fill('X');
            StringExtensions.Repeat(inputSpan, buff, count);
            Assert.Equal(expected, buff.Slice(0, input.Length * count).ToString());

            if (input.Length == 1)
            {
                buff.Fill('X');
                StringExtensions.Repeat(inputSpan[0], buff, count);
                Assert.Equal(expected, buff.Slice(0, input.Length * count).ToString());
            }
        }


        [Fact]
        [SuppressMessage("ReSharper", "InvokeAsExtensionMethod")]
        public void Test_Repeat_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => StringExtensions.Repeat("input", -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => StringExtensions.Repeat('i', -1));
            const int buffSize = 2;

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Span<char> buff = stackalloc char[buffSize];
                ReadOnlySpan<char> input = "i";
                StringExtensions.Repeat(input, buff, -1);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Span<char> buff = stackalloc char[buffSize];
                StringExtensions.Repeat('i', buff, -1);
            });
        }
    }
}