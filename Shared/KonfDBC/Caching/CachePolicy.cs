using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KonfDB.Infrastructure.Caching
{
    public enum CachePolicy
    {
        ExpireAsPerConfig,
        ExpireByMidnight,
        AlwaysLive
    }
}
