using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Maxisoft.Utils.Algorithm;
using Maxisoft.Utils.Collection.Dictionary;
using Xunit;
using Xunit.Sdk;

namespace Maxisoft.Utils.Tests.Collection.Dictionary
{
    public class OrderedDictionaryTests
    {
        private readonly IReadOnlyList<string> numberToString = new[]
        {
            "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve",
            "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen"
        };


        [Fact]
        public void Test_Adversarial()
        {
            var adversarial = new OrderedDictionary();
            var d = new OrderedDictionary<string, object>();
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
        public void Test_Constructor()
        {
            var objectPool = Enumerable.Range(0, 4).Select(i => new object()).ToArray();
            var d = new OrderedDictionary<string, object>()
            {
                {"zero", objectPool[0]},
                {"one", objectPool[1]},
                {"two", objectPool[2]},
                {"three", objectPool[3]},
            };

            Assert.Equal(objectPool.Length, d.Count);
            Assert.Equal(objectPool, d.Values);

            Assert.Equal(d["zero"], d[0]);
            Assert.Equal(d["one"], d[1]);
            Assert.Equal(d["two"], d[2]);
            Assert.Equal(d["three"], d[3]);

            for (var i = 0; i < objectPool.Length; i++)
            {
                var obj = objectPool[i];
                Assert.Equal(obj, d[i]);
            }
        }


        [Fact]
        public void Test_IndexOf()
        {
            var objectPool = Enumerable.Range(0, 4).Select(i => new object()).ToArray();
            var d = new OrderedDictionary<string, object>()
            {
                {"zero", objectPool[0]},
                {"one", objectPool[1]},
                {"two", objectPool[2]},
                {"three", objectPool[3]},
            };

            Assert.Equal(objectPool.Length, d.Count);

            for (var i = 0; i < objectPool.Length; i++)
            {
                var obj = objectPool[i];
                Assert.Equal(i, d.IndexOf(numberToString[i]));
                Assert.Equal(i, d.IndexOf(d[i]));
                Assert.Equal(i, d.IndexOf(in obj));
            }

            Assert.Equal(-1, d.IndexOf("nonExisting"));
            Assert.Equal(-1, d.IndexOf(new object()));
        }

        [Fact]
        public void Test_IndexOf_Same_Key_Value_Type()
        {
            const int numElement = 4;
            var d = new OrderedDictionary<string, string>()
            {
                {"zero", "0"},
                {"one", "1"},
                {"two", "2"},
                {"three", "3"},
            };

            Assert.Equal(numElement, d.Count);

            for (var i = 0; i < numElement; i++)
            {
                Assert.Equal(i, d.IndexOf(value: d[i]));
                Assert.Equal(i, d.IndexOf(key: numberToString[i]));
            }

            var nonExistingValue = d.At(0).Key;
            var nonExistingKey = d.At(0).Value;
            Assert.NotEqual(nonExistingKey, nonExistingValue);

            Assert.Equal(-1, d.IndexOf(key: nonExistingKey));
            Assert.Equal(-1, d.IndexOf(value: nonExistingValue));
        }

        [Fact]
        public void Test_Clear()
        {
            var objectPool = Enumerable.Range(0, 4).Select(i => new object()).ToArray();
            var d = new OrderedDictionary<string, object>()
            {
                {"zero", objectPool[0]},
                {"one", objectPool[1]},
                {"two", objectPool[2]},
                {"three", objectPool[3]},
            };

            Assert.Equal(objectPool.Length, d.Count);

            d.Clear();
            Assert.Empty(d);

            d.Add("add", objectPool[0]);
            Assert.NotEmpty(d);

            d.Clear();
            Assert.Empty(d);
        }


        [Fact]
        public void Test_Contains()
        {
            var objectPool = Enumerable.Range(0, 4).Select(i => new object()).ToArray();
            var d = new OrderedDictionary<string, object>()
            {
                {"zero", objectPool[0]},
                {"one", objectPool[1]},
                {"two", objectPool[2]},
                {"three", objectPool[3]},
            };

            Assert.Equal(objectPool.Length, d.Count);

            for (int i = 0; i < d.Count; i++)
            {
                var key = numberToString[i];
                Assert.Contains(new KeyValuePair<string, object>(key, objectPool[i]), d);
            }

            Assert.DoesNotContain(new KeyValuePair<string, object>("nonExisting", objectPool[0]), d);
            Assert.DoesNotContain(new KeyValuePair<string, object>("nonExisting", "nonExisting"), d);
            Assert.DoesNotContain(new KeyValuePair<string, object>(d.At(0).Key, "nonExisting"), d);

            d = new OrderedDictionary<string, object>();
            Assert.DoesNotContain(new KeyValuePair<string, object>("zero", objectPool[0]), d);
        }


        [Theory]
        [InlineData(4, 0)]
        [InlineData(5, 1)]
        [InlineData(10, 5)]
        [InlineData(0, 0)]
        [InlineData(0, -1)]
        [InlineData(1, -1)]
        [InlineData(1, 2)]
        public void Test_CopyTo_Adversarial(int arrayLength, int arrayIndex)
        {
            var objectPool = Enumerable.Range(0, 4).Select(i => new object()).ToArray();
            var d = new OrderedDictionary<string, object>()
            {
                {"zero", objectPool[0]},
                {"one", objectPool[1]},
                {"two", objectPool[2]},
                {"three", objectPool[3]},
            };

            var adversarial = new OrderedDictionary()
            {
                {"zero", objectPool[0]},
                {"one", objectPool[1]},
                {"two", objectPool[2]},
                {"three", objectPool[3]},
            };

            Assert.Equal(adversarial.GetKeyValuePairEnumerator<string, object>(), d);

            var toArray = new KeyValuePair<string, object>[arrayLength];
            void TestCode() => d.CopyTo(toArray, arrayIndex);

            static IEnumerable<KeyValuePair<TKey, TValue>> GetKeyValuePairEnumerator<TKey, TValue>(
                IEnumerable enumerable)
            {
                static KeyValuePair<TKey, TValue> Cast(object entry)
                {
                    if (entry is DictionaryEntry de)
                    {
                        return new KeyValuePair<TKey, TValue>((TKey) de.Key, (TValue) de.Value);
                    }

                    if (entry is KeyValuePair<TKey, TValue> kv)
                    {
                        return kv;
                    }

                    if (entry is KeyValuePair<object, object> kvo)
                    {
                        return new KeyValuePair<TKey, TValue>((TKey) kvo.Key, (TValue) kvo.Value);
                    }

                    throw new ArgumentException();
                }

                return enumerable.Cast<object>().Select(Cast);
            }

            try
            {
                var adversarialResult = new object[arrayLength];
                // use a workaround since OrderedDictionary.CopyTo doesn't preserve order
                var workaround = GetKeyValuePairEnumerator<string, object>(adversarial);
                Array.Fill(adversarialResult, new KeyValuePair<string, object>());
                workaround.ToArray().CopyTo(adversarialResult, arrayIndex);

                TestCode();
                Assert.Equal(GetKeyValuePairEnumerator<string, object>(adversarialResult), toArray);
            }
            catch (Exception e) when (!(e is XunitException))
            {
                var expectedType = e.GetType();
                if (e is ArgumentOutOfRangeException)
                {
                    expectedType = typeof(ArgumentException);
                }

                var exception = Record.Exception(TestCode);
                Assert.True(exception.GetType().IsAssignableFrom(e.GetType()) ||
                            e.GetType().IsAssignableFrom(exception.GetType()));
            }
        }

        [Fact]
        public void Test_Add()
        {
            var objectPool = Enumerable.Range(0, 4).Select(i => new object()).ToArray();
            var d = new OrderedDictionary<string, object>()
            {
                {"zero", objectPool[0]},
                {"one", objectPool[1]},
                {"two", objectPool[2]},
                {"three", objectPool[3]},
            };
            var marker = new object();

            d.Add("four", marker);
            Assert.True(d.ContainsKey("four"));
            Assert.Equal(4, d.IndexOf("four"));
            Assert.Equal(4, d.IndexOf(in marker));

            Assert.Throws<ArgumentException>(() => d.Add("four", marker));
            Assert.Throws<ArgumentException>(() => d.Add("four", new object()));
            d.Add("another marker", marker);

            Assert.Equal(4, d.IndexOf("four"));
            Assert.Equal(5, d.IndexOf("another marker"));
            Assert.Equal(4, d.IndexOf(in marker));
            Assert.Equal(marker, d.At(5).Value);
            Assert.Equal(marker, d.At("another marker").Value);
        }

        [Fact]
        public void Test_Indexer()
        {
            var objectPool = Enumerable.Range(0, 4).Select(i => new object()).ToArray();
            var d = new OrderedDictionary<string, object>()
            {
                {"zero", objectPool[0]},
                {"one", objectPool[1]},
                {"two", objectPool[2]},
                {"three", objectPool[3]},
            };

            for (int i = 0; i < d.Count; i++)
            {
                Assert.Equal(objectPool[i], d[i]);
                Assert.Equal(objectPool[i], d[numberToString[i]]);
            }

            Assert.Throws<KeyNotFoundException>(() => d["not a valid key"]);
            var outOfBound = d.Count + 2;
            Assert.Throws<ArgumentOutOfRangeException>(() => d[outOfBound]);

            var marker = new object();
            d["four"] = marker;
            Assert.Equal(marker, d["four"]);
            Assert.Equal(marker, d[^1]);


            Assert.NotEqual(marker, d[0]);

            d[0] = marker;
            Assert.Equal(marker, d["zero"]);
            Assert.Equal(marker, d[0]);


            d["one"] = marker;
            Assert.Equal(marker, d["one"]);
            Assert.Equal(marker, d[1]);
        }

        [Fact]
        public void Test_Insert()
        {
            var objectPool = Enumerable.Range(0, 4).Select(i => new object()).ToArray();
            var d = new OrderedDictionary<string, object>()
            {
                {"zero", objectPool[0]},
                {"one", objectPool[1]},
                {"two", objectPool[2]},
                {"three", objectPool[3]},
            };

            var adversarial = new OrderedDictionary()
            {
                {"zero", objectPool[0]},
                {"one", objectPool[1]},
                {"two", objectPool[2]},
                {"three", objectPool[3]},
            };

            var marker = new object();
            d.Insert(d.Count, "four", marker);
            adversarial.Insert(adversarial.Count, "four", marker);


            Assert.Equal(marker, d["four"]);
            Assert.Equal(marker, d[^1]);
            Assert.Equal(marker, d[4]);

            Assert.Equal(adversarial.GetKeyValuePairEnumerator<string, object>(), d);


            marker = new object();
            d.Insert(0, "zerozero", marker);
            adversarial.Insert(0, "zerozero", marker);
            Assert.Equal(adversarial.GetKeyValuePairEnumerator<string, object>(), d);

            marker = new object();
            d.Insert(d.Count / 2, "middle", marker);
            adversarial.Insert(adversarial.Count / 2, "middle", marker);
            Assert.Equal(adversarial.GetKeyValuePairEnumerator<string, object>(), d);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                adversarial.Insert(adversarial.Count + 1, "error", marker));
            Assert.Throws<ArgumentOutOfRangeException>(() => d.Insert(d.Count + 1, "error", marker));

            Assert.Throws<ArgumentOutOfRangeException>(() => adversarial.Insert(-1, "error", marker));
            Assert.Throws<ArgumentOutOfRangeException>(() => d.Insert(-1, "error", marker));


            Assert.Throws<ArgumentException>(() => adversarial.Insert(0, "zero", marker));
            Assert.Throws<ArgumentException>(() => d.Insert(0, "zero", marker));
        }

