#region License and Product Information

// 
//     This file 'NewParam.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.Encryption;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;

namespace KonfDB.Engine.Commands.Shared
{
    internal class NewParam : IAuthCommand
    {
        public string Keyword
        {
            get { return "NewParam"; }
        }

        public string Command
        {
            get { return "NewParam /sid:suiteId /name:ParameterName [/val:ParameterValue] [/protect]"; }
        }

        public string Help
        {
            get { return "Creates New Parameter entry if the parameter with same value does not exist"; }
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
            var value = arguments["val"];
            var isEncrypted = arguments.HasArgument("protect");
            long userId = arguments.GetUserId();
            if (isEncrypted)
            {
                var suite = AppContext.Current.Provider.ConfigurationStore.GetSuite(userId, suiteId);
                if (suite != null)
                {
                    value = EncryptionEngine.Default.Encrypt(value, suite.PublicKey);
                }
                else
                {
                    output.DisplayMessage = "Could not retrieve suite :" + suiteId;
                    output.MessageType = CommandOutput.DisplayMessageType.Error;
                    output.PostAction = CommandOutput.PostCommandAction.ShowCommandHelp;

                    return output;
                }
            }

            var parameter = AppContext.Current.Provider.ConfigurationStore.AddParameter(new ParameterModel
            {
                ParameterName = name,
                ParameterValue = string.IsNullOrEmpty(value) ? string.Empty : value,
                IsActive = true,
                IsEncrypted = isEncrypted,
                SuiteId = suiteId
            });

            output.Data = parameter;
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
                Area = AuditArea.Parameter,
                Reason = AuditReason.Added,
                Message = input.Command,
                Key = input["name"] + "-" + input["sid"],
                UserId = input.GetUserId()
            };
        }
    }
}