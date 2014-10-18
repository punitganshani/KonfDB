using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KonfDB.Infrastructure.Configuration.Caching;
using KonfDB.Infrastructure.Configuration.Interfaces;

namespace KonfDBCF.Configuration.Caching
{
    public class CacheConfiguration : ICacheConfiguration
    {
        public bool Enabled { get; set; }
        public CacheMode Mode { get; set; }
        public long DurationInSeconds { get; set; }
    }
}
