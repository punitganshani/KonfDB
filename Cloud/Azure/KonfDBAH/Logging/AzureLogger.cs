#region License and Product Information

// 
//     This file 'AzureLogger.cs' is part of KonfDB application - 
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
using System.Diagnostics;
using KonfDB.Infrastructure.Logging;
using KonfDB.Infrastructure.Utilities;

namespace KonfDBAH.Logging
{
    public sealed class AzureLogger : BaseLogger
    {
        public AzureLogger(IArguments args) : base(args)
        {
        }

        public override void Info(object message)
        {
            Trace.TraceInformation("[KonfDB] Information: " + message);
        }

        public override void InfoFormat(string format, params object[] args)
        {
            Info(String.Format(format, args));
        }

        public override void Debug(object message)
        {
            Trace.TraceInformation("[KonfDB] DEBUG: " + message);
        }

        public override void DebugFormat(string format, params object[] args)
        {
            Debug(String.Format(format, args));
        }

        public override void Error(object message)
        {
            Trace.TraceError("[KonfDB] Error occured: " + message);
        }

        public override void Error(object message, Exception exception)
        {
            if (exception == null)
                return;

            Trace.TraceError("[KonfDB] Error occured: " + message + "Exception Details: " + exception);
        }

        public override void ErrorFormat(string format, params object[] args)
        {
            Error(String.Format(format, args));
        }

        public override void SvcInfo(object message)
        {
            Info(message);
        }

        public override void SvcInfo(object message, Exception exception)
        {
            Error(message, exception);
        }
    }
}