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
using KonfDB.Infrastructure.WCF.Interfaces;

namespace KonfDB.Infrastructure.WCF.Bindings
{
    public class Binding : IBinding
    {
        public BindingConfiguration Configuration { get; set; }
        public System.ServiceModel.Channels.Binding WcfBinding { get; set; }
        public Type EndPointType { get; set; }
        public DataTypeSupport DataTypes { get; set; }

        private Binding(BindingConfiguration config, System.ServiceModel.Channels.Binding wcfBinding,
            Type endpointType, DataTypeSupport dataTypes)
        {
            Configuration = config;
            WcfBinding = wcfBinding;
            EndPointType = endpointType;
            DataTypes = dataTypes;
        }

        internal static IBinding Create(BindingConfiguration config, System.ServiceModel.Channels.Binding wcfBinding,
            Type endpointType, DataTypeSupport dataTypes)
        {
            return new Binding(config, wcfBinding, endpointType, dataTypes);
        }

        public override string ToString()
        {
            return String.Format("{0} -> {1}", Configuration.ServiceType, Configuration.Port);
        }
    }

    public class BindingFactory
    {
        public static IBinding Create(BindingConfiguration config)
        {
            switch (config.ServiceType)
            {
                case ServiceType.HTTP:
                    return Binding.Create(config, new HttpBinding(), typeof (HttpEndpoint), DataTypeSupport.Native);
                case ServiceType.TCP:
                    return Binding.Create(config, new TcpBinding(), typeof (NetTcpEndpoint), DataTypeSupport.Native);
                case ServiceType.REST:
                    return Binding.Create(config, new RestBinding(), typeof (RestEndpoint), DataTypeSupport.Json);
                case ServiceType.WSHTTP:
                    return Binding.Create(config, new WsHttpBinding(), typeof (WsHttpEndpoint), DataTypeSupport.Native);
            }

            throw new NotImplementedException("Unknown Binding");
        }
    }
}