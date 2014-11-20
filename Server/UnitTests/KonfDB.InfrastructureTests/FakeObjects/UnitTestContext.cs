using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KonfDB.Infrastructure.Caching;
using KonfDB.Infrastructure.Configuration.Caching;
using KonfDB.Infrastructure.Extensions;
using KonfDB.Infrastructure.Logging;
using KonfDB.Infrastructure.Shell;
using KonfDB.Infrastructure.Utilities;

namespace KonfDB.InfrastructureTests.FakeObjects
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
