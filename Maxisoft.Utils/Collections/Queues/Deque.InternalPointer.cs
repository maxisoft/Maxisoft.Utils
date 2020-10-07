using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Maxisoft.Utils.Collections.Queues
{
    public partial class Deque<T>
    {
        [DebuggerDisplay(nameof(Index) + " = {" + nameof(Index) + "}, " + nameof(Node) + " = {" + nameof(Node) + "}")]
        protected internal readonly struct InternalPointer
        {
            internal readonly long Index;
            private readonly LinkedListNode<T[]>? _node;
            internal readonly long ChunkSize;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public InternalPointer(LinkedListNode<T[]>? node, long index, long chunkSize)
            {
                _node = node;
                Index = index;
                ChunkSize = chunkSize;
            }


            // ReSharper disable once ConvertToAutoProperty
            // ReSharper disable once ConvertToAutoPropertyWhenPossible
            internal LinkedListNode<T[]> Node => _node!;

            public ref T Value => ref Node.Value[Index];

            public long DistanceToBeginning => Index;
            public long DistanceToEnd => ChunkSize - Index;

            public bool HasNode => !ReferenceEquals(_node, null);
            public bool Valid => HasNode && Index >= 0 && Index < ChunkSize;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void ThrowForNullNode()
            {
                if (!HasNode)
                {
                    throw new InvalidOperationException("This pointer has no node");
                }
            }

            public static InternalPointer operator +(InternalPointer p, long inc)
            {
                p.ThrowForNullNode();
                if (inc < 0)
                {
                    return p - inc * -1;
                }

                var originalInc = inc;
                var res = p;
                var @continue = inc >= 0;
                while (@continue)
                {
                    @continue = inc != 0;
                    if (res.DistanceToEnd == 0)
                    {
                        if (res.Node.Next is null)
                        {
                            if (inc == 0) // stop here as res pointing at the very end of the allocated space
                            {
                                break;
                            }

                            throw new IndexOutOfRangeException($"Cannot move pointer to +{originalInc}");
                        }

                        res = new InternalPointer(res.Node.Next, 0, p.ChunkSize);
                    }
                    else if (res.DistanceToEnd >= inc)
                    {
                        res = new InternalPointer(res.Node, res.Index + inc, p.ChunkSize);
                        inc = 0;
                    }
                    else
                    {
                        var distanceToEnd = res.DistanceToEnd;
                        res = new InternalPointer(res.Node, res.Index + distanceToEnd, p.ChunkSize);
                        inc -= distanceToEnd;
                    }
                }

                return res;
            }

            public static InternalPointer operator -(InternalPointer p, long decr)
            {
                p.ThrowForNullNode();
                if (decr < 0)
                {
                    return p + decr * -1;
                }

                var originalDecr = decr;
                var res = p;
                while (decr > 0)
                {
                    if (res.DistanceToBeginning == 0)
                    {
                        if (res.Node.Previous is null)
                        {
                            if (decr == 0) // stop here as res pointing at the very beginning of the allocated space
                            {
                                break;
                            }

                            throw new IndexOutOfRangeException($"Cannot move pointer to -{originalDecr}");
                        }

                        res = new InternalPointer(res.Node.Previous, p.ChunkSize, p.ChunkSize);
                    }
                    else if (res.DistanceToBeginning >= decr)
                    {
                        res = new InternalPointer(res.Node, res.Index - decr, p.ChunkSize);
                        decr = 0;
                    }
                    else
                    {
                        var distanceToBeginning = res.DistanceToBeginning;
                        res = new InternalPointer(res.Node, res.Index - distanceToBeginning, p.ChunkSize);
                        decr -= distanceToBeginning;
                    }
                }

                return res;
            }
        }
    }
}