#region License and Product Information

// 
//     This file 'NewSuite.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.Encryption;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;

namespace KonfDB.Engine.Commands.Shared
{
    [IgnoreCache]
    internal class NewSuite : IAuthCommand
    {
        public string Keyword
        {
            get { return "NewSuite"; }
        }

        public string Command
        {
            get { return "NewSuite /name:suitename"; }
        }

        public string Help
        {
            get { return "Creates New Suite if the Suite does not exist."; }
        }

        public bool IsValid(CommandInput input)
        {
            return input.HasArgument("name");
        }

        public CommandOutput OnExecute(CommandInput arguments)
        {
            var output = new CommandOutput {PostAction = CommandOutput.PostCommandAction.None};

            var name = arguments["name"];

            long userId = arguments.GetUserId();

            if (string.IsNullOrEmpty(name))
            {
                output.DisplayMessage = "Suite Name missing";
                output.PostAction = CommandOutput.PostCommandAction.ShowCommandHelp;
                output.MessageType = CommandOutput.DisplayMessageType.Error;
            }
            else
            {
                var keys = EncryptionEngine.Default.CreateKeys();
                var model = new SuiteCreateModel
                {
                    SuiteName = name,
                    SuiteType = SuiteTypes.PersonalWithMultiEnvironment,
                    PrivateKey = keys.Item1,
                    UsesSysEncryption = true,
                    PublicKey = keys.Item2,
                    UserId = userId
                };

                model.Environments.Add(new EnvironmentModel
                {
                    EnvironmentName = "*",
                    EnvironmentType = EnvironmentType.PROD
                });

                model.Applications.Add(new ApplicationModel
                {
                    ApplicationName = "*",
                    Description = "All Applications in Suite " + name
                });

                model.Regions.Add(new RegionModel
                {
                    RegionName = "*",
                    Description = "All Regions in Suite " + name
                });

                model.Servers.Add(new ServerModel
                {
                    ServerName = "*",
                    Description = "All Servers in Suite " + name
                });

                var suite = HostContext.Current.Provider.ConfigurationStore.AddSuite(model);
                output.Data = suite;
                output.DisplayMessage = "Success";

                output.PostAction = CommandOutput.PostCommandAction.None;
            }

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
                Reason = AuditReason.Added,
                Message = input.Command,
                Key = input["name"],
                UserId = input.GetUserId()
            };
        }
    }
}