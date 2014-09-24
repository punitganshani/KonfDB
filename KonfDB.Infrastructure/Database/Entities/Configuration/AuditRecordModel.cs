#region License and Product Information

// 
//     This file 'AuditRecordModel.cs' is part of KonfDB application - 
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
using System.Runtime.Serialization;
using KonfDB.Infrastructure.Database.Enums;
using KonfDB.Infrastructure.Services;
using Newtonsoft.Json;

namespace KonfDB.Infrastructure.Database.Entities.Configuration
{
    [DataContract(Namespace = ServiceConstants.Schema)]
    [Serializable]
    public class AuditRecordModel : BaseViewModel
    {
        [DataMember]
        [JsonProperty]
        public AuditReason Reason { get; set; }

        [DataMember]
        [JsonProperty]
        public AuditArea Area { get; set; }

        [DataMember]
        [JsonProperty]
        public string Message { get; set; }

        [DataMember]
        [JsonProperty]
        public string Key { get; set; }

        [DataMember]
        [JsonProperty]
        public string Metadata1 { get; set; }

        [DataMember]
        [JsonProperty]
        public string Metadata2 { get; set; }

        public override string ToString()
        {
            return String.Format("AddAudit /a:{0} /akey:{1} /m:{2} /r:{3} /m1:{4} /m2:{5}",
                Area, Key, Message, Reason, Metadata1, Metadata2);
        }
    }
}