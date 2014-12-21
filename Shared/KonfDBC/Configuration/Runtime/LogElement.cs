using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace KonfDB.Infrastructure.Configuration.Runtime
{
    public class LogElement
    {
        [JsonProperty("provider")]
        public string ProviderType { get; set; }

        [JsonProperty("params")]
        public string Parameters { get; set; }

        public LogElement()
        {
            Parameters = string.Empty;
        }
    }
}
