#region License and Product Information

// 
//     This file 'ClientContext.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.Factory;
using KonfDB.Infrastructure.Logging;
using KonfDB.Infrastructure.Shell;
using KonfDB.Infrastructure.Utilities;
using KonfDBCF.Configuration;

namespace KonfDBCF.Core
{
    /// <summary>
    ///     Application Context for Client using Config
    /// </summary>
    internal class ClientContext
    {
        private static ClientContext _current;
        private static ClientConfig _config;

        internal static ClientContext Current
        {
            get
            {
                if (_current == null)
                {
                    throw new InvalidOperationException("ClientContext should be created by CreateNew method");
                }
                return _current;
            }
        }

        private ClientContext(ClientConfig configuration)
        {
            _config = configuration;

            if (configuration == null)
                throw new InvalidOperationException(
                    "Current Context could not be initialized. No configuration passed to the context");

            var logger = LogFactory.CreateInstance(configuration.Runtime.LogInfo);

            var cache = CacheFactory.Create(configuration.Caching);
            cache.ItemRemoved +=
                (sender, args) =>
                    Log.Debug("Item removed from cache: " + args.CacheKey + " Reason : " + args.RemoveReason);

            CurrentContext.CreateDefault(logger, new CommandArgs(string.Empty), cache);
        }

        public static void CreateNew(ClientConfig configuration)
        {
            if (_current == null)
            {
                _current = new ClientContext(configuration);
            }
        }

        public BaseLogger Log
        {
            get { return CurrentContext.Default.Log; }
        }

        public BaseCacheStore Cache
        {
            get { return CurrentContext.Default.Cache; }
        }

        public IArguments ApplicationParams
        {
            get { return CurrentContext.Default.ApplicationParams; }
        }

        public ClientConfig Config
        {
            get { return _config; }
        }
    }
}