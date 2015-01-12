#region License and Product Information

// 
//     This file 'JsonCommandService.cs' is part of KonfDB application - 
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
using System.ServiceModel;
using KonfDB.Infrastructure.Adapter;
using KonfDB.Infrastructure.Extensions;
using KonfDB.Infrastructure.Services;

namespace KonfDB.Engine.Services
{
    public class JsonCommandService : ICommandService<string>
    {
        private readonly ServiceCore _core = new ServiceCore();

        public ServiceCommandOutput<string> ExecuteCommand(string command, string token)
        {
            ServiceRequestContext context = new ServiceRequestContext
            {
                SessionId = token,
                Token = token,
                Command = command
            };
            var commandOutput = _core.ExecuteCommand(context);
            return commandOutput.ConvertForJson();
        }

        public string[] GetCommandsStartingWith(string command)
        {
            ServiceRequestContext context = new ServiceRequestContext
            {
                SessionId = Guid.NewGuid().ToString(),
                Token = null,
                Command = command
            };
            return _core.GetCommandsStartingWith(context);
        }
    }
}