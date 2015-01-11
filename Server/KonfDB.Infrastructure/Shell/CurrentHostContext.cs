#region License and Product Information

// 
//     This file 'CurrentHostContext.cs' is part of KonfDB application - 
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
using System.Collections.Generic;
using KonfDB.Infrastructure.Caching;
using KonfDB.Infrastructure.Commands;
using KonfDB.Infrastructure.Common;
using KonfDB.Infrastructure.Configuration.Interfaces;
using KonfDB.Infrastructure.Configuration.Runtime;
using KonfDB.Infrastructure.Database.Providers;
using KonfDB.Infrastructure.Exceptions;
using KonfDB.Infrastructure.Extensions;
using KonfDB.Infrastructure.Factory;
using KonfDB.Infrastructure.Logging;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Utilities;

namespace KonfDB.Infrastructure.Shell
{
    internal sealed class CurrentHostContext : IHostContext
    {
        private static IHostContext _defaultContext;

        public static IHostContext Default
        {
            get
            {
                if (_defaultContext == null)
                {
                    throw new InvalidOperationException("CurrentHostContext has not been initialized");
                }
                return _defaultContext;
            }
        }

        public AuditElement Audit { get; private set; }
        public IHostConfig Config { get; private set; }
        public BaseProvider Provider { get; private set; }

        public ICommandFactory CommandFactory { get; private set; }

        public AppType ApplicationType
        {
            get { return AppType.Server; }
        }

        public List<string> UserTokens { get; private set; }

        public IArguments ApplicationParams
        {
            get { return CurrentContext.Default.ApplicationParams; }
            set { CurrentContext.Default.ApplicationParams = value; }
        }

        public BaseLogger Log
        {
            get { return CurrentContext.Default.Log; }
            set { CurrentContext.Default.Log = value; }
        }

        public BaseCacheStore Cache
        {
            get { return CurrentContext.Default.Cache; }
            set { CurrentContext.Default.Cache = value; }
        }

        private CurrentHostContext(IHostConfig configuration, ContextSettings settings)
        {
            Audit = configuration.Runtime.Audit;
            Config = configuration;
            UserTokens = new List<string>();

            var logger = LogFactory.CreateInstance(configuration.Runtime.LogInfo);
            var commandArgs = new CommandArgs(configuration.Runtime.Parameters);
            var cache = CacheFactory.Create(configuration.Caching);
            cache.ItemRemoved += (sender, args) =>
            {
                Log.Debug("Item removed from cache: " + args.CacheKey + " Reason : " + args.RemoveReason);
                if (args.Value is AuthenticationOutput)
                {
                    // User has been removed from cache. Login expired
                    UserTokens.Remove(args.CacheKey);
                }
            };

            CommandFactory = settings.CommandFactory;

            CurrentContext.CreateDefault(logger, commandArgs, cache);
            Provider = GetDatabaseProviderInstance(configuration);
        }

        public static IContext CreateDefault(IHostConfig configuration, ContextSettings settings)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            if (settings == null)
                throw new ArgumentNullException("settings");

            return _defaultContext ?? (_defaultContext = new CurrentHostContext(configuration, settings));
        }

        public List<AuthenticationOutput> GetUsers()
        {
            var users = new List<AuthenticationOutput>();
            UserTokens.ForEach(x => users.Add(Cache.Get<AuthenticationOutput>(x)));
            return users;
        }

        private BaseProvider GetDatabaseProviderInstance(IHostConfig configuration)
        {
            var defaultDatabaseConfig = configuration.Database.Default;
            Type providerType = Type.GetType(defaultDatabaseConfig.ProviderType);

            if (providerType == null)
                throw new InvalidConfigurationException("Could not locate Database Provider :" +
                                                        defaultDatabaseConfig.ProviderType);

            if (!providerType.ImplementsClass<BaseProvider>())
                throw new InvalidConfigurationException("Database Provider does not implement BaseProvider:" +
                                                        defaultDatabaseConfig.ProviderType);

            var instance = Activator.CreateInstance(providerType, defaultDatabaseConfig);

            var baseProvider = instance as BaseProvider;
            if (baseProvider != null)
            {
                baseProvider.Init();
                return baseProvider;
            }
            throw new InvalidOperationException("Type : " + providerType + " does not inherit from BaseProvider");
        }

        //public IEnumerable<ICommand> GetAllCommands()
        //{
        //    var catalog = new AggregateCatalog();
        //    catalog.Catalogs.Add(new DirectoryCatalog(@".", "*.dll"));
        //    catalog.Catalogs.Add(new DirectoryCatalog(@".", "*.exe"));
        //    var container = new CompositionContainer(catalog);
        //    return container.GetExportedValues<ICommand>().Where(x => (x.Type & ApplicationType) == ApplicationType);
        //}
    }
}