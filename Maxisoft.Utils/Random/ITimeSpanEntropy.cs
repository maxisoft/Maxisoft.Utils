using System;

namespace Maxisoft.Utils.Random
{
    public interface ITimeSpanEntropy
    {
        TimeSpan Max { get; }
        TimeSpan Min { get; }
        
        TimeSpan Value { get; }

        TimeSpan Next();
    }
}