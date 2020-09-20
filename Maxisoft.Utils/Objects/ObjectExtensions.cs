using System;

namespace Maxisoft.Utils.Objects
{
    public static class ObjectExtensions
    {
        public static Boxed<T> Box<T>(in T value) => new Boxed<T>(in value);

        public static T[] WrapIntoArray<T>(in T value) => new[] {value};

        public static WeakReference<T> WeakReference<T>(in T value) where T : class => new WeakReference<T>(value);
        
        public static IntPtr GetPointer<T>(in T value) where T : class => PointerHelper.GetPointer(in value); 
    }
}