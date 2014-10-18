using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;
using KonfDBCF.Core;

namespace KonfDBCF
{
    public class ConnectionProxy
    {
        private readonly ICommandService _commandService;

        internal ConnectionProxy(ICommandService commandService)
        {
            if (commandService == null)
                throw new ArgumentNullException("commandService");

            _commandService = commandService;
        }

        public CommandOutput ExecuteCommand(string command, string token)
        {
            CommandOutput commandOutput;

            if (CurrentContext.Default.Cache.Enabled)
            {
                commandOutput = CurrentContext.Default.Cache.Get(command, () =>
                    ExecuteCommandInternal(command, token));
            }
            else
            {
                commandOutput = ExecuteCommandInternal(command, token);
            }

            return commandOutput;
        }

        private CommandOutput ExecuteCommandInternal(string command, string token)
        {
            return _commandService.ExecuteCommand(command, token);
        }


        public string[] GetCommandsStartingWith(string command)
        {
            return _commandService.GetCommandsStartingWith(command);
        }
    }
}
