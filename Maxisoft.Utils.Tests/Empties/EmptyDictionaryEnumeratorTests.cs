using System;
using Maxisoft.Utils.Empties;
using Xunit;

namespace Maxisoft.Utils.Tests.Empties
{
    public class EmptyDictionaryEnumeratorTests
    {
        [Fact]
        public void Tests()
        {
            var enumerator = new EmptyDictionaryEnumerator();
            enumerator.Reset();
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
            Assert.Throws<InvalidOperationException>(() => enumerator.Entry);
            Assert.Throws<InvalidOperationException>(() => enumerator.Key);
            Assert.Throws<InvalidOperationException>(() => enumerator.Value);
        }
    }
}