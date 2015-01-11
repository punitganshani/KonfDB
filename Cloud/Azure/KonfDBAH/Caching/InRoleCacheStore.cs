#region License and Product Information

// 
//     This file 'InRoleCacheStore.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.Caching;
using KonfDB.Infrastructure.Configuration.Interfaces;
using KonfDB.Infrastructure.Extensions;
using KonfDB.Infrastructure.Shell;
using KonfDB.Infrastructure.Utilities;
using Microsoft.ApplicationServer.Caching;

namespace KonfDBAH.Caching
{
    public sealed class InRoleCacheStore : BaseCacheStore
    {
        private static DataCache _cache;
        private static DataCacheFactory _cacheFactory;
        private readonly CachePolicy _policy;
        private readonly int _durationInMinutes;

        public InRoleCacheStore(ICacheConfiguration configuration)
            : base(configuration)
        {
            var args = new CommandArgs(configuration.Parameters);

            if (_cacheFactory == null)
                _cacheFactory = new DataCacheFactory();

            _cache = args.ContainsKey("cacheName")
                ? _cacheFactory.GetCache(args.GetValue("cacheName", "default"))
                : _cacheFactory.GetDefaultCache();

            _policy = args.GetValue("policy", CachePolicy.AlwaysLive.ToString()).ToEnum<CachePolicy>();
            _durationInMinutes = int.Parse(args.GetValue("duration", "10"));
        }

        public override T Get<T>(string key)
        {
            string cacheKey = CreateUniqueKey<T>(key, typeof (T).Name);
            if (_cache.Get(cacheKey) != null)
            {
                return (T) _cache[cacheKey];
            }

            return default(T);
        }

        public override T Get<T>(string key, Func<T> func, CachePolicy mode)
        {
            // If cache is not enabled
            if (!Enabled && mode == CachePolicy.ExpireAsPerConfig)
                return func();

            // Cache is enabled, so check in cache
            T newObject;
            string region = typeof (T).Name;
            string cacheKey = CreateUniqueKey<T>(key, region);
            if (_cache.Get(cacheKey) != null)
            {
                CurrentContext.Default.Log.Debug("Got from cache :" + cacheKey);
                newObject = (T) _cache[cacheKey];
            }
            else
            {
                newObject = func();
                CurrentContext.Default.Log.Debug("Added to cache :" + cacheKey + " > " +
                                                 Add(key, region, newObject, mode));
            }

            return newObject;
        }

        public override T Get<T>(string key, Func<T> func)
        {
            return Get(key, func, _policy);
        }

        public override void Remove<T>(string key)
        {
            string cacheKey = CreateUniqueKey<T>(key, typeof (T).Name);

            if (_cache.Get(cacheKey) != null)
            {
                _cache.Remove(cacheKey);
            }
        }

        #region Private Methods

        private bool Add<T>(string key, string region, T value, CachePolicy policy)
        {
            _cache.CreateRegion(region);

            if (policy == CachePolicy.ExpireAsPerConfig)
            {
                return _cache.Add(key, value, region) != null;
            }

            return _cache.Add(key, value, GetExpiryTime(policy), region) != null;
        }

        private TimeSpan GetExpiryTime(CachePolicy policy)
        {
            if (policy == CachePolicy.AlwaysLive)
                return TimeSpan.MaxValue;

            if (policy == CachePolicy.ExpireByMidnight)
            {
                DateTime now = DateTime.UtcNow;
                var midnight = new DateTime(now.Year, now.Month, now.Day, 11, 59, 59, DateTimeKind.Utc);
                return midnight.Subtract(now);
            }

            //default case
            return TimeSpan.FromMinutes(_durationInMinutes);
        }

        #endregion
    }
}