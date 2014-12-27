#region License and Product Information

// 
//     This file 'CConnectionFactory.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.Interfaces;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;
using KonfDB.Infrastructure.WCF;
using KonfDBCF.Core;

namespace KonfDBCF
{
    public static class CConnectionFactory
    {
        private static ConnectionProxy<object> _commandServiceProxy;

        public static ConnectionProxy<object> GetInstance()
        {
            if (_commandServiceProxy == null)
            {
                WcfClient<ICommandService<object>> client =
                    WcfClient<ICommandService<object>>.Create(
                        ClientContextFromConfig.Current.Config.Runtime.Client.GetWcfServiceType(),
                        ClientContextFromConfig.Current.Config.Runtime.Client.Host,
                        ClientContextFromConfig.Current.Config.Runtime.Client.Port.ToString(CultureInfo.InvariantCulture),
                        "CommandService");

                client.OnFaulted += client_OnFaulted;

                var commandService = client.Contract;

                _commandServiceProxy = new ConnectionProxy<object>(commandService);

                CurrentContext.Default.Log.Info("Connection Established:" + client.ServerName + " Port:" + client.Port);
            }

            return _commandServiceProxy;
        }

        private static void client_OnFaulted(object sender, DataEventArgs<WcfClient<ICommandService<object>>> e)
        {
            CurrentContext.Default.Log.Error("Error occured in communication channel, will re-attempt to create it");
        }

        public static string GetUserToken()
        {
            var user = ClientContextFromConfig.Current.Config.Runtime.User;
            var tokenKey = GetServiceKey(user.Username);

            return CurrentContext.Default.Cache.Get(tokenKey, () =>
            {
                var authOutput = AuthenticateUser(user.Username, user.Password);
                return authOutput.IsAuthenticated ? authOutput.Token : null;
            }, CachePolicy.AlwaysLive);
        }

        private static string GetServiceKey(string user)
        {
            return String.Format("$UT${0}$", !string.IsNullOrEmpty(user) ? user : "Anonymous");
        }

        private static IAuthenticationOutput AuthenticateUser(string username, string password)
        {
            try
            {
                // Create User Token
                var authOutput = GetInstance().ExecuteCommand(Utilities.GetUserAuthCommand(username, password), null);

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
                    ClientContextFromConfig.Current.Config.Runtime.Client.Host,
                    ClientContextFromConfig.Current.Config.Runtime.Client.Port), exception);
            }
        }
    }
}