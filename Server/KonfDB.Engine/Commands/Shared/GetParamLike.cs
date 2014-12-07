#region License and Product Information

// 
//     This file 'GetParamLike.cs' is part of KonfDB application - 
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
    internal class GetParamLike : IAuthCommand
    {
        private const string DoesNotExist = "Parameters do not exist in the suite";

        public string Keyword
        {
            get { return "GetParamLike"; }
        }

        public string Command
        {
            get { return "GetParamLike /sid:suiteId /name:NameStartsWith [/pk:publicKey]"; }
        }

        public string Help
        {
            get { return "Lists all parameters with values"; }
        }

        public bool IsValid(CommandInput input)
        {
            return input.HasArgument("sid") && input.HasArgument("name");
        }

        public CommandOutput OnExecute(CommandInput arguments)
        {
            var output = new CommandOutput {PostAction = CommandOutput.PostCommandAction.None};
            long suiteId = -1;
            long userId = arguments.GetUserId();
            long.TryParse(arguments["sid"], out suiteId);
            var name = arguments["name"];

            List<ParameterModel> listOfParams = new List<ParameterModel>();

            if (suiteId != -1)
            {
                listOfParams = HostContext.Current.Provider.ConfigurationStore.GetParametersLike(userId, suiteId, name);
            }
            else
            {
                output.DisplayMessage = "No such suite id exists: " + suiteId;
                output.PostAction = CommandOutput.PostCommandAction.ShowCommandHelp;
                output.MessageType = CommandOutput.DisplayMessageType.Error;
            }

            if (listOfParams.Count > 0)
            {
                var pk = arguments["pk"];
                if (pk != null)
                {
                    var suite = HostContext.Current.Provider.ConfigurationStore.GetSuite(userId, suiteId);

                    if (suite.PublicKey.Equals(pk, StringComparison.InvariantCulture))
                    {
                        listOfParams.ForEach(
                            x =>
                            {
                                x.ParameterValue = EncryptionEngine.Default.Decrypt(x.ParameterValue, suite.PrivateKey);
                            });
                    }
                    else
                    {
                        output.DisplayMessage = "Invalid combination of Private / Public Key to decrypt ParameterValue";
                        output.MessageType = CommandOutput.DisplayMessageType.Error;

                        return output;
                    }
                }
                output.Data = listOfParams;
                output.DisplayMessage = "Success";
            }
            else
            {
                output.DisplayMessage = DoesNotExist;
                output.MessageType = CommandOutput.DisplayMessageType.Error;
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
                Area = AuditArea.Parameter,
                Reason = AuditReason.Retrieved,
                Message = input.Command,
                Key = input["name"] + "-" + input["sid"],
                UserId = input.GetUserId()
            };
        }
    }
}