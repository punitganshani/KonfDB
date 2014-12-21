#region License and Product Information

// 
//     This file 'Logger.cs' is part of KonfDB application - 
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
using System.IO;
using KonfDB.Infrastructure.Utilities;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;

namespace KonfDB.Infrastructure.Logging
{
    public sealed class Logger : BaseLogger
    {
        private readonly ILog _log;

        public Logger(IArguments args)
            : base(args)
        {
            if (args.ContainsKey("logConfigPath") && File.Exists(args["logConfigPath"]))
            {
                XmlConfigurator.Configure(new FileInfo(args["logConfigPath"]));
                _log = LogManager.GetLogger("KonfDB");
            }
            else
            {
                _log = LogManager.GetLogger("KonfDB");
                var appenders = new List<IAppender> {CreateFileAppender("FileAppender", @"Logs\KonfDB.log")};

                if (args.GetValue("ShowOnConsole", "false")
                    .Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase))
                    appenders.Add(CreateConsoleAppender());

                BasicConfigurator.Configure(appenders.ToArray());
            }
        }

        private static AppenderSkeleton CreateConsoleAppender()
        {
            var consoleAppender = new ConsoleAppender
            {
                Threshold = Level.All,
                Layout = new PatternLayout {ConversionPattern = "[%t] %-5p - %m%n"}
            };

            //consoleAppender.AddMapping(new ColoredConsoleAppender.LevelColors { Level = Level.Error, 
            //    ForeColor = ColoredConsoleAppender.Colors.Red | ColoredConsoleAppender.Colors.HighIntensity });
            //consoleAppender.AddMapping(new ColoredConsoleAppender.LevelColors { Level = Level.Fatal, 
            //    ForeColor = ColoredConsoleAppender.Colors.White | ColoredConsoleAppender.Colors.HighIntensity, 
            //    BackColor = ColoredConsoleAppender.Colors.Red });

            consoleAppender.ActivateOptions();

            return consoleAppender;
        }

        private static IAppender CreateFileAppender(string name, string fileName)
        {
            var appender = new FileAppender {Name = name, File = fileName, AppendToFile = true};
            var layout = new PatternLayout {ConversionPattern = "%d [%t] %-5p %c [%x] - %m%n"};
            layout.ActivateOptions();

            appender.Layout = layout;
            appender.ActivateOptions();

            return appender;
        }

        public override void Info(object message)
        {
            if (!_log.IsInfoEnabled) return;

            _log.Info(message);
        }

        public override void InfoFormat(string format, params object[] args)
        {
            if (!_log.IsInfoEnabled) return;

            _log.InfoFormat(format, args);
        }

        public override void Debug(object message)
        {
            if (!_log.IsDebugEnabled) return;

            _log.Debug(message);
        }

        public override void DebugFormat(string format, params object[] args)
        {
            if (!_log.IsDebugEnabled) return;

            _log.DebugFormat(format, args);
        }

        public override void Error(object message)
        {
            if (!_log.IsErrorEnabled) return;

            _log.Error(message);
        }

        public override void Error(object message, Exception exception)
        {
            if (!_log.IsErrorEnabled) return;

            _log.Error(message, exception);
        }

        public override void ErrorFormat(string format, params object[] args)
        {
            if (!_log.IsErrorEnabled) return;

            _log.ErrorFormat(format, args);
        }

        public override void SvcInfo(object message)
        {
            if (!_log.IsInfoEnabled) return;

            _log.Info(message);
        }

        public override void SvcInfo(object message, Exception exception)
        {
            if (!_log.IsInfoEnabled) return;

            _log.Info(message, exception);
        }
    }
}