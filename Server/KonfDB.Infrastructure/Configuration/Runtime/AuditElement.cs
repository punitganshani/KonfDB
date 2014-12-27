using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace KonfDB.Infrastructure.Configuration.Runtime
{
    public class AuditElement
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
    }
}
