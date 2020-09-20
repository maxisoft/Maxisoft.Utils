using System.Collections.Specialized;
using System.Linq;
using Maxisoft.Utils.Collection.Dictionary;
using Maxisoft.Utils.Collection.Dictionary.Specialized;
using Xunit;

namespace Maxisoft.Utils.Tests.Collection.Dictionary.Specialized
{
    public class OrderedLinkedListDictionaryTests
    {
        [Fact]
        public void Test_Adversarial()
        {
            var adversarial = new OrderedDictionary();
            var d = new OrderedLinkedListDictionary<string, object>();
            const int maxSize = 128;

            // build a ordered dict
            for (var i = 0; i < maxSize; i++)
            {
                var key = $"{i + 1}";
                Assert.Equal(adversarial.Contains(key), d.ContainsKey(key));
                var obj = new object();
                d.Add(key, obj);
                adversarial.Add(key, obj);

                Assert.Equal(adversarial.Contains(key), d.ContainsKey(key));

                Assert.Equal(adversarial.Keys.Cast<string>(), d.Keys);
                Assert.Equal(adversarial.Values.Cast<object>(), d.Values);
            }

            //delete 1/3 of data
            for (var i = maxSize - 1; i >= 0; i--)
            {
                if (i % 3 != 0) continue;
                var key = $"{i + 1}";
                Assert.Equal(adversarial.Contains(key), d.ContainsKey(key));


                if (i % 6 != 0)
                {
                    adversarial.RemoveAt(i * 2 / 3);
                    d.RemoveAt(i * 2 / 3);
                }
                else
                {
                    adversarial.Remove(key);
                    d.Remove(key);
                }


                Assert.Equal(adversarial.Contains(key), d.ContainsKey(key));

                Assert.Equal(adversarial.Keys.Cast<string>(), d.Keys);
                Assert.Equal(adversarial.Values.Cast<object>(), d.Values);
            }
        }

        [Fact]
        public void Test_Deque_Methods()
        {
            var objectPool = Enumerable.Range(0, 4).Select(i => new object()).ToArray();
            var d = new OrderedLinkedListDictionary<string, object>()
            {
                {"zero", objectPool[0]},
                {"one", objectPool[1]},
                {"two", objectPool[2]},
                {"three", objectPool[3]},
            };

            {
                var marker = new object();
                d.PushFront("front", marker);
                Assert.Equal(marker, d[0]);
                
                var expected = new OrderedLinkedListDictionary<string, object>()
                {
                    {"front", marker},
                    {"zero", objectPool[0]},
                    {"one", objectPool[1]},
                    {"two", objectPool[2]},
                    {"three", objectPool[3]},
                };


                Assert.Equal(marker, d[0]);
                Assert.Equal(marker, d["front"]);
                Assert.Equal(expected, d);

                var res = d.TryPopFront(out var pair);
                Assert.True(res);
                Assert.Equal("front", pair.Key);
                Assert.Same(marker, pair.Value);
            
                expected = new OrderedLinkedListDictionary<string, object>()
                {
                    {"zero", objectPool[0]},
                    {"one", objectPool[1]},
                    {"two", objectPool[2]},
                    {"three", objectPool[3]},
                };
            
                Assert.Equal(expected, d);
            }


            {
                var marker = new object();

                d.PushBack("back", marker);
                var expected = new OrderedLinkedListDictionary<string, object>()
                {
                    {"zero", objectPool[0]},
                    {"one", objectPool[1]},
                    {"two", objectPool[2]},
                    {"three", objectPool[3]},
                    {"back", marker},
                };


                Assert.Equal(marker, d[^1]);
                Assert.Equal(marker, d["back"]);
                Assert.Equal(expected, d);

                var res = d.TryPopBack(out var pair);
                Assert.True(res);
                Assert.Equal("back", pair.Key);
                Assert.Same(marker, pair.Value);
            
                expected = new OrderedLinkedListDictionary<string, object>()
                {
                    {"zero", objectPool[0]},
                    {"one", objectPool[1]},
                    {"two", objectPool[2]},
                    {"three", objectPool[3]},
                };
            
                Assert.Equal(expected, d);
            }

            var removeNum = d.Count;
            for (var i = 0; i < removeNum; i++)
            {
                Assert.True(d.TryPopFront(out _));
            }

            Assert.True(d.Count == 0);
            Assert.False(d.TryPopFront(out _));
            Assert.False(d.TryPopBack(out _));
        }
    }
}