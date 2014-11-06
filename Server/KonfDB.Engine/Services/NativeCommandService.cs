#region License and Product Information

// 
//     This file 'CommandService.cs' is part of KonfDB application - 
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
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KonfDB.Engine.Commands;
using KonfDB.Infrastructure.Adapter;
using KonfDB.Infrastructure.Attributes;
using KonfDB.Infrastructure.Exceptions;
using KonfDB.Infrastructure.Extensions;
using KonfDB.Infrastructure.Interfaces;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;
using KonfDB.Infrastructure.Utilities;

namespace KonfDB.Engine.Services
{
    internal class NativeCommandService : ICommandService<object>
    {
        private readonly ServiceCore _core = new ServiceCore();
        public ServiceCommandOutput<object> ExecuteCommand(string command, string token)
        {
            var commandOutput = _core.ExecuteCommand(command, token);
            return commandOutput.ConvertForNative();
        }

        public string[] GetCommandsStartingWith(string command)
        {
            return _core.GetCommandsStartingWith(command);
        }
    }
}