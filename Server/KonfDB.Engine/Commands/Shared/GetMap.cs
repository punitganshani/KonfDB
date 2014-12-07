#region License and Product Information

// 
//     This file 'GetMap.cs' is part of KonfDB application - 
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

using System.Collections.Generic;
using KonfDB.Infrastructure.Common;
using KonfDB.Infrastructure.Database.Entities.Configuration;
using KonfDB.Infrastructure.Database.Enums;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;

namespace KonfDB.Engine.Commands.Shared
{
    internal class GetMap : IAuthCommand
    {
        public string Keyword
        {
            get { return "GetMap"; }
        }

        public string Command
        {
            get { return "GetMap /sid:suiteId"; }
        }

        public string Help
        {
            get { return "Gets existing Maps"; }
        }

        public bool IsValid(CommandInput input)
        {
            return input.HasArgument("sid");
        }

        public CommandOutput OnExecute(CommandInput arguments)
        {
            var output = new CommandOutput {PostAction = CommandOutput.PostCommandAction.None};
            long suiteId = long.Parse(arguments["sid"]);
            long userId = arguments.GetUserId();
            List<MappingModel> listOfMaps = HostContext.Current.Provider.ConfigurationStore.GetMapping(userId, suiteId);
            output.Data = listOfMaps;
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
                Area = AuditArea.Mapping,
                Reason = AuditReason.Retrieved,
                Message = input.Command,
                Key = input["sid"],
                UserId = input.GetUserId()
            };
        }
    }
}