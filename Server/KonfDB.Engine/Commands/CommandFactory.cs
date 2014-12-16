#region License and Product Information

// 
//     This file 'CommandFactory.cs' is part of KonfDB application - 
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

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using KonfDB.Infrastructure.Common;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;

namespace KonfDB.Engine.Commands
{
    internal sealed class CommandFactory
    {
        [ImportMany(typeof (ICommand))] private readonly IEnumerable<ICommand> _commands;
        private readonly AppType _appType;

        public List<ICommand> Commands { get; private set; }

        private static CommandFactory _commandFactory;

        internal static CommandFactory Initiate()
        {
            return _commandFactory ?? (_commandFactory = new CommandFactory(CurrentHostContext.Default.ApplicationType));
        }

        private CommandFactory(AppType appType)
        {
            _appType = appType;
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(@".", "*.dll"));
            catalog.Catalogs.Add(new DirectoryCatalog(@".", "*.exe"));
            var container = new CompositionContainer(catalog);
            _commands = container.GetExportedValues<ICommand>().Where(x => (x.Type & _appType) == _appType);
            Commands = _commands.ToList();
        }
    }
}