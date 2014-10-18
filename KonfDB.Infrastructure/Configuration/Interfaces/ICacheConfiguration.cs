using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KonfDB.Infrastructure.Configuration.Caching;

namespace KonfDB.Infrastructure.Configuration.Interfaces
{
    public interface ICacheConfiguration
    {
        bool Enabled { get; set; }
        CacheMode Mode { get; set; }
        long DurationInSeconds { get; set; }
    }
}
