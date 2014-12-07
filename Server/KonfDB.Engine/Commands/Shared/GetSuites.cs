#region License and Product Information

// 
//     This file 'GetSuites.cs' is part of KonfDB application - 
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
    internal class GetSuites : IAuthCommand
    {
        public string Keyword
        {
            get { return "GetSuites"; }
        }

        public string Command
        {
            get { return "GetSuites"; }
        }

        public string Help
        {
            get { return "Gets list of existing Suites"; }
        }

        public bool IsValid(CommandInput input)
        {
            return true;
        }

        public CommandOutput OnExecute(CommandInput arguments)
        {
            var output = new CommandOutput {PostAction = CommandOutput.PostCommandAction.None};
            var listOfSuites = HostContext.Current.Provider.ConfigurationStore.GetSuites(arguments.GetUserId());

            output.Data = listOfSuites;
            output.DisplayMessage = "Success";

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
                Area = AuditArea.Suite,
                Reason = AuditReason.Retrieved,
                Message = input.Command,
                Key = "-",
                UserId = input.GetUserId()
            };
        }
    }
}