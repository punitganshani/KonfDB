using System;
using KonfDB.Infrastructure.Caching;
using KonfDB.Infrastructure.Configuration.Interfaces;
using KonfDB.Infrastructure.Exceptions;
using KonfDB.Infrastructure.Extensions;
using KonfDB.Infrastructure.Utilities;

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
                    throw new InvalidConfigurationException("Could not locate Cache Provider :" + cacheConfiguration.ProviderType);

                if (!providerType.ImplementsClass<BaseCacheStore>())
                    throw new InvalidConfigurationException("Cache Provider does not implement BaseCacheStore:" +
                                                            cacheConfiguration.ProviderType);
                
                _instance = (BaseCacheStore)Activator.CreateInstance(providerType, cacheConfiguration);
            }

            return _instance;
        }
    }
}
