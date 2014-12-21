#region License and Product Information

// 
//     This file 'LogFactory.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.Configuration.Runtime;
using KonfDB.Infrastructure.Exceptions;
using KonfDB.Infrastructure.Extensions;
using KonfDB.Infrastructure.Utilities;
using log4net;

namespace KonfDB.Infrastructure.Logging
{
    public class LogFactory
    {
        private static BaseLogger _instance;
        internal static BaseLogger CreateInstance(LogElement logElement)
        {
            if (_instance == null)
            {
                Type providerType = Type.GetType(logElement.ProviderType);
                if (providerType == null)
                    throw new InvalidConfigurationException("Could not locate Log Provider :" + logElement.ProviderType);

                if (!providerType.ImplementsClass<BaseLogger>())
                    throw new InvalidConfigurationException("Log Provider does not implement ILogger:" + logElement.ProviderType);

                var args = new CommandArgs(logElement.Parameters);
                _instance = (BaseLogger)Activator.CreateInstance(providerType, args);
            }

            return _instance;
        }
    }
}