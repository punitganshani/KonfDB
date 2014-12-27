using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KonfDB.Infrastructure.Caching
{
    public class CacheItemRemovedArgs : EventArgs
    {
        public object Value { get; set; }
        public string RemoveReason { get; set; }
        public string CacheKey { get; set; }
    }
}
