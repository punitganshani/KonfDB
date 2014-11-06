using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KonfDB.Infrastructure.Extensions;
using KonfDB.Infrastructure.Services;

namespace KonfDB.Infrastructure.Adapter
{
    public static class ServiceOutputAdapter
    {
        public static NativeServiceCommandOutput ConvertForNative(this CommandOutput commandOutput)
        {
            var output = new NativeServiceCommandOutput
            {
                Data = commandOutput.Data,
                DisplayMessage = commandOutput.DisplayMessage,
                MessageType = commandOutput.MessageType
            };

            return output;
        }

        public static JsonServiceCommandOutput ConvertForJson(this CommandOutput commandOutput)
        {
            var output = new JsonServiceCommandOutput
            {
                Data = commandOutput.Data.ToJsonUnIndented(),
                DisplayMessage = commandOutput.DisplayMessage,
                MessageType = commandOutput.MessageType
            };

            return output;
        }
    }
}
