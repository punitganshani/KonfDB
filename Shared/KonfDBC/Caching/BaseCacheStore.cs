#region License and Product Information

// 
//     This file 'BaseCacheStore.cs' is part of KonfDB application - 
//     a project perceived and developed by Punit Ganshani.
// 
//     KonfDB is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     KonfDB is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with KonfDB.  If not, see <http://www.gnu.org/licenses/>.
// 
//     You can also view the documentation and progress of this project 'KonfDB'
//     on the project website, <http://www.konfdb.com> or on 
//     <http://www.ganshani.com/applications/konfdb>

#endregion

using System;
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