#region License and Product Information

// 
//     This file 'NewApp.cs' is part of KonfDB application - 
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
    internal class NewApp : IAuthCommand
    {
        public string Keyword
        {
            get { return "NewApp"; }
        }

        public string Command
        {
            get { return "NewApp /sid:suiteId /name:Applicationname"; }
        }

        public string Help
        {
            get { return "Creates New Application if the Application does not exist."; }
        }

        public bool IsValid(CommandInput input)
        {
            return input.HasArgument("name") && input.HasArgument("sid");
        }

        public CommandOutput OnExecute(CommandInput arguments)
        {
            var output = new CommandOutput {PostAction = CommandOutput.PostCommandAction.None};
            var name = arguments["name"];
            long suiteId = long.Parse(arguments["sid"]);
            long userId = arguments.GetUserId();
            var application = HostContext.Current.Provider.ConfigurationStore.AddApplication(new ApplicationModel
            {
                ApplicationName = name,
                IsActive = true,
                SuiteId = suiteId,
                UserId = userId
            });

            output.Data = application;
            output.DisplayMessage = "Application Created";

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
                Area = AuditArea.Application,
                Reason = AuditReason.Added,
                Message = input.Command,
                Key = input["name"],
                UserId = input.GetUserId()
            };
        }
    }
}