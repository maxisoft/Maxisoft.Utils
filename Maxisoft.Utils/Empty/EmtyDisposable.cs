using System;

namespace Maxisoft.Utils.Empty
{
    public struct EmptyDisposable : IEmpty, IDisposable
    {
        public void Dispose()
        {
        }
    }
}