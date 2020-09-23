using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Maxisoft.Utils.Collections.Dictionaries;
using Maxisoft.Utils.Empty;
using Moq;
using Xunit;

namespace Maxisoft.Utils.Tests.Collections.Dictionaries
{
    public class DictionaryChainTests
    {
        [Fact]
        public void Test_DictionaryChain_Constructor_Throw_For_No_Dict()
        {
            Assert.Throws<InvalidOperationException>(() => new DictionaryChain<string, int>());
            Assert.Throws<IndexOutOfRangeException>(() =>
                new DictionaryChain<string, int>(new IDictionary<string, int>[0]));
        }

        [Fact]
        public void Test_Querying_Simple()
        {
            var evenMock = new Mock<VirtualDictionaryWrapper<string, int, OrderedDictionary<string, int>>>
                {CallBase = true};
            evenMock.Object.Add("zero", 0);
            evenMock.Object.Add("two", 2);
            evenMock.Object.Add("four", 4);
            evenMock.Invocations.Clear();

            var even = evenMock.Object;
            var odd = new OrderedDictionary<string, int>
            {
                {"one", 1},
                {"three", 3},
                {"five", 5}
            };

            var empty = new NoOpDictionary<string, int>();
            var chain = new DictionaryChain<string, int>(even, odd, empty);
            Assert.Equal(6, chain.Count);
            Assert.Equal(3, chain.DictionariesChain.Length);
            Assert.Same(even, chain.DictionariesChain.ToArray()[0]);
            var expected = new OrderedDictionary<string, int>
            {
                {"zero", 0},
                {"two", 2},
                {"four", 4},
                {"one", 1},
                {"three", 3},
                {"five", 5}
            };

            Assert.Equal(expected, chain);
            Assert.Equal(expected.ToArray(), chain.ToArray());

            Assert.Equal(expected.Keys, chain.Keys);
            Assert.Equal(expected.Keys.ToArray(), chain.Keys.ToArray());

            Assert.Equal(expected.Values, chain.Values);
            Assert.Equal(expected.Values.ToArray(), chain.Values.ToArray());

            evenMock.Verify(mock => mock.Keys, Times.AtLeastOnce);
            evenMock.Verify(mock => mock.GetEnumerator(), Times.AtLeastOnce);

            Assert.Equal(0, chain["zero"]);
            Assert.Equal(5, chain["five"]);

            foreach (var pair in evenMock.Object)
            {
                Assert.True(chain.TryGetValue(pair.Key, out var actual));
                Assert.True(chain.ContainsKey(pair.Key));
                Assert.Contains(pair, chain);
                Assert.Equal(pair.Value, actual);
                Assert.Equal(pair.Value, chain[pair.Key]);
            }

            foreach (var pair in odd)
            {
                Assert.True(chain.TryGetValue(pair.Key, out var actual));
                Assert.True(chain.ContainsKey(pair.Key));
                Assert.Contains(pair, chain);
                Assert.Equal(pair.Value, actual);
                Assert.Equal(pair.Value, chain[pair.Key]);
            }

            Assert.False(chain.TryGetValue("non existing", out _));
            Assert.False(chain.ContainsKey("non existing"));
            Assert.DoesNotContain(new KeyValuePair<string, int>("non existing", -1), chain);
        }

