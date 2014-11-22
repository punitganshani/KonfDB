#region License and Product Information

// 
//     This file 'Promote.cs' is part of KonfDB application - 
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
using System.Collections.Generic;
using System.Linq;
using KonfDB.Infrastructure.Common;
using KonfDB.Infrastructure.Database.Entities.Configuration;
using KonfDB.Infrastructure.Database.Enums;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;

namespace KonfDB.Engine.Commands.Shared
{
    internal class Promote : IAuthCommand
    {
        public AuditRecordModel GetAuditCommand(CommandInput input)
        {
            return new AuditRecordModel
            {
                Area = AuditArea.Mapping,
                Reason = AuditReason.Added,
                Message = input.Command,
                Key = input["sid"],
                UserId = input.GetUserId()
            };
        }

        public string Keyword
        {
            get { return "Promote"; }
        }

        public string Command
        {
            get { return "Promote /sid:id /from:id /to:id [/clone-param:y|n]"; }
        }

        public string Help
        {
            get
            {
                return
                    "Promotes (default) or clones parameters and mapping from one environment to another with separate ids";
            }
        }

        public CommandOutput OnExecute(CommandInput arguments)
        {
            var output = new CommandOutput {PostAction = CommandOutput.PostCommandAction.None};

            var sid = long.Parse(arguments["sid"]);
            var fromId = long.Parse(arguments["from"]);
            var toId = long.Parse(arguments["to"]);
            var clone = arguments.HasArgument("clone-param") &&
                        arguments["clone-param"].Equals("y", StringComparison.InvariantCultureIgnoreCase);

            var mappingsForSuite = AppContext.Current.Provider.ConfigurationStore.GetMapping(arguments.GetUserId(), sid);
            var sourceEnvironmentsMapping = mappingsForSuite.Where(x => x.EnvironmentId == fromId);
            var environmentsMapping = sourceEnvironmentsMapping as MappingModel[] ?? sourceEnvironmentsMapping.ToArray();
            if (!environmentsMapping.Any())
            {
                output.MessageType = CommandOutput.DisplayMessageType.Error;
                output.DisplayMessage = "No Mappings found";
                return output;
            }

            var targetMapping = new List<MappingModel>();
            if (clone)
            {
                var suiteParameters = AppContext.Current.Provider.ConfigurationStore.GetParameters(
                    arguments.GetUserId(), sid);
                foreach (var mapping in environmentsMapping)
                {
                    var param = suiteParameters.FirstOrDefault(x => x.ParameterId == mapping.ParameterId);
                    if (param != null)
                    {
                        var newParam = AppContext.Current.Provider.ConfigurationStore.AddParameter(param);
                        if (newParam != null)
                        {
                            mapping.ParameterId = newParam.AutoIncrementId;
                            mapping.EnvironmentId = toId;
                            targetMapping.Add(mapping);
                        }
                    }
                }
            }
            else
            {
                foreach (var mapping in environmentsMapping)
                {
                    mapping.EnvironmentId = toId;
                    targetMapping.Add(mapping);
                }
            }

            bool success = true;

            targetMapping.ForEach(
                mapping => success &= AppContext.Current.Provider.ConfigurationStore.AddMapping(mapping) != null);

            output.DisplayMessage = success
                ? String.Format("Mappings{0} from environment {1} to {2}",
                    clone ? "/Parameters Cloned" : " Promoted", fromId, toId)
                : "Some error occured while promoting pamareters/mapping";

            return output;
        }

        public AppType Type
        {
            get { return AppType.Client | AppType.Server; }
        }

        public bool IsValid(CommandInput input)
        {
            return input.HasArgument("sid") && input.HasArgument("from")
                   && input.HasArgument("to");
        }
    }
}