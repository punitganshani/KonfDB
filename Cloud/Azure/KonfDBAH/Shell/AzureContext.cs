#region License and Product Information

// 
//     This file 'AzureContext.cs' is part of KonfDB application - 
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
using System.Configuration;
using System.IO;
using KonfDB.Infrastructure.Configuration;
using KonfDB.Infrastructure.Configuration.Interfaces;
using KonfDB.Infrastructure.Extensions;
using KonfDB.Infrastructure.Shell;

namespace KonfDBAH.Shell
{
    internal class AzureContext
    {
        private static AzureContext _current;

        internal IHostConfig Config
        {
            get { return _config; }
        }

        private static IHostConfig _config;

        internal static AzureContext CreateFrom(string configFilePath)
        {
            if (string.IsNullOrEmpty(configFilePath))
                throw new ArgumentNullException("configFilePath");

            if (!File.Exists(configFilePath))
                throw new ConfigurationErrorsException("Could not find config file: " + configFilePath);

            _config = File.ReadAllText(configFilePath).FromJsonToObject<HostConfig>();
            _current = new AzureContext(_config);
            return _current;
        }

        internal static AzureContext CreateFrom(Stream configStream)
        {
            if (configStream == null)
                throw new ArgumentNullException("configStream");

            _config = configStream.ReadToEnd().FromJsonToObject<HostConfig>();
            _current = new AzureContext(_config);
            return _current;
        }


        internal static AzureContext Current
        {
            get
            {
                if (_current == null)
                    throw new InvalidOperationException("AzureContext should be initialized by CreateFrom");

                return _current;
            }
        }

        private AzureContext(IHostConfig configuration)
        {
            CurrentHostContext.CreateDefault(configuration);
        }
    }
}