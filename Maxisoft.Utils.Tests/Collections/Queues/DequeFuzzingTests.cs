using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text.Json;
using Maxisoft.Utils.Collections;
using Maxisoft.Utils.Collections.Queues;
using Maxisoft.Utils.Logic;
using Maxisoft.Utils.Random;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Maxisoft.Utils.Tests.Collections.Queues
{
    public class DequeFuzzingTests
    {
        private const long DefaultChunkSize = 7;

        private const long DefaultSize = DefaultChunkSize;
        private readonly RandomThreadSafe _randomThreadSafe = new RandomThreadSafe();
        private readonly ITestOutputHelper _testOutputHelper;

        public DequeFuzzingTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [ClassData(typeof(DequeTests.DataGenDifferentSizesAndChunkSize))]
        public void Test_Both_PushFront_PushBack_Fuzzing(long size, long chunkSize)
        {
            var q = new Deque<int>(chunkSize);
            var reconstruction = new LinkedList<int>();
            for (var i = 0; i < size; i++)
            {
                Assert.Equal(i, q.LongLength);
                if ((_randomThreadSafe.Next() & 1) == 0)
                {
                    q.PushBack(i);
                    reconstruction.AddLast(i);
                }
                else
                {
                    q.PushFront(i);
                    reconstruction.AddFirst(i);
                }

                Assert.Equal(i + 1, q.LongLength);
                Assert.Equal(reconstruction, q);
            }

            //construct an array with the expected same elements
            var arr = size > 0 ? reconstruction.ToArray() : new int[0];

            Assert.Equal(size, q.LongLength);
            Assert.Equal(size, q.Length);
            Assert.Equal(arr, q.ToArray());
        }


        [Theory]
        [ClassData(typeof(DequeTests.DataGenDifferentSizesAndChunkSize))]
        public void Test_Serialization(long size, long chunkSize)
        {
            var q = new Deque<int>(chunkSize);
            for (var i = 0; i < size; i++)
            {
                q.PushBack(i);
            }

            using var stream = new MemoryStream();
            var formatter = new SoapFormatter();

            formatter.Serialize(stream, q);
            stream.Seek(0, 0);

            formatter = new SoapFormatter();

            var actual = (Deque<int>) formatter.Deserialize(stream);
            Assert.Equal(q.ChunkSize, actual.ChunkSize);
            Assert.Equal((IEnumerable<int>) q, actual);
        }

        [Theory]
        [ClassData(typeof(DequeTests.DataGenDifferentSizesAndChunkSize))]
        public void Test_JsonSerialization(long size, long chunkSize)
        {
            var q = new Deque<TriBool>(chunkSize);
            for (var i = 0; i < size; i++)
            {
                var value = (TriBoolValue) (_randomThreadSafe.Next() % 3);
                q.PushBack(new TriBool(value));
            }

            var data = JsonSerializer.Serialize(q);
            var reconstruction = JsonSerializer.Serialize(q.ToArray());
            Assert.Equal(reconstruction, data);

            var actual = JsonSerializer.Deserialize<Deque<TriBool>>(data);

            Assert.Equal(q, actual);
            //Can't test Equality for chunk size
        }

        [Theory]
        [ClassData(typeof(DataGenDifferentSizesAndChunkSizeWithFuzzingIndex))]
        public void TestRemove_Fuzzing(long size, long chunkSize, long index)
        {
            var q = new Deque<int>(chunkSize);
            for (var i = 0; i < size; i++)
            {
                q.PushBack(i);
            }

            //construct an array with the expected same elements
            var arr = size > 0 ? Enumerable.Range(0, (int) size).ToArray() : new int[0];

            Assert.Equal(size, q.LongLength);
            Assert.Equal(arr, q.ToArray());

            var expected = arr.ToList();
            var expectedResult = false;
            Exception error = null;
            var elementToRemove = arr[index];
            try
            {
                expectedResult = expected.Remove(elementToRemove);
            }
            catch (Exception e)
            {
                error = e;
            }

            if (error is null)
            {
                var res = q.Remove(arr[index]);
                Assert.Equal(expectedResult, res);
                Assert.Equal(expected, q);
            }
            else
            {
                Assert.Throws<Exception>(() => q.Remove(elementToRemove));
            }
        }

        [Theory]
        [ClassData(typeof(DataGenDifferentSizesAndChunkSizeWithFuzzingIndex))]
        public void TestRemoveAt_Fuzzing(long size, long chunkSize, long index)
        {
            var q = new Deque<int>(chunkSize);
            for (var i = 0; i < size; i++)
            {
                q.PushBack(i);
            }

            //construct an array with the expected same elements
            var arr = size > 0 ? Enumerable.Range(0, (int) size).ToArray() : new int[0];

            Assert.Equal(size, q.LongLength);
            Assert.Equal(arr, q.ToArray());

            var expected = arr.ToList();
            Exception error = null;
            var elementToRemove = arr[index];
            try
            {
                expected.RemoveAt((int) index);
            }
            catch (Exception e)
            {
                error = e;
            }

            if (error is null)
            {
                q.RemoveAt((int) index);
                Assert.Equal(expected, q);
            }
            else
            {
                Assert.Throws<Exception>(() => q.Remove(elementToRemove));
            }
        }

        [Theory]
        [ClassData(typeof(DataGenDifferentSizesAndChunkSizeWithFuzzingIndex))]
        public void TestContains_Fuzzing(long size, long chunkSize, long index)
        {
            var q = new Deque<int>(chunkSize);
            for (var i = 0; i < size; i++)
            {
                q.PushBack(i);
            }

            //construct an array with the expected same elements
            var arr = size > 0 ? Enumerable.Range(0, (int) size).ToArray() : new int[0];

            Assert.Equal(size, q.LongLength);
            Assert.Equal(arr, q.ToArray());

            var element = arr[index];

            Assert.Equal(arr.Contains(element), q.Contains(element));

            int nonExisting;
            unchecked
            {
                nonExisting = _randomThreadSafe.Next() * -1;
            }

            Assert.Equal(arr.Contains(nonExisting), q.Contains(nonExisting));
        }

        [Theory]
        [ClassData(typeof(DataGenDifferentSizesAndChunkSizeWithFuzzingIndex))]
        public void TestInsert_Fuzzing(long size, long chunkSize, long index)
        {
            var q = new Deque<int>(chunkSize);
            for (var i = 0; i < size; i++)
            {
                q.PushBack(i);
            }

            //construct an array with the expected same elements
            var arr = size > 0 ? Enumerable.Range(0, (int) size).ToArray() : new int[0];

            Assert.Equal(size, q.LongLength);
            Assert.Equal(arr, q.ToArray());

            var element = _randomThreadSafe.Next() * -1;

            q.Insert((int) index, element);
            var expected = arr.ToList();
            expected.Insert((int) index, element);

            Assert.Equal(q, expected);
        }

        [Theory]
        [ClassData(typeof(DataGenDifferentSizesAndChunkSizeWithFuzzingIndex))]
        public void TestPopBack_Fuzzing(long size, long chunkSize, long index)
        {
            var expected = new object();
            var q = new Deque<object>(chunkSize);
            for (var i = 0; i < size; i++)
            {
                q.PushBack(i == index ? expected : null);
            }

            Assert.Equal(size, q.Count);

            if (size > 0)
            {
                Assert.Same(expected, q[index]);
            }

            if (size > 0)
            {
                for (var i = 0; i < size - index - 1; i++)
                {
                    Assert.Null(q.PopBack());
                }

                Assert.Same(expected, q.PopBack());
            }
            else
            {
                Assert.Throws<InvalidOperationException>(() => q.PopBack());
            }

            Assert.Equal(index, q.LongLength);
        }

        [Theory]
        [ClassData(typeof(DataGenDifferentSizesAndChunkSizeWithFuzzingIndex))]
        public void TestPopFront_Fuzzing(long size, long chunkSize, long index)
        {
            var expected = new object();
            var q = new Deque<object>(chunkSize);
            for (var i = 0; i < size; i++)
            {
                q.PushBack(i == index ? expected : null);
            }

            Assert.Equal(size, q.Count);

            if (size > 0)
            {
                Assert.Same(expected, q[index]);
            }

            if (size > 0)
            {
                for (var i = 0; i < index; i++)
                {
                    Assert.Null(q.PopFront());
                }

                Assert.Same(expected, q.PopFront());
            }
            else
            {
                Assert.Throws<InvalidOperationException>(() => q.PopFront());
            }

            Assert.Equal(Math.Max(size - index - 1, 0), q.LongLength);
        }

        private static Action<(long index, T value)>[] BindActionSet<T>(Deque<T> q)
        {
            return new Action<(long index, T value)>[]
            {
                args => q.At(args.index),
                args => q.Front(),
                args => q.Back(),
                args => q.TryPeekFront(out _),
                args => q.TryPeekBack(out _),
                args => q.Insert((int) args.index, args.value),
                args => q.RemoveAt((int) args.index),
                args => q.PushBack(args.value),
                args => q.PushFront(args.value),
                args => q.PopBack(),
                args => q.PopFront(),
                args => q.TryPopBack(out _),
                args => q.TryPopFront(out _),
                args => q.TrimExcess(),
                args => q.IndexOf(args.value),
                // balance the number of addition/removal 
                args => q.PushBack(args.value),
                args => q.PushFront(args.value)
            };
        }

        private static Action<(long index, T value)>[] BindActionSet<T>(LinkedList<T> l)
        {
            return new Action<(long index, T value)>[]
            {
                args => l.At((int) args.index),
                args =>
                {
                    try
                    {
                        var _ = l.First!.Value;
                    }
                    catch (NullReferenceException e)
                    {
                        throw new InvalidOperationException();
                    }
                },
                args =>
                {
                    try
                    {
                        var _ = l.Last!.Value;
                    }
                    catch (NullReferenceException e)
                    {
                        throw new InvalidOperationException();
                    }
                },
                args =>
                {
                    try
                    {
                        var _ = l.First!.Value;
                    }
                    catch (NullReferenceException e)
                    {
                    }
                },
                args =>
                {
                    try
                    {
                        var _ = l.Last!.Value;
                    }
                    catch (NullReferenceException e)
                    {
                    }
                },
                args => l.Insert((int) args.index, args.value),
                args => l.RemoveAt((int) args.index),
                args => l.AddLast(args.value),
                args => l.AddFirst(args.value),
                args => l.RemoveLast(),
                args => l.RemoveFirst(),
                args =>
                {
                    try
                    {
                        l.RemoveLast();
                    }
                    catch (InvalidOperationException)
                    {
                    }
                },
                args =>
                {
                    try
                    {
                        l.RemoveFirst();
                    }
                    catch (InvalidOperationException)
                    {
                    }
                },
                args => { },
                args => l.IndexOf(args.value),
                // balance
                args => l.AddLast(args.value),
                args => l.AddFirst(args.value)
            };
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(16)]
        [InlineData(null)]
        public void FullyFuzz(long? chunkSize)
        {
            var rnd = new RandomThreadSafe();
            Deque<sbyte> q;
            if (chunkSize.HasValue)
            {
                q = new Deque<sbyte>(chunkSize.Value);
            }
            else
            {
                q = new Deque<sbyte>();
            }

            var history = new List<(long index, sbyte value, int action)>();
            var adversarial = new LinkedList<sbyte>();
            var actions = BindActionSet(q);
            var adversarialActions = BindActionSet(adversarial);

            for (var i = 0; i < 4096; i++)
            {
                (long index, sbyte value) args = default;
                var index = rnd.Next(q.Length);
                var intRnd = rnd.Next();
                if ((intRnd & 1) == 1)
                {
                    var shift = (intRnd & 0b1110) >> 1;
                    if ((intRnd & 0b10000) == 1)
                    {
                        shift *= -1;
                    }

                    Debug.Assert(shift < 0x7F);
                    Debug.Assert(shift > -0x7F);
                    index += shift;
                }

                args.index = index;
                args.value = (sbyte) ((intRnd >> 16) & 0xFF);

                var actionIndex = rnd.Next(actions.Length);
                history.Add((args.index, args.value, actionIndex));
                var adversarialAction = adversarialActions[actionIndex];
                Exception? exception = null;
                try
                {
                    adversarialAction(args);
                }
                catch (Exception e)
                {
                    exception = e;
                }

                var action = actions[actionIndex];

                if (exception is null)
                {
                    try
                    {
                        action(args);
                    }
                    catch (Exception)
                    {
                        _testOutputHelper.WriteLine(history.ToString());
                        throw;
                    }
                }
                else
                {
                    try
                    {
                        Assert.Throws(exception.GetType(), () => action(args));
                    }
                    catch (XunitException)
                    {
                        _testOutputHelper.WriteLine(history.ToString());
                        throw;
                    }
                }


                try
                {
                    Assert.Equal(adversarial, q);
                }
                catch (Exception)
                {
                    _testOutputHelper.WriteLine(history.ToString());
                    throw;
                }
            }
        }

        internal class DataGenDifferentSizesAndChunkSizeWithFuzzingIndex : DequeTests.DataGenDifferentSizesAndChunkSize
        {
            protected RandomThreadSafe _random = new RandomThreadSafe();
            public int NumberIndexToGen { get; set; } = 4;

            public override IEnumerator<object[]> GetEnumerator()
            {
                var dejaVu = new HashSet<(long, long)>();
                checked
                {
                    foreach (var size in DequeSize)
                    {
                        for (var i = -1; i <= 1; i++)
                        {
                            var rsize = Math.Max(size + i, 0);
                            foreach (var chunk in ChunkSizes)
                            {
                                for (var j = -1; j <= 1; j++)
                                {
                                    var rchunk = Math.Max(chunk + j, 1);
                                    if (dejaVu.Add((rsize, rchunk)))
                                    {
                                        var indexSeen = new HashSet<long>();
                                        if (NumberIndexToGen >= size) // => no need to fuzz
                                        {
                                            for (var k = 0; k < size; k++)
                                            {
                                                yield return new object[] {rsize, rchunk, k};
                                            }
                                        }
                                        else
                                        {
                                            for (var k = 0; k < NumberIndexToGen; k++)
                                            {
                                                long rindex;
                                                do
                                                {
                                                    rindex = _random.Next((int) rsize);
                                                } while (!indexSeen.Add(rindex));

                                                yield return new object[] {rsize, rchunk, rindex};
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}