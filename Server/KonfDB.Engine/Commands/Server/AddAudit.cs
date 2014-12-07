#region License and Product Information

// 
//     This file 'AddAudit.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.Attributes;
using KonfDB.Infrastructure.Common;
using KonfDB.Infrastructure.Database.Entities.Configuration;
using KonfDB.Infrastructure.Database.Enums;
using KonfDB.Infrastructure.Extensions;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;

namespace KonfDB.Engine.Commands.Server
{
    [IgnoreCache]
    internal class AddAudit : IAuthCommand
    {
        private AuditRecordModel model;

        public string Keyword
        {
            get { return "AddAudit"; }
        }

        public string Command
        {
            get { return "AddAudit /a:Area /akey:AreaKey /m:Message /r:Reason [/m1:other] [/m2:other]"; }
        }

        public string Help
        {
            get { return "Adds Audit Record"; }
        }

        public bool IsValid(CommandInput input)
        {
            if (input.HasArgument("record"))
            {
                model = input["record"].FromJsonToObject<AuditRecordModel>();
                return model != null;
            }
            AuditArea auditArea = AuditArea.Suite;
            AuditReason auditReason = AuditReason.Retrieved;
            bool valid = Enum.TryParse(input["a"], out auditArea)
                         && input.HasArgument("akey")
                         && input.HasArgument("m")
                         && Enum.TryParse(input["r"], out auditReason);

            if (valid)
            {
                model = new AuditRecordModel
                {
                    Area = auditArea,
                    Key = input["akey"],
                    Message = input["m"],
                    Reason = auditReason
                };
            }

            return valid;
        }

        public CommandOutput OnExecute(CommandInput arguments)
        {
            model.UserId = arguments.GetUserId();
            if (model.Key == null)
                model.Key = "<Unknown>";
            HostContext.Current.Provider.ConfigurationStore.AddAuditRecord(model);

            return new CommandOutput
            {
                PostAction = CommandOutput.PostCommandAction.None
            };
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