        [Fact]
        public void Test_UpdateAt()
        {
            var objectPool = Enumerable.Range(0, 4).Select(i => new object()).ToArray();
            var d = new OrderedDictionary<string, object>()
            {
                {"zero", objectPool[0]},
                {"one", objectPool[1]},
                {"two", objectPool[2]},
                {"three", objectPool[3]},
            };

            var marker = new object();
            var res = d.UpdateAt("two", in marker);
            Assert.Equal("two", res);
            Assert.Equal(marker, d["two"]);
            Assert.Equal(marker, d[2]);

            marker = new object();
            res = d.UpdateAt(3, in marker);
            Assert.Equal("three", res);
            Assert.Equal(marker, d[3]);
            Assert.Equal(marker, d["three"]);


            marker = new object();
            Assert.Throws<KeyNotFoundException>(() => d.UpdateAt("non existing", in marker));

            marker = new object();
            Assert.Throws<ArgumentOutOfRangeException>(() => d.UpdateAt(-1, in marker));
            Assert.Throws<ArgumentOutOfRangeException>(() => d.UpdateAt(d.Count, in marker));
        }

        [Fact]
        [SuppressMessage("ReSharper", "ArgumentsStyleNamedExpression")]
        public void Test_Indexer_Same_Key_Value_Type()
        {
            const int size = 5;

            // Build a mapping such as {key = 2**i} and {value = i}
            // for i in range(0, size)
            var d = new OrderedDictionary<int, int>();
            for (var i = 0; i < size; i++)
            {
                var exp = 1 << i;
                d.Add(exp, i);
            }

            Assert.Equal(size, d.Count);

            for (var i = 0; i < size; ++i)
            {
                var exp = 1 << i;

                // to get value by key specify parameter name and we're good
                Assert.Equal(i, d[key: exp]);
                Assert.Equal(i, d.At(key: exp).Value);

                // or use At(in key) method
                Assert.Equal(i, d.At(in exp).Value);

                // to use index parameter name :
                Assert.Equal(i, d[index: i]);
                Assert.Equal(i, d.At(index: i).Value);
                Assert.Equal(exp, d.At(index: i).Key);
            }
        }

