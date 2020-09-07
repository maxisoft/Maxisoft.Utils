using System;

namespace Maxisoft.Utils.Empty
{
    public readonly struct EmptyDisposable : IEmpty, IDisposable
    {
        public void Dispose()
        {
        }
    }
}