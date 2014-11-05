#region License and Product Information

// 
//     This file 'BindingFactory.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.WCF.Endpoints;

namespace KonfDB.Infrastructure.WCF.Bindings
{
    public class Binding : IBinding
    {
        public string Port { get; set; }
        public System.ServiceModel.Channels.Binding WcfBinding { get; set; }
        public ServiceType ServiceType { get; set; }
        public Type EndPointType { get; set; }

        private Binding(ServiceType type, string port, System.ServiceModel.Channels.Binding wcfBinding,
            Type endpointType)
        {
            ServiceType = type;
            Port = port;
            WcfBinding = wcfBinding;
            EndPointType = endpointType;
        }

        internal static IBinding Create(ServiceType type, string port, System.ServiceModel.Channels.Binding wcfBinding,
            Type endpointType)
        {
            return new Binding(type, port, wcfBinding, endpointType);
        }

        public override string ToString()
        {
            return String.Format("{0} -> {1}", ServiceType, Port);
        }
    }

    public class BindingFactory
    {
        public static IBinding Create(ServiceType type, string port)
        {
            switch (type)
            {
                case ServiceType.BasicHttp:
                    return Binding.Create(type, port, new HttpBinding(), typeof (HttpEndpoint));
                case ServiceType.NetTcp:
                    return Binding.Create(type, port, new TcpBinding(), typeof (NetTcpEndpoint));
                case ServiceType.REST:
                    return Binding.Create(type, port, new RestBinding(), typeof (RestEndpoint));
                case ServiceType.AzureRelay:
                    throw new NotImplementedException("Pending Implementation 'AzureRelay'");
            }

            throw new NotImplementedException("Unknown Binding");
        }
    }
}