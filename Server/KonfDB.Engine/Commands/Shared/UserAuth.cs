#region License and Product Information

// 
//     This file 'UserAuth.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.Attributes;
using KonfDB.Infrastructure.Caching;
using KonfDB.Infrastructure.Common;
using KonfDB.Infrastructure.Database.Entities.Account;
using KonfDB.Infrastructure.Encryption;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;

namespace KonfDB.Engine.Commands.Shared
{
    [IgnoreCache]
    internal class UserAuth : ICommand
    {
        public string Keyword
        {
            get { return "UserAuth"; }
        }

        public string Command
        {
            get { return "UserAuth /name:username /pwd:password"; }
        }

        public string Help
        {
            get { return "Authenticates a user with his password"; }
        }

        public CommandOutput OnExecute(CommandInput arguments)
        {
            var username = arguments["name"];
            var password = arguments["pwd"];
            DateTime nowUtc = DateTime.UtcNow;
            int uniqueTime = (nowUtc.Day*10) + (nowUtc.Month*100) + ((nowUtc.Year%100)*1000);

            string token =
                EncryptionEngine.Get<SHA256Encryption>()
                    .Encrypt(String.Format("{0}{1}{2}", uniqueTime, username, password), null, null);

            // Authenticate in DB
            AuthenticationModel authInfo =
                CurrentHostContext.Default.Provider.ConfigurationStore.GetAuthenticatedInfo(username, password, token);

            if (!CurrentHostContext.Default.UserTokens.Contains(token))
                CurrentHostContext.Default.UserTokens.Add(token);

            var authenticationOutput = CurrentContext.Default.Cache.Get(token,
                () => new AuthenticationOutput
                {
                    Token = token,
                    Username = username,
                    IsAuthenticated = authInfo.IsAuthenticated,
                    UserId = authInfo.UserId,
                    ExpireUtc = DateTime.MaxValue
                }, CachePolicy.AlwaysLive);

            var output = new CommandOutput
            {
                PostAction = CommandOutput.PostCommandAction.None,
                Data = authenticationOutput,
                DisplayMessage = "Success"
            };
            return output;
        }

        public AppType Type
        {
            get { return AppType.Client | AppType.Server; }
        }

        public bool IsValid(CommandInput input)
        {
            return input.HasArgument("name") && input.HasArgument("pwd");
        }
    }
}