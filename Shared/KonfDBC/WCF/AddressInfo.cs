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
using System.ServiceModel.Description;
using KonfDB.Infrastructure.Enums;
using KonfDB.Infrastructure.Shell;
using KonfDB.Infrastructure.WCF.Bindings;
using Binding = System.ServiceModel.Channels.Binding;

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
        public ServiceSecurityMode SecurityMode { get; private set; }

        public AddressInfo(ServiceType type, string serverName, string port, string serviceName,
            ServiceSecurityMode mode)
        {
            if (string.IsNullOrEmpty(serverName) || string.IsNullOrEmpty(port) || string.IsNullOrEmpty(serverName))
                throw new ArgumentException("One or more arguments are either NULL or empty.");

            this.Type = type;
            this.ServerName = serverName;
            this.Port = port;
            this.ServiceName = serviceName;
            this.SecurityMode = mode;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_address))
            {
                var prefix = string.Empty;
                switch (Type)
                {
                    case ServiceType.HTTP:
                    case ServiceType.REST:
                    case ServiceType.WSHTTP:
                        prefix = this.SecurityMode == ServiceSecurityMode.BasicSSL ? "https" : "http";
                        break;
                    case ServiceType.TCP:
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
            Binding binding = null;
            bool useSSL = this.SecurityMode == ServiceSecurityMode.BasicSSL;
            switch (Type)
            {
                case ServiceType.HTTP:
                    prefix = useSSL ? "https" : "http";
                    if (useSSL)
                    {
                        binding = new HttpBinding
                        {
                            Security =
                            {
                                Mode = BasicHttpSecurityMode.Transport,
                                Transport =
                                {
                                    ClientCredentialType = HttpClientCredentialType.None,
                                    Realm = string.Empty,
                                    ProxyCredentialType = HttpProxyCredentialType.None
                                }
                            }
                        };
                    }
                    else
                    {
                        binding = new HttpBinding();
                    }
                    break;
                case ServiceType.WSHTTP:
                    prefix = useSSL ? "https" : "http";
                    if (useSSL)
                    {
                        binding = new WsHttpBinding
                        {
                            Security =
                            {
                                Mode = System.ServiceModel.SecurityMode.Transport,
                                Transport =
                                {
                                    ClientCredentialType = HttpClientCredentialType.None,
                                    Realm = string.Empty,
                                    ProxyCredentialType = HttpProxyCredentialType.None
                                }
                            }
                        };
                    }
                    else
                    {
                        binding = new WsHttpBinding();
                    }
                    break;
                case ServiceType.TCP:
                    binding = new TcpBinding();
                    prefix = "net.tcp";
                    break;
                case ServiceType.REST:
                    if (useSSL)
                    {
                        binding = new RestBinding
                        {
                            Security =
                            {
                                Mode = WebHttpSecurityMode.Transport,
                                Transport =
                                {
                                    ClientCredentialType = HttpClientCredentialType.None,
                                    Realm = string.Empty,
                                    ProxyCredentialType = HttpProxyCredentialType.None
                                }
                            }
                        };
                    }
                    else
                    {
                        binding = new RestBinding();
                    }
                    prefix = useSSL ? "https" : "http";
                    break;
            }

            if (binding == null)
                throw new NullReferenceException("No appropriate binding found for type:" + Type);
            _address = String.Format(UriFormat, prefix, ServerName, Port, ServiceName);


            CurrentContext.Default.Log.SvcInfo(String.Format("Creating WCF Client at {0}", _address));

            channelfactory = new ChannelFactory<I>(binding, new EndpointAddress(_address));

            if (binding is RestBinding)
            {
                channelfactory.Endpoint.Behaviors.Add(new WebHttpBehavior());
            }

            var channel = channelfactory.CreateChannel();
            channelfactory.Endpoint.AttachDataResolver();

            return channel;
        }
    }
}