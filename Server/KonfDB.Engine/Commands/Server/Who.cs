#region License and Product Information

// 
//     This file 'Who.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;

namespace KonfDB.Engine.Commands.Server
{
    [IgnoreCache]
    internal class Who : ICommand
    {
        public string Keyword
        {
            get { return "Who"; }
        }

        public string Command
        {
            get { return Keyword; }
        }

        public string Help
        {
            get { return "Tells logged in user information"; }
        }

        public bool IsValid(CommandInput input)
        {
            return true;
        }

        public CommandOutput OnExecute(CommandInput arguments)
        {
            var authOutput = AppContext.Current.GetUsers();
            var output = new CommandOutput
            {
                PostAction = CommandOutput.PostCommandAction.None,
                Data = authOutput,
                DisplayMessage = "Success"
            };
            return output;
        }

        public AppType Type
        {
            get { return AppType.Server | AppType.Client; }
        }
    }
}