        [Fact]
        public void Test_Editing_With_Collision_Between_Dictionaries()
        {
            var evenMock = new Mock<VirtualDictionaryWrapper<string, int, OrderedDictionary<string, int>>>
                {CallBase = true};
            evenMock.Object.Add("zero", 0);
            evenMock.Object.Add("two", 2);
            evenMock.Object.Add("four", 4);
            evenMock.Invocations.Clear();

            var even = evenMock.Object;
            var odd = new OrderedDictionary<string, int>
            {
                {"zero", -1}, // the collision
                // this pair must be ignored by the chain by default
                {"one", 1},
                {"three", 3},
                {"five", 5}
            };

            var empty = new NoOpDictionary<string, int>();

            var chain = new DictionaryChain<string, int>(even, odd, empty);
            Assert.Equal(6, chain.Count);
            var expected = new OrderedDictionary<string, int>
            {
                {"zero", 0},
                {"two", 2},
                {"four", 4},
                {"one", 1},
                {"three", 3},
                {"five", 5}
            };

            Assert.Equal(expected, chain);

            // Add a new pair ("six", 6)
            {
                evenMock.Invocations.Clear();
                chain.Add("six", 6);
                evenMock.Verify(mock => mock.Add("six", 6), Times.Once);
                evenMock.VerifyNoOtherCalls();
                expected.Insert(3, "six", 6);
                Assert.Equal(expected, chain);
                evenMock.Invocations.Clear();
            }

            // Re-add pair ("six", 6)
            {
                Assert.Contains(new KeyValuePair<string, int>("six", 6), chain);
                Assert.Throws<ArgumentException>(() => chain.Add("six", 6));
                Assert.Throws<ArgumentException>(() => chain.Add(new KeyValuePair<string, int>("six", 6)));
                evenMock.Invocations.Clear();
            }


            // Update pair ("six", -6)
            {
                chain["six"] = -6;
                evenMock.VerifySet(mock => mock["six"] = -6, Times.Once);
                evenMock.VerifyNoOtherCalls();
                expected["six"] = -6;
                Assert.Equal(expected, chain);
                evenMock.Invocations.Clear();
            }

            // Set a colliding pair ("one, -1)
            {
                chain["one"] = -1;
                evenMock.VerifySet(mock => mock["one"] = -1, Times.Once);
                evenMock.VerifyNoOtherCalls();

                expected.Remove("one");
                expected.Insert(4, "one", -1);
                Assert.Equal(expected, chain);
                evenMock.Invocations.Clear();

                Assert.Equal(1, odd["one"]); // odd's pair ("one, 1) not changed
                evenMock.Invocations.Clear();
            }

            // Remove the colliding pair
            {
                Assert.True(chain.Remove("one"));
                evenMock.Verify(mock => mock.Remove("one"), Times.Once);
                evenMock.VerifyNoOtherCalls();
                Assert.False(chain.Remove("one"));
                evenMock.Verify(mock => mock.Remove("one"), Times.Exactly(2));
                evenMock.VerifyNoOtherCalls();
                expected.Move(expected.IndexOf("one"), 4);
                expected["one"] = 1;
                Assert.Equal(expected, chain);
                evenMock.Invocations.Clear();
            }

            // Clear the chain
            {
                Assert.Throws<InvalidOperationException>(() => chain.Clear());
                evenMock.VerifyNoOtherCalls();

                odd.Clear();
                expected = expected.Take(even.Count).ToOrderedDictionary();
                Assert.Equal(expected, chain);

                evenMock.Invocations.Clear();

                chain.Clear();
                evenMock.Verify(mock => mock.Clear(), Times.Once);
                evenMock.VerifyNoOtherCalls();

                Assert.Empty(chain);
            }
        }


