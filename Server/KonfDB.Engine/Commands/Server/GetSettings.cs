#region License and Product Information

// 
//     This file 'GetSettings.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;

namespace KonfDB.Engine.Commands.Server
{
    [IgnoreCache]
    internal class GetSettings : ICommand
    {
        public string Keyword
        {
            get { return "GetSettings"; }
        }

        public string Command
        {
            get { return "GetSettings /active=true|false /autoLoad=true|false"; }
        }

        public string Help
        {
            get { return "Gets KonfDB Settings. Default, Active & AutoLoad are true"; }
        }

        public bool IsValid(CommandInput input)
        {
            return true;
        }

        public CommandOutput OnExecute(CommandInput arguments)
        {
            var output = new CommandOutput
            {
                PostAction = CommandOutput.PostCommandAction.None,
                DisplayMessage = "Success"
            };

            bool active = true, autoLoad = true;
            if (arguments.HasArgument("active"))
                bool.TryParse(arguments["active"], out active);

            if (arguments.HasArgument("autoLoad"))
                bool.TryParse(arguments["autoLoad"], out autoLoad);

            output.Data = HostContext.Current.Provider.ConfigurationStore.GetSettings(active, autoLoad);

            return output;
        }

        public AppType Type
        {
            get { return AppType.Server; }
        }

        public AuditRecordModel GetAuditCommand(CommandInput input)
        {
            return null;
        }
    }
}