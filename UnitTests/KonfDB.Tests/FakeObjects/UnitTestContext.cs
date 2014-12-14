#region License and Product Information

// 
//     This file 'UnitTestContext.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.Logging;
using KonfDB.Infrastructure.Shell;
using KonfDB.Infrastructure.Utilities;

namespace KonfDB.Tests.FakeObjects
{
    internal class UnitTestContext
    {
        private static UnitTestContext _current;

        private UnitTestContext(IArguments arguments)
        {
            if (arguments == null)
                throw new InvalidOperationException(
                    "Current Context could not be initialized. No arguments passed to the context");

            var logger = Logger.CreateInstance(Environment.UserInteractive,
                arguments.ContainsKey(@"runtime-logConfigPath") ? arguments["runtime-logConfigPath"] : string.Empty);

            CurrentContext.CreateDefault(logger, arguments, null);
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
                _current = new UnitTestContext(arguments);
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
    }
}