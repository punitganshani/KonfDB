#region License and Product Information

// 
//     This file 'ServiceTypeElement.cs' is part of KonfDB application - 
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

using System;
using System.Configuration;

namespace KonfDB.Infrastructure.Configuration.Runtime
{
    public enum ServiceType
    {
        tcp = 100,
        http = 200,
        rest = 300,
        azurerelay = 400
    }

    public class ServiceTypeElement : ConfigurationElement
    {
        [ConfigurationProperty("port", IsRequired = true, IsKey = true)]
        public int Port
        {
            get { return (int) this["port"]; }
            set { this["port"] = value; }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public ServiceType Type
        {
            get { return (ServiceType) this["type"]; }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("host", IsRequired = false)]
        public string Host
        {
            get { return (string) this["host"]; }
            set { this["host"] = value; }
        }

        public WCF.ServiceType GetWcfServiceType()
        {
            if (Type == ServiceType.http)
                return WCF.ServiceType.BasicHttp;
            if (Type == ServiceType.tcp)
                return WCF.ServiceType.NetTcp;
            if (Type == ServiceType.rest)
                return WCF.ServiceType.REST;
            if (Type == ServiceType.azurerelay)
                return WCF.ServiceType.AzureRelay;

            throw new InvalidOperationException("Invalid ServiceType detected in config:" + Type);
        }
    }
}