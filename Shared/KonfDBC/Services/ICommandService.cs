#region License and Product Information

// 
//     This file 'ICommandService.cs' is part of KonfDB application - 
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

using System.ServiceModel;
using System.ServiceModel.Web;

namespace KonfDB.Infrastructure.Services
{
    [ServiceContract(Namespace = ServiceConstants.Schema, Name = "ICommandService")]
    public interface ICommandService<T> : IService
    {
        [OperationContract(Name = "Execute")]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "/Execute?cmd={command}&token={token}")]
        ServiceCommandOutput<T> ExecuteCommand(string command, string token);

        [OperationContract(Name = "List")]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "/List/{command}")]
        string[] GetCommandsStartingWith(string command);
    }
}