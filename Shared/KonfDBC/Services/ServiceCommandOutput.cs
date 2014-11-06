using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace KonfDB.Infrastructure.Services
{
    [DataContract(Namespace = ServiceConstants.Schema)]
    [Serializable]
    [KnownType(typeof(NativeServiceCommandOutput))]
    [KnownType(typeof(JsonServiceCommandOutput))]
    public  class ServiceCommandOutput<T>
    {
        [DataContract(Namespace = ServiceConstants.Schema)]
        public enum DisplayMessageType
        {
            [EnumMember] Message = 0,
            [EnumMember] Error = 100
        }

        [DataMember]
        public T Data { get; set; }

        [DataMember]
        public string DisplayMessage { get; set; }

        [DataMember]
        public CommandOutput.DisplayMessageType MessageType { get; set; }
    }

    [DataContract(Namespace = ServiceConstants.Schema)]
    [Serializable]
    public sealed class NativeServiceCommandOutput : ServiceCommandOutput<object>
    {
        
    }

    [DataContract(Namespace = ServiceConstants.Schema)]
    [Serializable]
    public sealed class JsonServiceCommandOutput : ServiceCommandOutput<string>
    {

    }
}
