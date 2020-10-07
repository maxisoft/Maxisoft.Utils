using System;
using System.Collections.Generic;
using System.Diagnostics;
using Maxisoft.Utils.Collections.LinkedLists;
using Maxisoft.Utils.Collections.Queues;
using Maxisoft.Utils.Collections.Queues.Specialized;
using Maxisoft.Utils.Random;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Maxisoft.Utils.Tests.Collections.Queues.Specialized
{
    public class PooledDequeFuzzingTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public PooledDequeFuzzingTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
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
                q = new PooledDeque<sbyte>(chunkSize.Value);
            }
            else
            {
                q = new PooledDeque<sbyte>();
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
    }
}