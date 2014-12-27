using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using KonfDB.Infrastructure.Common;
using KonfDB.Infrastructure.Configuration.Interfaces;

namespace KonfDB.Infrastructure.Caching
{
    public abstract class BaseCacheStore
    {
        private readonly ICacheConfiguration _configuration;
        private readonly bool _enabled;
         
        public event EventHandler<CacheItemRemovedArgs> ItemRemoved;
        protected virtual void OnItemRemoved(CacheItemRemovedArgs e)
        {
            EventHandler<CacheItemRemovedArgs> handler = ItemRemoved;
            if (handler != null) handler(this, e);
        }

        public bool Enabled
        {
            get { return _enabled; }
        }

        public ICacheConfiguration Configuration
        {
            get { return _configuration; }
        }

        protected BaseCacheStore(ICacheConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            _configuration = configuration;
            _enabled = configuration.Enabled; 
        } 
        
        public abstract T Get<T>(string key);
        public abstract T Get<T>(string key, Func<T> func, CachePolicy mode);
        public abstract T Get<T>(string key, Func<T> func);
        public abstract void Remove<T>(string key);
    }
}
