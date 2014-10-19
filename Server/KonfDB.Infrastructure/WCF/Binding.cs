#region License and Product Information

// 
//     This file 'Binding.cs' is part of KonfDB application - 
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
using System.ServiceModel.Description;
using KonfDB.Infrastructure.Shell;
using KonfDB.Infrastructure.WCF.ServiceTypes;

namespace KonfDB.Infrastructure.WCF
{
    /// <summary>
    ///     This class represents the Binding types that the user can
    ///     expose and consume
    /// </summary>
    public class Binding
    {
        public string Port { get; private set; }
        public System.ServiceModel.Channels.Binding WcfBinding { get; private set; }
        public ServiceType ServiceType { get; private set; }
        public string Prefix { get; private set; }


        public static Binding Create(ServiceType type, string port)
        {
            Binding binding = new Binding(type, port);

            switch (type)
            {
                case ServiceType.BasicHttp:
                    binding.Prefix = "http";
                    binding.WcfBinding = new BasicHttp();
                    break;
                case ServiceType.NetTcp:
                    binding.Prefix = "net.tcp";
                    binding.WcfBinding = new NetTcp();
                    break;
                case ServiceType.REST:
                    binding.Prefix = "http";
                    binding.WcfBinding = new WebHttp();
                    break;
                case ServiceType.AzureRelay:
                    throw new NotImplementedException("Pending Implementation 'AzureRelay'");
            }
            CurrentContext.Default.Log.Debug("Created binding type : " + type);
            return binding;
        }

        private Binding(ServiceType type, string port)
        {
            ServiceType = type;
            Port = port;
        }

        internal static System.ServiceModel.Channels.Binding CreateMex(ServiceType serviceTypes)
        {
            switch (serviceTypes)
            {
                case ServiceType.BasicHttp:
                    return MetadataExchangeBindings.CreateMexHttpBinding();
                case ServiceType.NetTcp:
                    return MetadataExchangeBindings.CreateMexTcpBinding();
                case ServiceType.REST:
                    return MetadataExchangeBindings.CreateMexHttpBinding();
            }

            CurrentContext.Default.Log.Debug("Created MEX type : " + serviceTypes);
            return MetadataExchangeBindings.CreateMexHttpBinding();
        }

        public override string ToString()
        {
            return string.Format("{0} ==> {1}", this.ServiceType, this.Port);
        }
    }
}