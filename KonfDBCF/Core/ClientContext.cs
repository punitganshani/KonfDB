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
using KonfDB.Infrastructure.Exceptions;
using KonfDB.Infrastructure.Logging;
using KonfDB.Infrastructure.Shell;
using KonfDB.Infrastructure.Utilities;
using KonfDBCF.Configuration;

namespace KonfDBCF.Core
{
    internal class ClientContext
    {
        private static ClientContext _current;

        internal static ClientContext Current
        {
            get { return _current ?? (_current = new ClientContext(ClientConfig.ThisSection)); }
        }

        private ClientContext(ClientConfig configuration)
        {
            if (configuration == null)
                throw new InvalidOperationException("Current Context could not be initialized. Configuration may be missing in App.Config file.");

            Config = configuration;

            var logger = Logger.CreateInstance(false, configuration.Runtime.LogConfigPath);

            if (configuration.Runtime == null)
                throw new InvalidConfigurationException("Could not find Runtime Configuration for KonfDBCF");
            
            var applicationParams = new CommandArgs(configuration.Runtime.Parameters);
            var cache = new InMemoryCacheStore(Config.Caching)
            {
                OnItemRemove =
                    x => Log.Debug("Item removed from cache: " + x.CacheItem.Key + " Reason : " + x.RemovedReason)
            };

            CurrentContext.CreateDefault(logger, applicationParams, cache);
        }

        public Logger Log
        {
            get { return CurrentContext.Default.Log; }
        }

        public InMemoryCacheStore Cache
        {
            get { return CurrentContext.Default.Cache; }
        }

        public IArguments ApplicationParams
        {
            get { return CurrentContext.Default.ApplicationParams; }
        }

        internal ClientConfig Config { get; private set; }
    }
}