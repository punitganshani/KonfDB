#region License and Product Information

// 
//     This file 'DeleteParam.cs' is part of KonfDB application - 
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

using KonfDB.Infrastructure.Common;
using KonfDB.Infrastructure.Database.Entities.Configuration;
using KonfDB.Infrastructure.Database.Enums;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;

namespace KonfDB.Engine.Commands.Shared
{
    internal class DeleteParam : IAuthCommand
    {
        public string Keyword
        {
            get { return "DeleteParam"; }
        }

        public string Command
        {
            get { return "DeleteParam /pid:ParamId"; }
        }

        public string Help
        {
            get { return "Deletes Param."; }
        }

        public bool IsValid(CommandInput input)
        {
            return input.HasArgument("pid");
        }

        public CommandOutput OnExecute(CommandInput arguments)
        {
            var output = new CommandOutput {PostAction = CommandOutput.PostCommandAction.None};
            var paramId = long.Parse(arguments["pid"]);
            long userId = arguments.GetUserId();
            var success = HostContext.Current.Provider.ConfigurationStore.DeleteParameter(userId, paramId);

            output.DisplayMessage = success ? "Parameter deleted successfully!" : "Could not delete Parameter";

            return output;
        }

        public AppType Type
        {
            get { return AppType.Client | AppType.Server; }
        }

        public AuditRecordModel GetAuditCommand(CommandInput input)
        {
            return new AuditRecordModel
            {
                Area = AuditArea.Parameter,
                Reason = AuditReason.Removed,
                Message = input.Command,
                Key = input["pid"],
                UserId = input.GetUserId()
            };
        }
    }
}