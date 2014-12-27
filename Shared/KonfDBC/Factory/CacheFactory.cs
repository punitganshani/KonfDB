#region License and Product Information

// 
//     This file 'CacheFactory.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.Exceptions;
using KonfDB.Infrastructure.Extensions;

namespace KonfDB.Infrastructure.Factory
{
    public static class CacheFactory
    {
        private static BaseCacheStore _instance;


        public static BaseCacheStore Create(ICacheConfiguration cacheConfiguration)
        {
            if (cacheConfiguration == null)
                throw new ArgumentNullException("cacheConfiguration");

            if (_instance == null)
            {
                Type providerType = Type.GetType(cacheConfiguration.ProviderType);
                if (providerType == null)
                    throw new InvalidConfigurationException("Could not locate Cache Provider :" +
                                                            cacheConfiguration.ProviderType);

                if (!providerType.ImplementsClass<BaseCacheStore>())
                    throw new InvalidConfigurationException("Cache Provider does not implement BaseCacheStore:" +
                                                            cacheConfiguration.ProviderType);

                _instance = (BaseCacheStore) Activator.CreateInstance(providerType, cacheConfiguration);
            }

            return _instance;
        }
    }
}