using System;
using System.Runtime.CompilerServices;
using Maxisoft.Utils.TimeRanges;
using Xunit;


namespace Maxisoft.Utils.Tests.TimeRanges
{
    public class ParserTest
    {
        [Fact]
        public void TestParseTimeRange()
        {
            TimeRange tr;
            Assert.True(Parser.TryParse("8:30 -> 8:50:30", out tr));
            Assert.Equal(new TimeRange(new TimePoint(8, 30), new TimePoint(8, 50, 30)), tr);

            Assert.True(Parser.TryParse("8->8:50:30", out tr));
            Assert.Equal(new TimeRange(new TimePoint(8), new TimePoint(8, 50, 30)), tr);

            Assert.True(Parser.TryParse("8:20:40 -> 8:50:30", out tr));
            Assert.Equal(new TimeRange(new TimePoint(8, 20, 40), new TimePoint(8, 50, 30)), tr);

            Assert.True(Parser.TryParse("00:20:40 ->8:50:30", out tr));
            Assert.Equal(new TimeRange(new TimePoint(0, 20, 40), new TimePoint(8, 50, 30)), tr);

            Assert.True(Parser.TryParse("0:20:40-> 8:50:30", out tr));
            Assert.Equal(new TimeRange(new TimePoint(0, 20, 40), new TimePoint(8, 50, 30)), tr);

            Assert.True(Parser.TryParse("18:20:40\t-> \t8:50:30", out tr));
            Assert.Equal(new TimeRange(new TimePoint(18, 20, 40), new TimePoint(8, 50, 30)), tr);

            Assert.False(Parser.TryParse("", out tr));
            Assert.False(Parser.TryParse("8", out tr));
            Assert.False(Parser.TryParse("8:0:0", out tr));
            Assert.False(Parser.TryParse("8:0:0", out tr));
            Assert.False(Parser.TryParse("8:300:0 -> 8:50:30", out tr));
            Assert.False(Parser.TryParse("8>-8:50:30", out tr));
            Assert.False(Parser.TryParse("8:0:0-8:50:30", out tr));
            Assert.False(Parser.TryParse("8:30 -> 8:50:30:50", out tr));
            Assert.False(Parser.TryParse("8:f -> 8:50", out tr));
            Assert.False(Parser.TryParse("8:f -> 8:50", out tr));
        }
    }
}