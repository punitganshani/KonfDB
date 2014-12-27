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
using KonfDB.Infrastructure.Configuration.Interfaces;
using KonfDB.Infrastructure.Configuration.Runtime;
using KonfDB.Infrastructure.Extensions;
using KonfDB.Infrastructure.Factory;
using KonfDB.Infrastructure.Logging;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;
using KonfDB.Infrastructure.Utilities;
using KonfDBCF.Configuration.Caching;

namespace KonfDBCF.Core
{
    /// <summary>
    ///     Application Context for Client using Config
    /// </summary>
    internal class ClientContext
    {
        private static ClientContext _current;

        private ClientContext(IArguments arguments)
        {
            if (arguments == null)
                throw new InvalidOperationException(
                    "Current Context could not be initialized. No arguments passed to the context");

            var element = new LogElement
            {
                Parameters = "-ShowOnConsole:true ",
                ProviderType = "KonfDB.Infrastructure.Logging.Logger, KonfDBC"
            };

            if (arguments.ContainsKey("runtime-logConfigPath"))
                element.Parameters += "-path:" + arguments["runtime-logConfigPath"];

            var logger = LogFactory.CreateInstance(element);

            var cacheConfig = new CacheConfiguration
            {
                Enabled = bool.Parse(arguments.GetValue(@"cache-enabled", "false")),
                ProviderType = typeof(InMemoryCacheStore).AssemblyQualifiedName,
                Parameters = "-duration:30 -mode:Absolute"
            };

            var cache = CacheFactory.Create(cacheConfig);
            cache.ItemRemoved += (sender, args) => Log.Debug("Item removed from cache: " + args.CacheKey + " Reason : " + args.RemoveReason);

            CurrentContext.CreateDefault(logger, arguments, cache);
        }

        public static void CreateNew(IArguments arguments)
        {
            // Validate the mandatory input params
            if (!arguments.ContainsKey("type"))
                throw new ArgumentException("-type not provided");

            if (!arguments.ContainsKey("port"))
                throw new ArgumentException("-port not provided");

            if (!arguments.ContainsKey("host"))
                throw new ArgumentException("-host not provided");

            if (!arguments.ContainsKey("username"))
                throw new ArgumentException("-username not provided");

            if (!arguments.ContainsKey("password"))
                throw new ArgumentException("-password not provided");

            if (_current == null)
                _current = new ClientContext(arguments);
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
    }
}