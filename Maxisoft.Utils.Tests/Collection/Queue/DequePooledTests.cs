using System.Linq;
using Maxisoft.Utils.Collection;
using Maxisoft.Utils.Collection.Queue;
using Xunit;

namespace Maxisoft.Utils.Tests.Collection.Queue
{
    public class DequePooledTests
    {
        [Fact]
        public void TestDequePooled()
        {
            var q = new DequePooled<double>();
            Assert.Empty(q);
            Assert.Equal(q.ChunkSize, q.OptimalChunkSize());
            var adversarial = new LinkedListAsIList<double>();

            void CheckEqualsHeuristicPerformance()
            {
                Assert.Equal(adversarial.Count,q.Count);
                if (q.Count < 5)
                {
                    Assert.Equal(adversarial, q);
                }
                else
                {
                    Assert.Equal(adversarial.First?.Value, q.Front());
                    Assert.Equal(adversarial.Last?.Value, q.Back());
                }
            }
            
            for (var i = 0; i < q.ChunkSize * 2; i++)
            {
                q.PushBack(i);
                adversarial.AddLast(i);
                CheckEqualsHeuristicPerformance();
                q.PushFront(-i);
                adversarial.AddFirst(-i);
                CheckEqualsHeuristicPerformance();
            }
            
            Assert.Equal(adversarial, q);

            while (adversarial.Any())
            {
                q.PopFront();
                adversarial.RemoveFirst();
                CheckEqualsHeuristicPerformance();

                q.PopBack();
                adversarial.RemoveLast();
                CheckEqualsHeuristicPerformance();
            }
            
            Assert.Equal(adversarial, q);
        }
    }
}