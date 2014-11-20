using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KonfDB.Infrastructure.Services;

namespace KonfDB.InfrastructureTests.FakeObjects
{
    public class DummyNativeCommandService : ICommandService<object>
    {
        public ServiceCommandOutput<object> ExecuteCommand(string command, string token)
        {
            return new ServiceCommandOutput<object>()
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
