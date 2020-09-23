using System;

namespace Maxisoft.Utils.Empties
{
    public readonly struct EmptyDisposable : IEmpty, IDisposable
    {
        public void Dispose()
        {
        }
    }
}