#region License and Product Information

// 
//     This file 'AuditArea.cs' is part of KonfDB application - 
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

using System.Runtime.Serialization;
using KonfDB.Infrastructure.Services;

namespace KonfDB.Infrastructure.Database.Enums
{
    [DataContract(Namespace = ServiceConstants.Schema)]
    public enum AuditArea
    {
        [EnumMember] Access = 100,
        [EnumMember] Application = 200,
        [EnumMember] Environment = 300,
        [EnumMember] Mapping = 400,
        [EnumMember] Parameter = 500,
        [EnumMember] Region = 600,
        [EnumMember] Server = 700,
        [EnumMember] Suite = 800,   
        [EnumMember] Login = 1000,
        [EnumMember] User = 1100
    }
}