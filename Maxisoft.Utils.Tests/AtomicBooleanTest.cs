using System;
using Xunit;

namespace Maxisoft.Utils.Tests
{
    public class AtomicBooleanTest
    {
        [Fact]
        public void TestAtomicBoolean()
        {
            AtomicBoolean b = new AtomicBoolean();
            Assert.False(b.Value);

            b = new AtomicBoolean(false);
            Assert.False(b.Value);

            b = new AtomicBoolean(true);
            Assert.True(b.Value);

            //when Value is already true, FalseToTrue fails
            b.Value = true;
            Assert.False(b.FalseToTrue());
            Assert.True(b.Value);

            //when Value is already false, TrueToFalse fails
            b.Value = false;
            Assert.False(b.TrueToFalse());
            Assert.False(b.Value);

            //Value not changed if SetWhen fails
            b.Value = false;
            Assert.False(b.SetWhen(true, true));
            Assert.False(b.Value);

            //Value not changed if SetWhen fails
            b.Value = true;
            Assert.False(b.SetWhen(false, false));
            Assert.True(b.Value);
        }
    }
}