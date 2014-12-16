#region License and Product Information

// 
//     This file 'Get.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.Common;
using KonfDB.Infrastructure.Database.Entities.Configuration;
using KonfDB.Infrastructure.Database.Enums;
using KonfDB.Infrastructure.Encryption;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;

namespace KonfDB.Engine.Commands.Shared
{
    internal class Get : IAuthCommand
    {
        public string Keyword
        {
            get { return "Get"; }
        }

        public string Command
        {
            get
            {
                return "Get /app:id|name [/server:id|name|*] [/env:id|name|*] [/region:id|name|*] [/unprotect:publicKey]";
            }
        }

        public string Help
        {
            get { return "Gets Config"; }
        }

        public bool IsValid(CommandInput input)
        {
            return input.HasArgument("app");
        }

        public CommandOutput OnExecute(CommandInput arguments)
        {
            var output = new CommandOutput {PostAction = CommandOutput.PostCommandAction.None};
            long appId = -1, serverId = -1, envId = -1, regionId = -1;
            long userId = arguments.GetUserId();
            string server, env, region;

            server = !arguments.HasArgument("server") ? "*" : arguments["server"];
            env = !arguments.HasArgument("env") ? "*" : arguments["env"];
            region = !arguments.HasArgument("region") ? "*" : arguments["region"];

            if (!long.TryParse(arguments["app"], out appId))
                appId =
                    CurrentHostContext.Default.Provider.ConfigurationStore.GetApplication(userId, arguments["app"])
                        .ApplicationId.GetValueOrDefault();

            if (!long.TryParse(server, out serverId))
                serverId =
                    CurrentHostContext.Default.Provider.ConfigurationStore.GetServer(userId, server)
                        .ServerId.GetValueOrDefault(-1);

            if (!long.TryParse(env, out envId))
                envId =
                    CurrentHostContext.Default.Provider.ConfigurationStore.GetEnvironment(userId, env)
                        .EnvironmentId.GetValueOrDefault(-1);

            if (!long.TryParse(region, out regionId))
                regionId =
                    CurrentHostContext.Default.Provider.ConfigurationStore.GetRegion(userId, region)
                        .RegionId.GetValueOrDefault(-1);

            List<ConfigurationModel> model =
                CurrentHostContext.Default.Provider.ConfigurationStore.GetConfigurations(userId,
                    appId,
                    serverId, envId, regionId, string.Empty);

            model.ForEach(config =>
            {
                if (config.IsEncrypted)
                {
                    var pk = arguments["unprotect"];
                    if (pk != null)
                    {
                        var suite =
                            CurrentHostContext.Default.Provider.ConfigurationStore.GetSuite(arguments.GetUserId(),
                                config.SuiteId);

                        if (suite.PublicKey.Equals(pk, StringComparison.InvariantCulture))
                        {
                            config.ParameterValue = EncryptionEngine.Default.Decrypt(config.ParameterValue,
                                suite.PrivateKey);
                        }
                    }
                }
            });

            output.Data = model;
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
                Reason = AuditReason.Executed,
                Message = input.Command,
                Key = input["app"],
                UserId = input.GetUserId()
            };
        }
    }
}