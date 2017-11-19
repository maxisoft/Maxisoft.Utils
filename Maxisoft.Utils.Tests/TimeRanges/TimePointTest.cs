using System;
using Maxisoft.Utils.TimeRanges;
using Xunit;

namespace Maxisoft.Utils.Tests.TimeRanges
{
    public class TimePointTest
    {
        [Fact]
        public void TestBasicTimepoint()
        {
            var tp = new TimePoint();
            Assert.Equal(0, tp.TotalSeconds);
            Assert.Equal(default(TimePoint), tp);
            Assert.Equal(1, TimePoint.Unit.TotalSeconds);
        }
        
        [Fact]
        public void TestTimepointConstructor()
        {
            var @default = new TimePoint();
            Assert.Equal(0, @default.TotalSeconds);
            
            var hourOnly = new TimePoint(5);
            Assert.Equal(5, hourOnly.Hours);
            Assert.Equal(0, hourOnly.Minutes);
            Assert.Equal(0, hourOnly.Seconds);
            Assert.Equal(5 * 24 *60, hourOnly.TotalSeconds);
            
            var details = new TimePoint(3, 5, 30);
            Assert.Equal(3, details.Hours);
            Assert.Equal(5, details.Minutes);
            Assert.Equal(30, details.Seconds);
            Assert.Equal(3 * 24 *60 + 5 * 60 + 30, details.TotalSeconds);
        }
        
        [Fact]
        public void TestTimepointConstructorThrowing()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimePoint(-5));
            try
            {
                var _ = new TimePoint(-5);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Assert.Equal("h", e.ParamName);
            }
            
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimePoint(24));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimePoint(0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimePoint(0, 60));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimePoint(0, 0, 850));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimePoint(0, 0, 60));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimePoint(0, 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimePoint(-1, -1, -1));
        }

        [Fact]
        public void TestTimepointEquality()
        {
            var @default = new TimePoint();
            Assert.Equal(TimePoint.Zero, @default);
            Assert.StrictEqual(TimePoint.Zero, @default);
            Assert.Equal(TimePoint.Zero.GetHashCode(), @default.GetHashCode());

            var someDate = new TimePoint(5, 30, 58);
            var someDateCpy = new TimePoint(5, 30, 58);
            var anotherDate = new TimePoint(8, 30);
            Assert.Equal(someDate, someDateCpy);
            Assert.StrictEqual(someDate, someDateCpy);
            
            Assert.NotEqual(someDate, anotherDate);
            Assert.NotStrictEqual(someDate, anotherDate);
            Assert.NotEqual(someDate.TotalSeconds, anotherDate.TotalSeconds);
            Assert.NotEqual(someDate.GetHashCode(), anotherDate.GetHashCode());
        }

        [Fact]
        public void TestTimePointCompare()
        {
            var someDate = new TimePoint(5, 30, 58);
            var someDateCpy = new TimePoint(5, 30, 58);
            var anotherDate = new TimePoint(8, 30);
            Assert.True(someDate < anotherDate);
            Assert.True(someDate <= anotherDate);
            Assert.False(anotherDate < someDate);
            Assert.False(anotherDate <= someDate);
            Assert.True(someDateCpy <= someDate);
            Assert.True(someDate <= someDateCpy);
            Assert.True(someDate <= someDateCpy);
            Assert.False(someDate < someDateCpy);
            
            Assert.False(someDate > someDateCpy);
            Assert.True(someDate >= someDateCpy);
            Assert.False(someDateCpy > someDate);
            Assert.True(someDateCpy >= someDate);
            
            Assert.Equal(0, someDate.CompareTo(someDateCpy));
            Assert.Equal(0, someDateCpy.CompareTo(someDate));
            Assert.True(someDate.CompareTo(anotherDate) < 0);
            Assert.True(anotherDate.CompareTo(someDate) > 0);
        }
        
        [Fact]
        public void TestTimePointAddition()
        {
            var someDate = new TimePoint(5, 30, 58);
            var anotherDate = new TimePoint(8, 30);
            var sum = someDate + anotherDate;
            Assert.Equal(someDate.TotalSeconds + anotherDate.TotalSeconds, sum.TotalSeconds);
            Assert.Equal(someDate + anotherDate, sum);
        }
        
        [Fact]
        public void TestTimePointSubscraction()
        {
            var someDate = new TimePoint(5, 30, 58);
            var anotherDate = new TimePoint(8, 30);
            var sum = anotherDate - someDate;
            Assert.Equal(anotherDate.TotalSeconds - someDate.TotalSeconds, sum.TotalSeconds);
            Assert.Equal(anotherDate - someDate, sum);
        }
        
        [Fact]
        public void TestTimePointToString()
        {
            var someDate = new TimePoint(5, 30, 58);
            Assert.Equal("TimePoint(Hours: 5, Minutes: 30, Seconds: 58)", someDate.ToString());
        }
    }
}