        [Fact]
        public void Test_Modifications_With_AllMethods()
        {
            var evenMock = new Mock<VirtualDictionaryWrapper<string, int, OrderedDictionary<string, int>>>
                {CallBase = true};
            evenMock.Object.Add("zero", 0);
            evenMock.Object.Add("two", 2);
            evenMock.Object.Add("four", 4);
            evenMock.Invocations.Clear();

            var even = evenMock.Object;
            var odd = new OrderedDictionary<string, int>
            {
                {"zero", -1}, // the collision
                // this pair must be ignored by the chain by default
                {"one", 1},
                {"three", 3},
                {"five", 5}
            };

            var wasEmptyMock = new Mock<VirtualDictionaryWrapper<string, int, OrderedDictionary<string, int>>>
                {CallBase = true};

            var wasEmpty = wasEmptyMock.Object;
            var chain = new DictionaryChain<string, int>(even, odd, wasEmpty);
            Assert.Equal(6, chain.Count);
            var expected = new OrderedDictionary<string, int>
            {
                {"zero", 0},
                {"two", 2},
                {"four", 4},
                {"one", 1},
                {"three", 3},
                {"five", 5}
            };

            Assert.Equal(expected, chain);

            // Add a new pair ("six", 6) into all dict
            {
                evenMock.Invocations.Clear();
                wasEmptyMock.Invocations.Clear();
                chain.AddAll("six", 6);
                evenMock.Verify(mock => mock.Add("six", 6), Times.Once);
                evenMock.VerifyNoOtherCalls();
                wasEmptyMock.Verify(mock => mock.Add("six", 6), Times.Once);
                wasEmptyMock.VerifyNoOtherCalls();
                expected.Insert(3, "six", 6);
                Assert.Equal(expected, chain);
                Assert.Equal(chain.CountAll, even.Count + odd.Count + wasEmpty.Count);
                evenMock.Invocations.Clear();
                wasEmptyMock.Invocations.Clear();
            }

            // Re-add pair ("six", 6)
            {
                Assert.Contains(new KeyValuePair<string, int>("six", 6), chain);
                Assert.Throws<ArgumentException>(() => chain.Add("six", 6));
                Assert.Throws<ArgumentException>(() => chain.Add(new KeyValuePair<string, int>("six", 6)));
                evenMock.Invocations.Clear();
                wasEmptyMock.Invocations.Clear();
            }


            // Update/add pair ("six", -6)
            {
                chain.UpdateAll("six", -6);
                evenMock.VerifySet(mock => mock["six"] = -6, Times.Once);
                evenMock.VerifyNoOtherCalls();
                wasEmptyMock.VerifySet(mock => mock["six"] = -6, Times.Once);
                wasEmptyMock.VerifyNoOtherCalls();
                expected["six"] = -6;
                Assert.Equal(expected, chain);
                evenMock.Invocations.Clear();
                wasEmptyMock.Invocations.Clear();
            }

            // Set a colliding pair ("one, -1)
            {
                chain.UpdateAll("one", -1);
                evenMock.VerifySet(mock => mock["one"] = -1, Times.Once);
                evenMock.VerifyNoOtherCalls();
                wasEmptyMock.VerifySet(mock => mock["one"] = -1, Times.Once);
                wasEmptyMock.VerifyNoOtherCalls();

                expected.Remove("one");
                expected.Insert(4, "one", -1);
                Assert.Equal(expected, chain);
                evenMock.Invocations.Clear();

                Assert.Equal(-1, odd["one"]); // odd's pair ("one, 1) changed
                evenMock.Invocations.Clear();
                wasEmptyMock.Invocations.Clear();
            }

            // Remove the colliding pair
            {
                Assert.Equal(3, chain.RemoveAll("one"));
                evenMock.Verify(mock => mock.Remove("one"), Times.Once);
                evenMock.VerifyNoOtherCalls();
                wasEmptyMock.Verify(mock => mock.Remove("one"), Times.Once);
                wasEmptyMock.VerifyNoOtherCalls();
                Assert.Equal(0, chain.RemoveAll("one"));
                evenMock.Verify(mock => mock.Remove("one"), Times.Exactly(2));
                evenMock.VerifyNoOtherCalls();
                wasEmptyMock.Verify(mock => mock.Remove("one"), Times.Exactly(2));
                wasEmptyMock.VerifyNoOtherCalls();
                expected.Remove("one");
                Assert.Equal(expected, chain);
                evenMock.Invocations.Clear();
                wasEmptyMock.Invocations.Clear();
            }

            // Clear all the chain' dictionaries
            {
                Assert.Throws<InvalidOperationException>(() => chain.Clear());
                evenMock.VerifyNoOtherCalls();
                wasEmptyMock.VerifyNoOtherCalls();

                evenMock.Invocations.Clear();
                wasEmptyMock.Invocations.Clear();

                chain.ClearAll();
                evenMock.Verify(mock => mock.Clear(), Times.Once);
                evenMock.VerifyNoOtherCalls();
                wasEmptyMock.Verify(mock => mock.Clear(), Times.Once);
                evenMock.VerifyNoOtherCalls();

                Assert.Empty(chain);
            }
        }


        [Fact]
        public void Test_Querying_With_Collision_Between_Dictionaries()
        {
            var evenMock = new Mock<VirtualDictionaryWrapper<string, int, OrderedDictionary<string, int>>>
                {CallBase = true};
            evenMock.Object.Add("zero", 0);
            evenMock.Object.Add("two", 2);
            evenMock.Object.Add("four", 4);
            evenMock.Invocations.Clear();

            var even = evenMock.Object;
            var odd = new OrderedDictionary<string, int>
            {
                {"zero", -1}, // the collision
                // this pair must be ignored by the chain by default
                {"one", 1},
                {"three", 3},
                {"five", 5}
            };

            var empty = new NoOpDictionary<string, int>();

            var chain = new DictionaryChain<string, int>(even, odd, empty);
            Assert.Equal(6, chain.Count);
            var expected = new OrderedDictionary<string, int>
            {
                {"zero", 0},
                {"two", 2},
                {"four", 4},
                {"one", 1},
                {"three", 3},
                {"five", 5}
            };

            Assert.Equal(expected, chain);
            Assert.Equal(expected.ToArray(), chain.ToArray());

            Assert.Equal(expected.Keys, chain.Keys);
            Assert.Equal(expected.Keys.ToArray(), chain.Keys.ToArray());

            Assert.Equal(expected.Values, chain.Values);
            Assert.Equal(expected.Values.ToArray(), chain.Values.ToArray());

            evenMock.Verify(mock => mock.Keys, Times.AtLeastOnce);
            evenMock.Verify(mock => mock.GetEnumerator(), Times.AtLeastOnce);

            Assert.Equal(0, chain["zero"]);
            Assert.Equal(5, chain["five"]);

            foreach (var pair in evenMock.Object)
            {
                Assert.True(chain.TryGetValue(pair.Key, out var actual));
                Assert.True(chain.ContainsKey(pair.Key));
                Assert.Contains(pair, chain);
                Assert.Equal(pair.Value, actual);
                Assert.Equal(pair.Value, chain[pair.Key]);
            }

            foreach (var pair in odd)
            {
                Assert.True(chain.TryGetValue(pair.Key, out var actual));
                Assert.True(chain.ContainsKey(pair.Key));
                if (even.ContainsKey(pair.Key)) // => collision
                {
                    Assert.DoesNotContain(pair, chain);
                }
                else
                {
                    Assert.Contains(pair, chain);
                    Assert.Equal(pair.Value, actual);
                    Assert.Equal(pair.Value, chain[pair.Key]);
                }
            }

            Assert.False(chain.TryGetValue("non existing", out _));
            Assert.False(chain.ContainsKey("non existing"));
            Assert.DoesNotContain(new KeyValuePair<string, int>("non existing", -1), chain);
            Assert.DoesNotContain(new KeyValuePair<string, int>("zero", -1), chain);

            Assert.False(chain.Values.Contains(-1));
        }

