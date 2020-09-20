using System;
using System.Runtime.InteropServices;

namespace Maxisoft.Utils.Objects
{
    public static class PointerHelper
    {
        public static IntPtr GetPointer<T>(in T? obj) where T : class
        {
            return new ObjectUnion {Object = obj};
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct ObjectUnion
        {
            [FieldOffset(0)] internal Boxed<object?> Object;
            [FieldOffset(0)] private readonly Boxed<IntPtr> IntPtr;

            public static implicit operator IntPtr(ObjectUnion union)
            {
                return union.IntPtr;
            }
        }
    }
}