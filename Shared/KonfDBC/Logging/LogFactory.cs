using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace KonfDB.Infrastructure.Logging
{
    public class LogFactory
    {
        private static ILogger _instance;

        public static ILogger CreateInstance(bool isConsole, string configurationFilePath)
        {
            if (_instance == null)
                _instance = new Logger(isConsole, configurationFilePath);

            return _instance;
        }


        public static ILog CreateLog()
        {
            return LogManager.GetLogger("KonfDB");
        }

    }
}
