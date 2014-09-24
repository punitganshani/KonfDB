#region License and Product Information

// 
//     This file 'InMemoryCacheStore.cs' is part of KonfDB application - 
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
using System.Runtime.Caching;
using KonfDB.Infrastructure.Configuration.Caching;
using KonfDB.Infrastructure.Shell;

namespace KonfDB.Infrastructure.Caching
{
    public class InMemoryCacheStore
    {
        internal enum Mode
        {
            ExpireAsPerConfig,
            ExpireByMidnight,
            AlwaysLive
        }

        private static readonly ObjectCache Cache;
        private readonly CacheConfigurationSection _configuration;
        public CacheEntryRemovedCallback OnItemRemove;
        private readonly bool _enabled;

        static InMemoryCacheStore()
        {
            Cache = MemoryCache.Default;
        }

        internal InMemoryCacheStore(CacheConfigurationSection cacheConfiguration)
        {
            if (cacheConfiguration == null)
                throw new ArgumentNullException("cacheConfiguration");

            _configuration = cacheConfiguration;
            _enabled = _configuration.Enabled;
        }

        private bool Add<T>(string key, string region, T value, CacheItemPolicy policy)
        {
            return Cache.Add(new CacheItem(CreateUniqueKey<T>(key, region), value), policy ?? CreatePolicy());
        }

        private CacheItemPolicy CreatePolicy()
        {
            var policy = new CacheItemPolicy
            {
                Priority = CacheItemPriority.Default,
                RemovedCallback = OnItemRemove
            };

            TimeSpan validUntil = _configuration.DurationInSeconds <= 0
                ? TimeSpan.FromMinutes(10)
                : TimeSpan.FromSeconds(_configuration.DurationInSeconds);

            if (_configuration.Mode == CacheMode.Sliding)
                policy.SlidingExpiration = validUntil;
            else if (_configuration.Mode == CacheMode.Absolute)
                policy.AbsoluteExpiration = DateTimeOffset.Now.Add(validUntil);

            return policy;
        }

        private static string CreateUniqueKey<T>(string key, string region)
        {
            return String.Format("[{0}|key={1}]", region, key);
        }

        internal T Get<T>(string key)
        {
            string cacheKey = CreateUniqueKey<T>(key, typeof (T).Name);
            if (Cache.Contains(cacheKey))
            {
                return (T) Cache[cacheKey];
            }

            return default(T);
        }

        internal T Get<T>(string key, Func<T> func, Mode mode)
        {
            // If cache is not enabled
            if (!_enabled && mode == Mode.ExpireAsPerConfig)
                return func();

            // Cache is enabled, so check in cache
            T newObject;
            string region = typeof (T).Name;
            string cacheKey = CreateUniqueKey<T>(key, region);

            if (Cache.Contains(cacheKey))
            {
                CurrentContext.Default.Log.Debug("Got from cache :" + cacheKey);
                newObject = (T) Cache[cacheKey];
            }
            else
            {
                newObject = func();

                CacheItemPolicy policy = null;
                if (mode == Mode.ExpireByMidnight)
                    policy = GetPolicyTillMidnight();
                else if (mode == Mode.AlwaysLive)
                    policy = GetPolicyAlwaysLive();

                CurrentContext.Default.Log.Debug("Added to cache :" + cacheKey + " > " +
                                                 Add(key, region, newObject, policy));
            }

            return newObject;
        }


        internal T Get<T>(string key, Func<T> func)
        {
            return Get(key, func, Mode.ExpireAsPerConfig);
        }

        private void Remove<T>(string key)
        {
            string cacheKey = CreateUniqueKey<T>(key, typeof (T).Name);

            if (Cache.Contains(cacheKey))
                Cache.Remove(cacheKey);
        }

        private CacheItemPolicy GetPolicyAlwaysLive()
        {
            return new CacheItemPolicy
            {
                Priority = CacheItemPriority.NotRemovable,
                RemovedCallback = OnItemRemove
            };
        }


        private CacheItemPolicy GetPolicyTillMidnight()
        {
            DateTime now = DateTime.UtcNow;
            DateTime midnight = new DateTime(now.Year, now.Month, now.Day, 11, 59, 59, DateTimeKind.Utc);
            TimeSpan validUntil = midnight.Subtract(now);

            return new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.Add(validUntil),
                RemovedCallback = OnItemRemove,
                Priority = CacheItemPriority.Default
            };
        }
    }
}