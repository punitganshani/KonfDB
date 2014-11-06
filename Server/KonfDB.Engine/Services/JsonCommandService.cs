using KonfDB.Infrastructure.Adapter;
using KonfDB.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KonfDB.Engine.Services
{
    public class JsonCommandService : ICommandService<string>
    {
        private readonly ServiceCore _core = new ServiceCore();
        public ServiceCommandOutput<string> ExecuteCommand(string command, string token)
        {
            var commandOutput = _core.ExecuteCommand(command, token);
            return commandOutput.ConvertForJson();
        }

        public string[] GetCommandsStartingWith(string command)
        {
            return _core.GetCommandsStartingWith(command);
        }
    }
}
