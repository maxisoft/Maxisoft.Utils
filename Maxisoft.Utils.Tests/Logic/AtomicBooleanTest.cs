using Maxisoft.Utils.Logic;
using Xunit;

namespace Maxisoft.Utils.Tests.Logic
{
    public class AtomicBooleanTest
    {
        [Fact]
        public void TestAtomicBoolean()
        {
            var b = new AtomicBoolean();
            Assert.False(b.Value);

            b = new AtomicBoolean(false);
            Assert.False(b.Value);

            b = new AtomicBoolean(true);
            Assert.True(b.Value);

            //when TriValue is already true, FalseToTrue fails
            b.Value = true;
            Assert.False(b.FalseToTrue());
            Assert.True(b.Value);

            //when TriValue is already false, TrueToFalse fails
            b.Value = false;
            Assert.False(b.TrueToFalse());
            Assert.False(b.Value);

            //TriValue not changed if CompareExchange fails
            b.Value = false;
            Assert.False(b.CompareExchange(true, true));
            Assert.False(b.Value);

            //TriValue not changed if CompareExchange fails
            b.Value = true;
            Assert.False(b.CompareExchange(false, false));
            Assert.True(b.Value);
        }
    }
}