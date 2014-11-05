#region License and Product Information

// 
//     This file 'RestEndpoint.cs' is part of KonfDB application - 
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

using System.ServiceModel;
using System.ServiceModel.Description;
using KonfDB.Infrastructure.Shell;
using KonfDB.Infrastructure.WCF.Behavior;
using KonfDB.Infrastructure.WCF.Bindings;
using Binding = System.ServiceModel.Channels.Binding;

namespace KonfDB.Infrastructure.WCF.Endpoints
{
    public class RestEndpoint : IEndPoint
    {
        public ServiceEndpoint Host<T>(ServiceHost host, string serverName, string serviceName, IBinding binding)
        {
            const string addressUriFormat = "{0}://{1}:{2}/{3}/";

            string endpointAddress = string.Format(addressUriFormat, "http", serverName, binding.Port, serviceName);

            // add endpoint of service
            var wcfBinding = binding.WcfBinding;
            var restEndPoint = host.AddServiceEndpoint(typeof (T), wcfBinding, endpointAddress);
            restEndPoint.Behaviors.Add(new WebHttpBehavior());
            restEndPoint.Behaviors.Add(new FaultingWebHttpBehavior());

            return restEndPoint;
        }

        internal static Binding CreateMex(ServiceType serviceTypes)
        {
            CurrentContext.Default.Log.Debug("Created MEX type : " + serviceTypes);
            return MetadataExchangeBindings.CreateMexHttpBinding();
        }
    }
}