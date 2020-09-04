using System;
using Maxisoft.Utils.TimeRanges;
using Xunit;

namespace Maxisoft.Utils.Tests.TimeRanges
{
    public class TimeRangeTest
    {
        [Fact]
        public void TestBasicTimepoint()
        {
            var range = new TimeRange(TimePoint.FromSeconds(30), TimePoint.FromSeconds(60));
            Assert.True(range.IsValid());
            Assert.False(range.IsEmpty());
            
            var @default = default(TimeRange);
            Assert.True(@default.IsEmpty());
            Assert.True(@default.IsValid());
        }
        
        [Fact]
        public void TestOverlapsWithRange()
        {
            var range = new TimeRange(TimePoint.FromSeconds(30), TimePoint.FromSeconds(60));
            Assert.True(range.Overlaps(range));
            
            var @default = default(TimeRange);
            Assert.True(@default.Overlaps(@default));
            Assert.False(range.Overlaps(@default));
            
            var range2 = new TimeRange(TimePoint.FromSeconds(0), TimePoint.FromSeconds(80));
            Assert.True(range2.Overlaps(range));
            Assert.True(range.Overlaps(range2));
            
            var range3 = new TimeRange(TimePoint.FromSeconds(0), TimePoint.FromSeconds(30));
            Assert.True(range.Overlaps(range3));
            Assert.True(range3.Overlaps(range));
            
            var range4 = new TimeRange(TimePoint.FromSeconds(180), TimePoint.FromSeconds(300));
            Assert.False(range.Overlaps(range4));
            Assert.False(range4.Overlaps(range));
            
            var range5 = new TimeRange(TimePoint.FromSeconds(45), TimePoint.FromSeconds(50));
            Assert.True(range.Overlaps(range5));
            Assert.True(range5.Overlaps(range));
            
            var range6 = new TimeRange(TimePoint.FromSeconds(50), TimePoint.FromSeconds(180));
            Assert.True(range.Overlaps(range6));
            Assert.True(range6.Overlaps(range));
            
            var range7 = new TimeRange(TimePoint.FromSeconds(60), TimePoint.FromSeconds(180));
            Assert.True(range.Overlaps(range7));
            Assert.True(range7.Overlaps(range));
            
            var range8 = new TimeRange(TimePoint.FromSeconds(0), TimePoint.FromSeconds(29));
            Assert.False(range.Overlaps(range8));
            Assert.False(range8.Overlaps(range));
        }

        [Fact]
        public void TestOverlapsWithTimePoint()
        {
            var range = new TimeRange(TimePoint.FromSeconds(30), TimePoint.FromSeconds(60));
            Assert.True(range.Overlaps(range.From));
            Assert.True(range.Overlaps(range.To));
            
            Assert.True(range.Overlaps(TimePoint.FromSeconds(45)));
            Assert.True(range.Overlaps(TimePoint.FromSeconds(31)));
            Assert.True(range.Overlaps(TimePoint.FromSeconds(59)));
            Assert.False(range.Overlaps(TimePoint.FromSeconds(61)));
            Assert.False(range.Overlaps(TimePoint.FromSeconds(29)));
            Assert.False(range.Overlaps(TimePoint.FromSeconds(0)));
        }
    }
}