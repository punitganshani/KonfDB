#region License and Product Information

// 
//     This file 'NewUser.cs' is part of KonfDB application - 
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

using KonfDB.Infrastructure.Attributes;
using KonfDB.Infrastructure.Common;
using KonfDB.Infrastructure.Database.Entities.Account;
using KonfDB.Infrastructure.Database.Entities.Configuration;
using KonfDB.Infrastructure.Database.Enums;
using KonfDB.Infrastructure.Exceptions;
using KonfDB.Infrastructure.Extensions;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;

namespace KonfDB.Engine.Commands.Shared
{
    [IgnoreCache]
    internal class NewUser : IAuditCommand
    {
        public string Keyword
        {
            get { return "NewUser"; }
        }

        public string Command
        {
            get { return "NewUser /name:username /pwd:password /cpwd:password"; }
        }

        public string Help
        {
            get { return "Adds a user with his password"; }
        }

        public CommandOutput OnExecute(CommandInput arguments)
        {
            var output = new CommandOutput();
            var username = arguments["name"];
            var password = arguments["pwd"];
            var confirmPassword = arguments["cpwd"];
            bool runSilent = arguments.HasArgument("silent");

            if (!password.Equals(confirmPassword))
            {
                output.DisplayMessage = "Password and Confirm Password do not match";
                output.PostAction = CommandOutput.PostCommandAction.ShowCommandHelp;
                return output;
            }
            try
            {
                RegisterModel model = HostContext.Current.Provider.ConfigurationStore.AddUser(username, password,
                    username.GetRandom());

                output.DisplayMessage = "User Added";
                output.Data = model;
                output.PostAction = CommandOutput.PostCommandAction.None;
            }
            catch (UserAlreadyExistsException ex)
            {
                if (!runSilent)
                {
                    output.DisplayMessage = ex.Message;
                }
                output.PostAction = CommandOutput.PostCommandAction.None;
            }
            catch
            {
                if (!runSilent)
                {
                    throw;
                }
            }

            return output;
        }

        public AppType Type
        {
            get { return AppType.Client | AppType.Server; }
        }

        public bool IsValid(CommandInput input)
        {
            return input.HasArgument("name") && input.HasArgument("pwd") && input.HasArgument("cpwd");
        }

        public AuditRecordModel GetAuditCommand(CommandInput input)
        {
            return new AuditRecordModel
            {
                Area = AuditArea.User,
                Reason = AuditReason.Added,
                Message = input.Command,
                Key = input["name"],
                UserId = input.GetUserId()
            };
        }
    }
}