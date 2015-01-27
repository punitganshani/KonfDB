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
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using KonfDB.Infrastructure.Enums;
using KonfDB.Infrastructure.Extensions;
using KonfDB.Infrastructure.Shell;
using KonfDB.Infrastructure.WCF.Interfaces;
using Binding = KonfDB.Infrastructure.WCF.Bindings.BindingFactory;

namespace KonfDB.Infrastructure.WCF
{
    public sealed class WcfService<TInterface, TService> where TService : TInterface
    {
        private readonly string _serverName;
        private readonly string _serviceName;
        private readonly string _folder;
        private ServiceHost _svcHost;
        private AutoResetEvent _pause = new AutoResetEvent(false);
        private Hashtable _ports = new Hashtable();
        private List<IBinding> Bindings { get; set; }

        private bool _hosted;
        private bool _secured;
        private ISecurity _security;

        public WcfService(string serverName, string serviceName, string folder = "api")
        {
            if (string.IsNullOrEmpty(serverName))
                throw new ArgumentNullException("serverName");

            if (string.IsNullOrEmpty(serviceName))
                throw new ArgumentNullException("serviceName");

            if (string.IsNullOrEmpty(folder))
                throw new ArgumentNullException("folder");

            //creating binding list
            Bindings = new List<IBinding>();

            _serverName = serverName;
            _serviceName = serviceName;
            _folder = folder;

            //create Service host
            _svcHost = new ServiceHost(typeof(TService));
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
                _svcHost = new ServiceHost(typeof(TService));
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
            if (_ports.ContainsKey(binding.Configuration.Port))
            {
                var ex =
                    new Exception("You are trying to add service of type '" + binding.Configuration.ServiceType +
                                  "' on port '" +
                                  binding.Configuration.Port + "'." + Environment.NewLine
                                  + "Service of type '" + _ports[binding.Configuration.Port] +
                                  "' is already hosted on the same port.");

                CurrentContext.Default.Log.Error("An error occured", ex);
                throw ex;
            }

            _ports.Add(binding.Configuration.Port, binding.Configuration.ServiceType);

            Bindings.Add(binding);
            CurrentContext.Default.Log.Debug("Added a binding of the type : " + binding.Configuration.ServiceType);
        }

        public void Host()
        {
            if (_hosted) return;

            if (Bindings.Count == 0)
            {
                CurrentContext.Default.Log.Debug("No bindings found, will not host the service : ");
                return;
            }

            ConfigureEndPoints();

            var threadService = new Thread(StartService);
            threadService.Start();
        }

        public void Stop()
        {
            _hosted = false;
            _pause.Set();
        }

        private void StartService()
        {
            if (Bindings.Count == 0)
            {
                CurrentContext.Default.Log.Debug("No bindings found, will not host the service : ");
                _pause.WaitOne();
                return;
            }

            CurrentContext.Default.Log.Debug("Communication Service running on thread: " +
                                             Thread.CurrentThread.ManagedThreadId);
            try
            {
                _svcHost.Open();
                _hosted = true;
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
            bool needsSecuredBindings = _security != null && _security.SecurityMode != ServiceSecurityMode.None;
            foreach (var binding in Bindings)
            {
                var serviceInfo = new ServiceInfo
                {
                    Binding = binding,
                    Security = _security,
                    ServerName = _serverName,
                    ServiceName = _serviceName,
                    Folder = _folder
                };

                var endPointTypeInstance = Activator.CreateInstance(binding.EndPointType);
                if (endPointTypeInstance.InheritsFrom<IEndPoint>())
                {
                    var endPoint = (IEndPoint)endPointTypeInstance;
                    ServiceEndpoint wcfEndpoint;
                    if (needsSecuredBindings)
                    {
                        wcfEndpoint = endPoint.HostSecured<TInterface>(_svcHost, serviceInfo);
                    }
                    else
                    {
                        wcfEndpoint = endPoint.Host<TInterface>(_svcHost, serviceInfo);
                    }

                    CurrentContext.Default.Log.Debug("Endpoint available: " + wcfEndpoint.Address + " for type : " +
                                                     binding.Configuration.ServiceType);
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
            Bindings.ForEach(x => builder.Append(String.Format("{0} on {1} ", typeof(TService).Name, x.ToString())));
            return builder.ToString().Trim();
        }

        ~WcfService()
        {
            _pause = null;
            _svcHost.Close();
            _svcHost = null;
            _ports = null;
            Bindings = null;
            _secured = false;
            _hosted = false;
        }

        public void SetSecured(ISecurity security)
        {
            if (_secured) return;
            if (Bindings.Count == 0) return;
            if (_svcHost.Credentials.ServiceCertificate.Certificate != null) return;

            _security = security;
            _svcHost.Credentials.ServiceCertificate.Certificate = security.CertificateConfiguration.GetX509Certificate();
            _secured = true;
        }
    }
}