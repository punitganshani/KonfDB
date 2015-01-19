#region License and Product Information

// 
//     This file 'ServiceProxy.cs' is part of KonfDB application - 
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
using System.ServiceModel;
using KonfDB.Infrastructure.Caching;
using KonfDB.Infrastructure.Interfaces;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;
using KonfDBCF.Configuration;
using KonfDBCF.Core;

namespace KonfDBCF.Services
{
    public sealed class ServiceProxy
    {
        private readonly ICommandService<object> _commandService;
        private readonly ClientConfig _configuration;

        private IAuthenticationOutput _authenticationOutput;

        public ServiceProxy(ICommandService<object> commandService, ClientConfig configuration)
        {
            _commandService = commandService;
            _configuration = configuration;
        }

        public ServiceCommandOutput<object> ExecuteCommand(string command)
        {
            if (_authenticationOutput == null)
            {
                _authenticationOutput = AuthenticateUser(_configuration.Runtime.User.Username,
                    _configuration.Runtime.User.Password);
            }

            if (_authenticationOutput != null && _authenticationOutput.IsAuthenticated)
            {
                ServiceCommandOutput<object> commandOutput;

                if (CurrentContext.Default.Cache.Enabled)
                {
                    commandOutput = CurrentContext.Default.Cache.Get(command, () =>
                        ExecuteCommandInternal(command));
                }
                else
                {
                    commandOutput = ExecuteCommandInternal(command);
                }

                return commandOutput;
            }

            throw new InvalidOperationException("Could not authenticate user");
        }

        #region Private Methods

        private ServiceCommandOutput<object> ExecuteCommandInternal(string command)
        {
            return _commandService.ExecuteCommand(command, _authenticationOutput.Token);
        }

        private IAuthenticationOutput AuthenticateUser(string username, string password)
        {
            try
            {
                // Create User Token
                var authOutput = _commandService.ExecuteCommand(Utilities.GetUserAuthCommand(username, password), null);
                var tokenKey = String.Format("$UT${0}$", !string.IsNullOrEmpty(username) ? username : "Anonymous");
                var authenticationOutput = authOutput.Data as IAuthenticationOutput;
                if (authenticationOutput == null)
                {
                    throw new InvalidOperationException("Could not authenticate client user :" + username);
                }

                CurrentContext.Default.Cache.Get(tokenKey, () => authenticationOutput, CachePolicy.AlwaysLive);
                return authenticationOutput;
            }
            catch (EndpointNotFoundException exception)
            {
                throw new InvalidOperationException(String.Format(@"KonfDBH not reachable at {0}:{1}",
                    ClientContext.Current.Config.Runtime.Client.Host,
                    ClientContext.Current.Config.Runtime.Client.Port), exception);
            }
        }

        #endregion
    }
}