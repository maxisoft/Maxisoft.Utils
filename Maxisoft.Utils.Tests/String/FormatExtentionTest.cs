using System;
using System.Collections.Generic;
using Maxisoft.Utils.String;
using Xunit;

namespace Maxisoft.Utils.Tests.String
{
    public class FormatExtentionTest
    {
        [Fact]
        public void FormatFromDictionaryTest_Nominal()
        {
            Assert.Equal("1", "{x}".FormatFromDictionary(new Dictionary<string, object>() {{"x", 1}}));
            Assert.Equal("1 ystr",
                "{x} {y}".FormatFromDictionary(new Dictionary<string, object>() {{"x", 1}, {"y", "ystr"}}));
            Assert.Equal("1 y ystr",
                "{x} y {y}".FormatFromDictionary(new Dictionary<string, object>() {{"x", 1}, {"y", "ystr"}}));
            Assert.Throws<FormatException>(
                () => "{x} y {y}".FormatFromDictionary(new Dictionary<string, object>() {{"x", 1}}));
            Assert.Throws<FormatException>(() =>
                "{x}".FormatFromDictionary(new Dictionary<string, object>() {{"y", "ystr"}}));
            Assert.Equal("ystr",
                "{y}".FormatFromDictionary(new Dictionary<string, object>() {{"x", 1}, {"y", "ystr"}}));
        }
    }
}