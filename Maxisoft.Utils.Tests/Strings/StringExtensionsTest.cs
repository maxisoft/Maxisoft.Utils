using System;
using System.Collections.Generic;
using Maxisoft.Utils.Strings;
using Xunit;

namespace Maxisoft.Utils.Tests.Strings
{
    public class StringExtensionTest
    {
        [Fact]
        public void FormatWithDictionaryTest_Nominal()
        {
            Assert.Equal("1", "{x}".FormatWithDictionary(new Dictionary<string, object>() {{"x", 1}}));
            Assert.Equal("1 ystr",
                "{x} {y}".FormatWithDictionary(new Dictionary<string, object>() {{"x", 1}, {"y", "ystr"}}));
            Assert.Equal("1 y ystr",
                "{x} y {y}".FormatWithDictionary(new Dictionary<string, object>() {{"x", 1}, {"y", "ystr"}}));
            Assert.Throws<FormatException>(
                () => "{x} y {y}".FormatWithDictionary(new Dictionary<string, object>() {{"x", 1}}));
            Assert.Throws<FormatException>(() =>
                "{x}".FormatWithDictionary(new Dictionary<string, object>() {{"y", "ystr"}}));
            Assert.Equal("ystr",
                "{y}".FormatWithDictionary(new Dictionary<string, object>() {{"x", 1}, {"y", "ystr"}}));
        }
    }
}