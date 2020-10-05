using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Maxisoft.Utils.Collections.Spans;
using Xunit;

namespace Maxisoft.Utils.Tests.Collections.Spans
{
    public class SpanDictTests
    {
        [Fact]
        public void Test_strings()
        {
            Span<KeyValuePair<string, string>> rawPairs = new KeyValuePair<string, string>[128];

            var dict = new SpanDict<string, string>(rawPairs);
            dict["0"] = "5";
        }

        [Fact]
        public void Test_Basics()
        {
            Span<long> rawPairs = stackalloc long[100];

            var dict = SpanDict<int, long>.CreateFromBuffer(rawPairs);
            dict[0] = 5;

            Assert.Equal(5, dict[0]);
            foreach (var (key, value) in dict)
            {
                Assert.Equal(0, key);
                Assert.Equal(5, value);
            }

            dict[0] = 3;
            foreach (var (key, value) in dict)
            {
                Assert.Equal(0, key);
                Assert.Equal(3, value);
            }

            dict[5] = 0;
            dict[1 << 31] = 8;

            Assert.Equal(3, dict.Count);
            foreach (var (key, value) in dict)
            {
                var expected = key switch
                {
                    0 => 3,
                    5 => 0,
                    1 << 31 => 8,
                    _ => throw new ArgumentException(null, nameof(key))
                };
                Assert.Equal(expected, value);
            }

            dict.Remove(0);
        }
    }
}