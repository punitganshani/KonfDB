#region License and Product Information

// 
//     This file 'RevokeAccess.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;

namespace KonfDB.Engine.Commands.Shared
{
    [IgnoreCache]
    internal class RevokeAccess : ICommand
    {
        public string Keyword
        {
            get { return "RevokeAccess"; }
        }

        public string Command
        {
            get { return "RevokeAccess /sid:suiteId /user:username"; }
        }

        public string Help
        {
            get { return "Revokes all suite access to a user"; }
        }

        public CommandOutput OnExecute(CommandInput arguments)
        {
            var output = new CommandOutput {PostAction = CommandOutput.PostCommandAction.None};
            long suiteId = long.Parse(arguments["sid"]);
            string username = arguments["user"];
            long loggedInUserId = arguments.GetUserId();

            bool success = AppContext.Current.Provider.ConfigurationStore.RevokeRoleAccessToSuite(suiteId,
                loggedInUserId, username);
            output.Data = success;
            output.DisplayMessage = success
                ? "Grants revoked successfully"
                : "Some problem occured while revoking rights from user";

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
    }
}