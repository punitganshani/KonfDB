#region License and Product Information

// 
//     This file 'AddressInfo.cs' is part of KonfDB application - 
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
using System.ServiceModel;
using KonfDB.Infrastructure.Shell;
using KonfDB.Infrastructure.WCF.Bindings;

namespace KonfDB.Infrastructure.WCF
{
    public class AddressInfo
    {
        private string _address;
        private const string UriFormat = "{0}://{1}:{2}/{3}/";

        public ServiceType Type { get; private set; }
        public string ServerName { get; private set; }
        public string Port { get; private set; }
        public string ServiceName { get; private set; }

        public AddressInfo(ServiceType type, string serverName, string port, string serviceName)
        {
            if (string.IsNullOrEmpty(serverName) || string.IsNullOrEmpty(port) || string.IsNullOrEmpty(serverName))
                throw new ArgumentException("One or more arguments are either NULL or empty.");

            this.Type = type;
            this.ServerName = serverName;
            this.Port = port;
            this.ServiceName = serviceName;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_address))
            {
                var prefix = string.Empty;
                switch (Type)
                {
                    case ServiceType.BasicHttp:
                        prefix = "http";
                        break;
                    case ServiceType.NetTcp:
                        prefix = "net.tcp";
                        break;
                }
                _address = String.Format(UriFormat, prefix, ServerName, Port, ServiceName);
            }

            return _address;
        }

        public I CreateChannel<I>()
        {
            ChannelFactory<I> channelfactory = null;
            string prefix = string.Empty;
            switch (Type)
            {
                case ServiceType.BasicHttp:
                    prefix = "http";
                    channelfactory = new ChannelFactory<I>(new HttpBinding());
                    break;
                case ServiceType.NetTcp:
                    channelfactory = new ChannelFactory<I>(new TcpBinding());
                    prefix = "net.tcp";
                    break;
                    //case ServiceType.AzureRelay:
                    //    channelfactory = new ChannelFactory<I>(new NetTcpRelayBinding())
                    //break;
            }

            _address = String.Format(UriFormat, prefix, ServerName, Port, ServiceName);

            CurrentContext.Default.Log.SvcInfo(String.Format("Creating WCF Client at {0}", _address));
            if (channelfactory != null)
            {
                var channel = channelfactory.CreateChannel(new EndpointAddress(_address));
                channelfactory.Endpoint.AttachDataResolver();

                return channel;
            }
            throw new NotImplementedException("Could not create ChannelFactory, check ServiceType:" + Type);
        }
    }
}