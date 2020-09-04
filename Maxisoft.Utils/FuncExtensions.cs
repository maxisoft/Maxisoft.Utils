using System;
using System.Threading.Tasks;

namespace Maxisoft.Utils
{
    public static class FuncExtensions
    {
        public static Task<T> ToTask<T>(this Func<T> func)
        {
            return Task.Run(func);
        }
    }
}