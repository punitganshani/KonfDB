#region License and Product Information

// 
//     This file 'ConnectionFactory.cs' is part of KonfDB application - 
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
using System.Globalization;
using System.IO;
using KonfDB.Infrastructure.Common;
using KonfDB.Infrastructure.Extensions;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;
using KonfDB.Infrastructure.WCF;
using KonfDBCF.Configuration;
using KonfDBCF.Core;
using KonfDBCF.Services;

namespace KonfDBCF
{
    public static class ConnectionFactory
    {
        private static Lazy<ServiceProxy> _commandServiceProxy;

        public static Lazy<ServiceProxy> GetInstance(ClientConfig config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            return LoadFromConfig(config);
        }

        public static Lazy<ServiceProxy> GetInstance(FileInfo configFile)
        {
            var configFileText = File.ReadAllText(configFile.FullName);
            var config = configFileText.FromJsonToObject<ClientConfig>();

            return GetInstance(config);
        }

        private static Lazy<ServiceProxy> LoadFromConfig(ClientConfig config)
        {
            ClientContext.CreateNew(config);

            if (_commandServiceProxy == null)
            {
                var lazyConnection = new Lazy<ServiceProxy>(() =>
                {
                    var contextConfig = ClientContext.Current.Config;
                    var client =
                        WcfClient<ICommandService<object>>.Create(contextConfig.Runtime.Client.GetWcfServiceType(),
                            contextConfig.Runtime.Client.Host,
                            contextConfig.Runtime.Client.Port.ToString(CultureInfo.InvariantCulture),
                            "CommandService");

                    client.OnFaulted += client_OnFaulted;
                    CurrentContext.Default.Log.Info("Connection Established:" + client.ServerName + " Port:" +
                                                    client.Port);

                    return new ServiceProxy(client.Contract, config);
                });

                _commandServiceProxy = lazyConnection;
            }

            return _commandServiceProxy;
        }

        private static void client_OnFaulted(object sender, DataEventArgs<WcfClient<ICommandService<object>>> e)
        {
            CurrentContext.Default.Log.Error("Error occured in communication channel, will re-attempt to create it");
        }
    }
}