#region License and Product Information

// 
//     This file 'HttpPlusEndpoint.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.WCF.Interfaces;

namespace KonfDB.Infrastructure.WCF.Endpoints
{
    public class HttpPlusEndpoint : IEndPoint
    {
        public ServiceEndpoint Host<T>(ServiceHost host, string serverName, string serviceName, IBinding binding)
        {
            const string addressUriFormat = "{0}://{1}:{2}/{3}/";
            string endpointAddress = string.Format(addressUriFormat, "http", serverName, binding.Configuration.Port,
                serviceName);
            ;
            var smb = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (smb == null)
            {
                smb = new ServiceMetadataBehavior {MetadataExporter = {PolicyVersion = PolicyVersion.Policy15}};
                host.Description.Behaviors.Add(smb);
            }

            var mexAddress = new Uri(endpointAddress + "MEX");
            smb.HttpGetEnabled = true;
            smb.HttpGetUrl = mexAddress;

            var serviceEndpoint = host.AddServiceEndpoint(typeof (T), binding.WcfBinding, endpointAddress);
            host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
                MetadataExchangeBindings.CreateMexHttpBinding(), mexAddress);

            return serviceEndpoint;
        }

        public ServiceEndpoint HostSecured<T>(ServiceHost host, string serverName, string serviceName, IBinding binding,
            ISecurity security)
        {
            const string addressUriFormat = "{0}://{1}:{2}/{3}/";
            string endpointAddress = string.Format(addressUriFormat, "https", serverName, binding.Configuration.Port,
                serviceName);
            var httpBinding = binding.WcfBinding as WSHttpBinding;
            var smb = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (smb == null)
            {
                smb = new ServiceMetadataBehavior();
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                host.Description.Behaviors.Add(smb);
            }

            httpBinding.Security.Mode = SecurityMode.Transport;
            httpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;

            var mexAddress = new Uri(endpointAddress + "MEX");
            smb.HttpsGetEnabled = true;
            smb.HttpsGetUrl = mexAddress;

            var serviceEndpoint = host.AddServiceEndpoint(typeof (T), binding.WcfBinding, endpointAddress);
            host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
                MetadataExchangeBindings.CreateMexHttpsBinding(), mexAddress);

            return serviceEndpoint;
        }
    }
}