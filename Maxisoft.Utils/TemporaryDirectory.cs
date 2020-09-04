using System;
using System.IO;

namespace Maxisoft.Utils
{
    public class TemporaryDirectory : IDisposable
    {
        public readonly string Path;
        private bool _disposed;

        public TemporaryDirectory(string prefix = "", string suffix = "")
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            var tempDirectory = System.IO.Path.Combine(GetTempPath(),
                prefix + System.IO.Path.GetRandomFileName() + suffix);
            Directory.CreateDirectory(tempDirectory);
            Path = System.IO.Path.GetFullPath(tempDirectory);
        }

        protected virtual string GetTempPath() => System.IO.Path.GetTempPath();

        public void Dispose()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().Name);
            try
            {
                Directory.Delete(Path, true);
            }
            finally
            {
                _disposed = true;
            }
        }

        public static explicit operator string(TemporaryDirectory temporaryDirectory)
        {
            return temporaryDirectory.Path;
        }
    }
}