using System;
using System.IO;

namespace Maxisoft.Utils
{
    public static class SystemPathUtils
    {
        public static string PathAddBackslash(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return Path.DirectorySeparatorChar.ToString();
            }

            path = path.TrimEnd();
            var lastChar = path[path.Length - 1];

            if (lastChar == Path.DirectorySeparatorChar || lastChar == Path.AltDirectorySeparatorChar)
            {
                return path;
            }

            var index = path.LastIndexOfAny(new[] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar});
            if (index < 0)
            {
                return path + Path.DirectorySeparatorChar;
            }

            var sep = path[index];
            return path + sep;
        }

        [Obsolete("use Maxisoft.Utils.TemporaryDirectory")]
        public static string CreateTemporaryDirectory()
        {
            return new TemporaryDirectory().Path;
        }

        public static string PathSlashToUnix(string path)
        {
            return path.Replace("\\", "/");
        }
    }
}