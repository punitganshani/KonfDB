#region License and Product Information

// 
//     This file 'WcfService.cs' is part of KonfDB application - 
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using KonfDB.Infrastructure.Extensions;
using KonfDB.Infrastructure.Shell;
using KonfDB.Infrastructure.WCF.Bindings;
using KonfDB.Infrastructure.WCF.Endpoints;
using Binding = KonfDB.Infrastructure.WCF.Bindings.BindingFactory;

namespace KonfDB.Infrastructure.WCF
{
    public sealed class WcfService<TInterface, TService> where TService : TInterface
    {
        private readonly string _serverName;
        private readonly string _serviceName;
        private ServiceHost _svcHost;
        private AutoResetEvent _pause = new AutoResetEvent(false);
        private Hashtable _ports = new Hashtable();
        private List<IBinding> Bindings { get; set; }

        public WcfService(string serverName, string serviceName)
        {
            //creating binding list
            Bindings = new List<IBinding>();

            _serverName = serverName;
            _serviceName = serviceName;

            //create Service host
            _svcHost = new ServiceHost(typeof (TService));
            _svcHost.Faulted += _svcHost_Faulted;
        }

        private void _svcHost_Faulted(object sender, EventArgs e)
        {
            try
            {
                CurrentContext.Default.Log.Debug("Service is in Faulted state");
                Stop();
                _svcHost.Abort();
                CurrentContext.Default.Log.Debug("Service is stopped");
                _svcHost = new ServiceHost(typeof (TService));
                Host();
                CurrentContext.Default.Log.Info("Service was in Faulted state. Re-hosting the service");
            }
            catch (Exception ex)
            {
                CurrentContext.Default.Log.Error("Communication Exception", ex);
            }
        }

        public void AddBinding(IBinding binding)
        {
            if (_ports.ContainsKey(binding.Port))
            {
                var ex =
                    new Exception("You are trying to add service of type '" + binding.ServiceType + "' on port '" +
                                  binding.Port + "'." + Environment.NewLine
                                  + "Service of type '" + _ports[binding.Port] + "' is already hosted on the same port.");

                CurrentContext.Default.Log.Error("An error occured", ex);
                throw ex;
            }

            _ports.Add(binding.Port, binding.ServiceType);

            Bindings.Add(binding);
            CurrentContext.Default.Log.Debug("Added a binding of the type : " + binding.ServiceType);
        }

        public void Host()
        {
            if (Bindings.Count == 0)
                throw new Exception("No Bindings exposed for this service. Can not host the service");

            ConfigureEndPoints();
            var threadService = new Thread(StartService);
            threadService.Start();
        }

        public void Stop()
        {
            _pause.Set();
        }

        private string GetAddressUrl()
        {
            return Bindings.Aggregate(string.Empty, (current, t) => current + (t.ToString() + Environment.NewLine));
        }

        private void StartService()
        {
            CurrentContext.Default.Log.Debug("Service running on: " + Thread.CurrentThread.ManagedThreadId);
            try
            {
                CurrentContext.Default.Log.Debug("Hosting services on: " + GetAddressUrl());
                _svcHost.Open();
            }
            catch (AddressAlreadyInUseException addressException)
            {
                CurrentContext.Default.Log.Error("An exception occured", addressException);
                throw new FaultException(addressException.Message);
            }
            catch (Exception ex)
            {
                CurrentContext.Default.Log.Error("An exception occured", ex);
                throw new FaultException(ex.Message);
            }
            _pause.WaitOne();
        }

        private void ConfigureEndPoints()
        {
            foreach (var binding in Bindings)
            {
                var endPointTypeInstance = Activator.CreateInstance(binding.EndPointType);

                if (endPointTypeInstance.InheritsFrom<IEndPoint>())
                {
                    var endPoint = (IEndPoint) endPointTypeInstance;
                    var wcfEndPoint = endPoint.Host<TInterface>(_svcHost, _serverName, _serviceName, binding);
                    CurrentContext.Default.Log.Debug("Endpoint available: " + wcfEndPoint.Address);
                }
            }

            var behavior = _svcHost.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            behavior.IncludeExceptionDetailInFaults = true;
            behavior.MaxItemsInObjectGraph = 5242880;

            foreach (var endpoint in _svcHost.Description.Endpoints)
            {
                endpoint.AttachDataResolver();
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            Bindings.ForEach(x => builder.Append(String.Format("{0} on {1} ", typeof (TService).Name, x.ToString())));
            return builder.ToString().Trim();
        }

        ~WcfService()
        {
            _pause = null;
            _svcHost.Close();
            _svcHost = null;
            _ports = null;
            Bindings = null;
        }
    }
}