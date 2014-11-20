#region License and Product Information

// 
//     This file 'ServiceCommandOutput.cs' is part of KonfDB application - 
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

namespace KonfDB.Infrastructure.Services
{
    [DataContract(Namespace = ServiceConstants.Schema)]
    [Serializable]
    [KnownType(typeof (NativeServiceCommandOutput))]
    [KnownType(typeof (JsonServiceCommandOutput))]
    public class ServiceCommandOutput<T>
    {
        [DataContract(Namespace = ServiceConstants.Schema)]
        public enum DisplayMessageType
        {
            [EnumMember] Message = 0,
            [EnumMember] Error = 100
        }

        [DataMember]
        public T Data { get; set; }

        [DataMember]
        public string DisplayMessage { get; set; }

        [DataMember]
        public CommandOutput.DisplayMessageType MessageType { get; set; }
    }

    [DataContract(Namespace = ServiceConstants.Schema)]
    [Serializable]
    public sealed class NativeServiceCommandOutput : ServiceCommandOutput<object>
    {
    }

    [DataContract(Namespace = ServiceConstants.Schema)]
    [Serializable]
    public sealed class JsonServiceCommandOutput : ServiceCommandOutput<string>
    {
    }
}