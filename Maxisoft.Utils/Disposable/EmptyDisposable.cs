using System;

namespace Maxisoft.Utils.Disposable
{
    public sealed class EmptyDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
}