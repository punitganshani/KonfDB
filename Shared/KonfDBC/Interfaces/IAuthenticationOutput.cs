using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace KonfDB.Infrastructure.Interfaces
{
    public interface IAuthenticationOutput
    {
        [DataMember]
        string Token { get; set; }

        [DataMember]
        string Username { get; set; }

        [DataMember]
        bool IsAuthenticated { get; set; }

        [IgnoreDataMember]
        long? UserId { get; set; }

        [DataMember]
        DateTime ExpireUtc { get; set; }
    }
}
