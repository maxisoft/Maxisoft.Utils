using System;
using System.Collections.Generic;

namespace Maxisoft.Utils.Collection
{
    public partial class Deque<T>
    {
        internal readonly struct InternalPointer
        {
            internal readonly long Index;

            public InternalPointer(in LinkedListNode<T[]> node, long index)
            {
                Node = node;
                Index = index;
            }


            internal LinkedListNode<T[]> Node { get; }

            public ref T Value => ref Node.Value[Index];

            public long DistanceToBeginning => Index;
            public long DistanceToEnd => Node.Value.LongLength - Index;

            public bool HasNode => !ReferenceEquals(Node, null);
            public bool Valid => HasNode && Index >= 0 && Index < Node.Value.LongLength;

            public static InternalPointer operator +(InternalPointer p, long inc)
            {
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

                        res = new InternalPointer(res.Node.Next, 0);
                    }
                    else if (res.DistanceToEnd >= inc)
                    {
                        res = new InternalPointer(res.Node, res.Index + inc);
                        inc = 0;
                    }
                    else
                    {
                        var distanceToEnd = res.DistanceToEnd;
                        res = new InternalPointer(res.Node, res.Index + distanceToEnd);
                        inc -= distanceToEnd;
                    }
                }

                return res;
            }

            public static InternalPointer operator -(InternalPointer p, long decr)
            {
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

                        res = new InternalPointer(res.Node.Previous, res.Node.Previous.Value.Length);
                    }
                    else if (res.DistanceToBeginning >= decr)
                    {
                        res = new InternalPointer(res.Node, res.Index - decr);
                        decr = 0;
                    }
                    else
                    {
                        var distanceToBeginning = res.DistanceToBeginning;
                        res = new InternalPointer(res.Node, res.Index - distanceToBeginning);
                        decr -= distanceToBeginning;
                    }
                }

                return res;
            }
        }
    }
}