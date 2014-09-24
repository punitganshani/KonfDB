#region License and Product Information

// 
//     This file 'Help.cs' is part of KonfDB application - 
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
using System.Linq;
using System.Text;
using KonfDB.Infrastructure.Common;
using KonfDB.Infrastructure.Services;

namespace KonfDB.Engine.Commands.Shared
{
    internal class HelpCommand : ICommand
    {
        public string Keyword
        {
            get { return "?"; }
        }

        public string Command
        {
            get { return "? or help"; }
        }

        public string Help
        {
            get { return "Displays usage/help"; }
        }

        public bool IsValid(CommandInput input)
        {
            return true;
        }

        public AppType Type
        {
            get { return AppType.Server | AppType.Client; }
        }

        public CommandOutput OnExecute(CommandInput arguments)
        {
            var commandFactory = CommandFactory.Initiate();
            var commands = commandFactory.Commands.OrderBy(x => x.Keyword);
            var output = new CommandOutput();
            var builder = new StringBuilder();

            builder.AppendLine("Usage: <command>");
            builder.AppendLine();
            builder.AppendLine("Commands:");
            builder.AppendLine();

            foreach (var command in commands)
            {
                builder.AppendLine(String.Format("{0,-15}\t{1}", command.Keyword, command.Help));
            }

            output.DisplayMessage = builder.ToString();

            return output;
        }
    }
}