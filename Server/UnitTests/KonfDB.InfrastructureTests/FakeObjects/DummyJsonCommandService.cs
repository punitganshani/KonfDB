using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KonfDB.Infrastructure.Services;

namespace KonfDB.InfrastructureTests.FakeObjects
{
    public class DummyJsonCommandService : ICommandService<string>
    {
        public ServiceCommandOutput<string> ExecuteCommand(string command, string token)
        {
            return new ServiceCommandOutput<string>()
            {
                DisplayMessage = "Successfully executed",
                MessageType = CommandOutput.DisplayMessageType.Message
            };
        }

        public string[] GetCommandsStartingWith(string command)
        {
            return null;
        }
    }
}
