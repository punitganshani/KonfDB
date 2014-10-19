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
using System.ServiceModel;
using KonfDB.Infrastructure.Caching;
using KonfDB.Infrastructure.Common;
using KonfDB.Infrastructure.Configuration.Runtime;
using KonfDB.Infrastructure.Extensions;
using KonfDB.Infrastructure.Interfaces;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;
using KonfDB.Infrastructure.Utilities;
using KonfDB.Infrastructure.WCF;
using KonfDBCF.Core;

namespace KonfDBCF
{
    public static class ConnectionFactory
    {
        private static ConnectionProxy _commandServiceProxy;
        private static string _username;
        private static EndPointType _endPointType;
        private static string _host;
        private static int _port;

        public static ConnectionProxy GetInstance()
        {
            if (String.IsNullOrEmpty(_username) || _commandServiceProxy == null)
                throw new UnauthorizedAccessException(@"User has not been authenticated. Please try GetUserToken first.");

            if (CurrentContext.Default == null)
                throw new InvalidOperationException(@"Context has not been set");

            if (_commandServiceProxy != null)
                return _commandServiceProxy;

            throw new UnauthorizedAccessException(@"User has not been authenticated. Please try GetUserToken first.");
        }

        public static string GetUserToken(IArguments arguments)
        {
            CreateInstance(arguments);

            var tokenKey = GetServiceKey(_username);
            return CurrentContext.Default.Cache.Get(tokenKey, () =>
            {
                var authOutput = AuthenticateUser(_username, arguments["password"]);
                return authOutput.IsAuthenticated ? authOutput.Token : null;
            }, InMemoryCacheStore.Mode.AlwaysLive);
        }

        public static string GetUserToken(string serviceType, string host, int port, string username, string password)
        {
            var args = new CommandArgs(String.Format("-type:{0} -host:{1} -port:{2} -username:{3} -password:{4}",
                serviceType, host, port, username, password));

            return GetUserToken(args);
        }

        #region Private Methods

        private static string GetServiceKey(string user)
        {
            return String.Format("$UT${0}$", !string.IsNullOrEmpty(user) ? user : "Anonymous");
        }

        private static IAuthenticationOutput AuthenticateUser(string username, string password)
        {
            try
            {
                // Create User Token
                var authOutput = _commandServiceProxy.ExecuteCommand(Utilities.GetUserAuthCommand(username, password),
                    null);

                var authenticationOutput = authOutput.Data as IAuthenticationOutput;
                if (authenticationOutput == null)
                {
                    throw new InvalidOperationException("Could not authenticate client user :" + username);
                }

                return authenticationOutput;
            }
            catch (EndpointNotFoundException exception)
            {
                throw new InvalidOperationException(String.Format(@"KonfDBH not reachable at {0}:{1}",
                    _host, _port), exception);
            }
        }

        private static void client_OnFaulted(object sender, DataEventArgs<WcfClient<ICommandService>> e)
        {
            CurrentContext.Default.Log.Error("Error occured in communication channel, will re-attempt to create it");
        }

        private static void CreateInstance(IArguments arguments)
        {
            ClientContext.CreateNew(arguments);

            _endPointType = arguments["type"].ToEnum<EndPointType>();
            _port = Int32.Parse(arguments["port"]);
            _username = arguments["username"];
            _host = arguments["host"];

            if (_commandServiceProxy == null)
            {
                WcfClient<ICommandService> client =
                    WcfClient<ICommandService>.Create(_endPointType.GetWcfServiceType(), _host,
                        _port.ToString(CultureInfo.InvariantCulture),
                        "CommandService");
                client.OnFaulted += client_OnFaulted;
                _commandServiceProxy = new ConnectionProxy(client.Contract);

                CurrentContext.Default.Log.Info("Connection Established:" + client.ServerName + " Port:" + client.Port);
            }
        }

        #endregion
    }
}