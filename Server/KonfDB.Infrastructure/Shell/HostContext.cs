#region License and Product Information

// 
//     This file 'AppContext.cs' is part of KonfDB application - 
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

using System.Collections.Generic;
using KonfDB.Infrastructure.Caching;
using KonfDB.Infrastructure.Common;
using KonfDB.Infrastructure.Configuration;
using KonfDB.Infrastructure.Database.Providers;
using KonfDB.Infrastructure.Logging;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Utilities;

namespace KonfDB.Infrastructure.Shell
{
    /// <summary>
    ///     Application Context for KonfDBHost
    /// </summary>
    internal class HostContext
    {
        private static HostContext _current;

        internal static HostContext Current
        {
            get { return _current ?? (_current = new HostContext(AppConfig.ThisSection)); }
        }

        internal AppType ApplicationType;

        internal Logger Log
        {
            get { return CurrentContext.Default.Log; }
        }

        internal InMemoryCacheStore Cache
        {
            get { return CurrentContext.Default.Cache; }
        }


        internal IArguments ApplicationParams
        {
            get { return CurrentContext.Default.ApplicationParams; }
        }

        private HostContext(AppConfig configuration)
        {
            Config = configuration;
            UserTokens = new List<string>();

            ApplicationType = AppType.Server;

            var logger = Logger.CreateInstance(true, configuration.Runtime.LogConfigPath);
            var commandArgs = new CommandArgs(configuration.Runtime.Parameters);
            var cache = new InMemoryCacheStore(Config.Caching)
            {
                OnItemRemove = x =>
                {
                    Log.Debug("Item removed from cache: " + x.CacheItem.Key + " Reason : " + x.RemovedReason);
                    if (x.CacheItem.Value is AuthenticationOutput)
                    {
                        // User has been removed from cache. Login expired
                        UserTokens.Remove(x.CacheItem.Key);
                    }
                }
            };

            CurrentContext.CreateDefault(logger, commandArgs, cache);
        }

        internal bool AuditEnabled
        {
            get { return Config.Runtime.Audit; }
        }

        internal AppConfig Config { get; private set; }

        internal BaseProvider Provider { get; set; }


        internal List<string> UserTokens { get; set; }

        public List<AuthenticationOutput> GetUsers()
        {
            var users = new List<AuthenticationOutput>();
            UserTokens.ForEach(x => users.Add(Cache.Get<AuthenticationOutput>(x)));
            return users;
        }
    }
}