        [Fact]
        public void Test_Swap()
        {
            var objectPool = Enumerable.Range(0, 4).Select(i => new object()).ToArray();
            var d = new OrderedDictionary<string, object>()
            {
                {"zero", objectPool[0]},
                {"one", objectPool[1]},
                {"two", objectPool[2]},
                {"three", objectPool[3]},
            };

            d.Swap(0, 3);

            var expected = new OrderedDictionary<string, object>()
            {
                {"three", objectPool[3]},
                {"one", objectPool[1]},
                {"two", objectPool[2]},
                {"zero", objectPool[0]},
            };

            Assert.Equal(expected, d);
        }

        [Fact]
        public void Test_Move()
        {
            var objectPool = Enumerable.Range(0, 4).Select(i => new object()).ToArray();
            var d = new OrderedDictionary<string, object>()
            {
                {"zero", objectPool[0]},
                {"one", objectPool[1]},
                {"two", objectPool[2]},
                {"three", objectPool[3]},
            };

            d.Move(2, 0);

            var expected = new OrderedDictionary<string, object>()
            {
                {"two", objectPool[2]},
                {"zero", objectPool[0]},
                {"one", objectPool[1]},
                {"three", objectPool[3]},
            };

            Assert.Equal(expected, d);
        }
    }
}