using System;

namespace Maxisoft.Utils.Time
{
    public interface IMonotonicTimestamp : IComparable<IMonotonicTimestamp>, IEquatable<IMonotonicTimestamp>
    {
        long ToMilliseconds();
    }
}