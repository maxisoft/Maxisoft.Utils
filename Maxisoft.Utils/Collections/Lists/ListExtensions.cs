using System.Collections.Generic;

namespace Maxisoft.Utils.Collections.Lists
{
    public static class ListExtensions
    {
        public static ref T At<T>(this T[] array, WrappedIndex index)
        {
            return ref array[index.Resolve(array)];
        }
        
        public static T At<T, TList>(TList list, WrappedIndex index) where TList : IList<T>
        {
            return list[index.Resolve<T, TList>(list)];
        }
        
        public static T At<T>(this IList<T> list, WrappedIndex index)
        {
            return At<T, IList<T>>(list, index);
        }
    }
}