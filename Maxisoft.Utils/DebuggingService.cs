namespace Maxisoft.Utils
{
    public static class DebuggingService
    {
        public const bool IsInDebug =
#if DEBUG
                true
#else
                false
#endif
            ;
    }
}