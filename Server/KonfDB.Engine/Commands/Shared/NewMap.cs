#region License and Product Information

// 
//     This file 'NewMap.cs' is part of KonfDB application - 
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

using System.Linq;
using KonfDB.Infrastructure.Common;
using KonfDB.Infrastructure.Database.Entities.Configuration;
using KonfDB.Infrastructure.Database.Enums;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;

namespace KonfDB.Engine.Commands.Shared
{
    internal class NewMap : IAuthCommand
    {
        public string Keyword
        {
            get { return "NewMap"; }
        }

        public string Command
        {
            get
            {
                return
                    "NewMap /sid:suiteId /pid:ParamId [/srid:ServerId|*] [/aid:AppId|*] [/rid:RegionId|*] [/eid:EnvironmentId|*]";
            }
        }

        public string Help
        {
            get { return "Creates New Map of Suite, Server, Region, Application, Environment"; }
        }

        public bool IsValid(CommandInput input)
        {
            return input.HasArgument("pid") && input.HasArgument("sid");
        }

        public CommandOutput OnExecute(CommandInput arguments)
        {
            var output = new CommandOutput {PostAction = CommandOutput.PostCommandAction.None};
            long parameterId = long.Parse(arguments["pid"]);
            long suiteId = long.Parse(arguments["sid"]);
            long serverId = -1, appId = -1, regionId = -1, environmentId = -1;
            long userId = arguments.GetUserId();

            var suite = HostContext.Current.Provider.ConfigurationStore.GetSuite(userId, suiteId);
            var parameter = HostContext.Current.Provider.ConfigurationStore.GetParameter(userId, parameterId);
            if (parameter.SuiteId != suite.SuiteId)
            {
                output.DisplayMessage = "No such parameter id (pid) exists for suite id:" + suite.SuiteId;
                output.MessageType = CommandOutput.DisplayMessageType.Error;
                return output;
            }

            if (!long.TryParse(arguments["srid"], out serverId) && suite.Servers.Any(x => x.ServerName == "*"))
                serverId = suite.Servers.First(x => x.ServerName == "*").ServerId.GetValueOrDefault();

            if (!long.TryParse(arguments["aid"], out appId) && suite.Applications.Any(x => x.ApplicationName == "*"))
                appId = suite.Applications.First(x => x.ApplicationName == "*").ApplicationId.GetValueOrDefault();

            if (!long.TryParse(arguments["rid"], out regionId) && suite.Regions.Any(x => x.RegionName == "*"))
                regionId = suite.Regions.First(x => x.RegionName == "*").RegionId.GetValueOrDefault();

            if (!long.TryParse(arguments["eid"], out environmentId) &&
                suite.Environments.Any(x => x.EnvironmentName == "*" && x.EnvironmentType == EnvironmentType.PROD))
                environmentId =
                    suite.Environments.First(x => x.EnvironmentName == "*" && x.EnvironmentType == EnvironmentType.PROD)
                        .EnvironmentId.GetValueOrDefault();

            if (serverId == 0 || appId == 0 || regionId == 0 || environmentId == 0)
            {
                output.DisplayMessage = "Invalid Map Parameters. Please try providing optional fields";
                output.MessageType = CommandOutput.DisplayMessageType.Error;
                output.PostAction = CommandOutput.PostCommandAction.ShowCommandHelp;

                return output;
            }

            var model = new MappingModel
            {
                ApplicationId = appId,
                EnvironmentId = environmentId,
                ParameterId = parameterId,
                RegionId = regionId,
                ServerId = serverId,
                SuiteId = suiteId,
                UserId = userId
            };

            MappingModel mapping = HostContext.Current.Provider.ConfigurationStore.AddMapping(model);

            output.Data = mapping;
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
                Reason = AuditReason.Added,
                Message = input.Command,
                Key = input["pid"] + "-" + input["sid"],
                UserId = input.GetUserId()
            };
        }
    }
}