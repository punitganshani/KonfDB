#region License and Product Information

// 
//     This file 'GrantAccess.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.Database.Entities.Configuration;
using KonfDB.Infrastructure.Database.Enums;
using KonfDB.Infrastructure.Extensions;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;

namespace KonfDB.Engine.Commands.Shared
{
    [IgnoreCache]
    internal class GrantAccess : IAuthCommand
    {
        public string Keyword
        {
            get { return "GrantAccess"; }
        }

        public string Command
        {
            get { return "GrantAccess /sid:suiteId /user:username [/role:Admin|ReadOnly]"; }
        }

        public string Help
        {
            get { return "Grants suite access to a user. Default Role: ReadOnly"; }
        }

        public CommandOutput OnExecute(CommandInput arguments)
        {
            var output = new CommandOutput {PostAction = CommandOutput.PostCommandAction.None};
            long suiteId = long.Parse(arguments["sid"]);
            string username = arguments["user"];
            long loggedInUserId = arguments.GetUserId();
            RoleType role = arguments.HasArgument("role") ? arguments["role"].ToEnum<RoleType>() : RoleType.ReadOnly;
            bool success = CurrentHostContext.Default.Provider.ConfigurationStore.GrantRoleAccessToSuite(suiteId,
                loggedInUserId,
                username, role);

            output.Data = success;
            output.DisplayMessage = success
                ? "Grants added successfully"
                : "Some problem occured while granting rights to user";
            return output;
        }

        public AppType Type
        {
            get { return AppType.Client | AppType.Server; }
        }

        public bool IsValid(CommandInput input)
        {
            return input.HasArgument("sid") && input.HasArgument("user");
        }

        public AuditRecordModel GetAuditCommand(CommandInput input)
        {
            return new AuditRecordModel
            {
                Area = AuditArea.User,
                Reason = AuditReason.Changed,
                Message = input.Command,
                Key = input["user"],
                UserId = input.GetUserId()
            };
        }
    }
}