        [Fact]
        public void Test_CopyTo()
        {
            var evenMock = new Mock<VirtualDictionaryWrapper<string, int, OrderedDictionary<string, int>>>
                {CallBase = true};
            evenMock.Object.Add("zero", 0);
            evenMock.Object.Add("two", 2);
            evenMock.Object.Add("four", 4);
            evenMock.Invocations.Clear();

            var even = evenMock.Object;
            var odd = new OrderedDictionary<string, int>
            {
                {"one", 1},
                {"three", 3},
                {"five", 5}
            };

            var expected = new OrderedDictionary<string, int>
            {
                {"zero", 0},
                {"two", 2},
                {"four", 4},
                {"one", 1},
                {"three", 3},
                {"five", 5}
            };


            var chain = new DictionaryChain<string, int>(even, odd);
            var chainCount = chain.Count; // save result as Count is expensive to compute
            Assert.Equal(6, chainCount);

            var arr = new KeyValuePair<string, int>[chainCount];
            chain.CopyTo(arr, 0);
            Assert.Throws<ArgumentOutOfRangeException>(() => chain.CopyTo(arr, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => chain.CopyTo(arr, -1));
            Assert.Equal(expected.ToArray(), arr);

            var limitedArray = new KeyValuePair<string, int>[chainCount - 1];
            Assert.Throws<ArgumentOutOfRangeException>(() => chain.CopyTo(limitedArray, 0));

            var bigArray = new KeyValuePair<string, int>[chainCount * 2];
            chain.CopyTo(bigArray, 0);
            Span<KeyValuePair<string, int>> bigSpan = bigArray;
            Assert.Equal(expected.ToArray(), bigSpan.Slice(0, chainCount).ToArray());

            chain.CopyTo(bigArray, chainCount);
            Assert.Equal(expected.ToArray(), bigSpan.Slice(chainCount, chainCount).ToArray());

            Assert.Throws<ArgumentOutOfRangeException>(() => chain.CopyTo(bigArray, chainCount + 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => chain.CopyTo(bigArray, -1));
        }

        /// <summary>
        ///     Used to mock most of <see cref="IDictionary{TKey,TValue}" /> methods
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TDictionary"></typeparam>
        public class VirtualDictionaryWrapper<TKey, TValue, TDictionary> : IDictionary<TKey, TValue>
            where TDictionary : class, IDictionary<TKey, TValue>, new()
        {
            public readonly TDictionary Value;

            public VirtualDictionaryWrapper(TDictionary value)
            {
                Value = value;
            }

            public VirtualDictionaryWrapper() : this(new TDictionary())
            {
            }

            public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                return Value.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable) Value).GetEnumerator();
            }

            public virtual void Add(KeyValuePair<TKey, TValue> item)
            {
                Value.Add(item);
            }

            public virtual void Clear()
            {
                Value.Clear();
            }

            public virtual bool Contains(KeyValuePair<TKey, TValue> item)
            {
                return Value.Contains(item);
            }

            public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            {
                Value.CopyTo(array, arrayIndex);
            }

            public virtual bool Remove(KeyValuePair<TKey, TValue> item)
            {
                return Value.Remove(item);
            }

            public virtual int Count => Value.Count;

            public virtual bool IsReadOnly => Value.IsReadOnly;

            public virtual void Add(TKey key, TValue value)
            {
                Value.Add(key, value);
            }

            public virtual bool ContainsKey(TKey key)
            {
                return Value.ContainsKey(key);
            }

            public virtual bool Remove(TKey key)
            {
                return Value.Remove(key);
            }

            public virtual bool TryGetValue(TKey key, out TValue value)
            {
                return Value.TryGetValue(key, out value);
            }

            public virtual TValue this[TKey key]
            {
                get => Value[key];
                set => Value[key] = value;
            }

            public virtual ICollection<TKey> Keys => Value.Keys;

            public virtual ICollection<TValue> Values => Value.Values;
        }
    }
}