#region License and Product Information

// 
//     This file 'WorkerRole.cs' is part of KonfDB application - 
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using KonfDB.Engine.Services;
using KonfDB.Infrastructure.Enums;
using KonfDB.Infrastructure.Extensions;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;
using KonfDB.Infrastructure.Utilities;
using KonfDB.Infrastructure.WCF;
using KonfDB.Infrastructure.WCF.Bindings;
using KonfDB.Infrastructure.WCF.Interfaces;
using KonfDBAH.Shell;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace KonfDBAH
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private readonly IArguments _arguments;
        private WcfService<ICommandService<object>, NativeCommandService> _serviceHostNative;
        private WcfService<ICommandService<string>, JsonCommandService> _serviceHostJson;
        public ServiceCore ServiceFacade;
        public string AuthenticationToken;

        public override void Run()
        {
            Trace.TraceInformation("KonfDBAH is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();
            AzureContext.CreateFrom("konfdb.json");

            CurrentContext.Default.Log.Info("KonfDBAH Started Successfully!");

            #region Run Command Service

            ServiceFacade = new ServiceCore();

            // Ensure that the super user admin exists
            ServiceFacade.ExecuteCommand(String.Format("NewUser /name:{0} /pwd:{1} /cpwd:{1} /role:admin /silent",
                AzureContext.Current.Config.Runtime.SuperUser.Username,
                AzureContext.Current.Config.Runtime.SuperUser.Password), null);

            // Ensure that the super user readonly exists
            ServiceFacade.ExecuteCommand(
                String.Format("NewUser /name:{0}_ro /pwd:{1} /cpwd:{1} /role:readonly /silent",
                    AzureContext.Current.Config.Runtime.SuperUser.Username,
                    AzureContext.Current.Config.Runtime.SuperUser.Password), null);

            var serviceConfig = AzureContext.Current.Config.Runtime.Server;
            _serviceHostNative = new WcfService<ICommandService<object>, NativeCommandService>("localhost",
                "CommandService");
            _serviceHostJson = new WcfService<ICommandService<string>, JsonCommandService>("localhost", "CommandService");

            for (int i = 0; i < serviceConfig.Count; i++)
            {
                var configuration = new BindingConfiguration
                {
                    Port = serviceConfig[i].Port.ToString(CultureInfo.InvariantCulture),
                    ServiceType = serviceConfig[i].GetWcfServiceType()
                };

                var binding = BindingFactory.Create(configuration);
                if (binding.DataTypes.IsSet(DataTypeSupport.Native))
                {
                    _serviceHostNative.AddBinding(binding);
                }
                else if (binding.DataTypes.IsSet(DataTypeSupport.Json))
                {
                    _serviceHostJson.AddBinding(binding);
                }
            }

            if (AzureContext.Current.Config.Runtime.ServiceSecurity == ServiceSecurityMode.BasicSSL)
            {
                var serviceSecurity = new ServiceSecurity
                {
                    CertificateConfiguration = AzureContext.Current.Config.Certificate.Default,
                    SecurityMode = AzureContext.Current.Config.Runtime.ServiceSecurity
                };

                _serviceHostJson.SetSecured(serviceSecurity);
                _serviceHostNative.SetSecured(serviceSecurity);
            }

            _serviceHostNative.Host();
            _serviceHostJson.Host();

            var authOutput = ServiceFacade.ExecuteCommand(String.Format("UserAuth /name:{0} /pwd:{1}",
                AzureContext.Current.Config.Runtime.SuperUser.Username,
                AzureContext.Current.Config.Runtime.SuperUser.Password), null);

            var authenticationOutput = authOutput.Data as AuthenticationOutput;
            if (authenticationOutput == null)
            {
                throw new InvalidOperationException(
                    "Could not authenticate server user: " + authOutput.DisplayMessage);
            }

            AuthenticationToken = authenticationOutput.Token;

            // get settings from database
            var settingsOutput = ServiceFacade.ExecuteCommand("GetSettings", null);
            if (settingsOutput != null && settingsOutput.Data != null)
            {
                var settings = (Dictionary<string, string>) settingsOutput.Data;
                foreach (var setting in settings)
                {
                    CurrentContext.Default.ApplicationParams.Add(setting.Key, setting.Value);
                }
            }

            //AppContext.Current.Log.Info("Agent Started: " + _serviceHostNative);
            //AppContext.Current.Log.Info("Agent Started: " + _serviceHostJson);

            #endregion

            return result;
        }

        public override void OnStop()
        {
            CurrentContext.Default.Log.Info("KonfDBAH is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            CurrentContext.Default.Log.Info("KonfDBAH has stopped");
        }

        private Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                // TODO: Replace the following with your own logic.
                while (!cancellationToken.IsCancellationRequested)
                {
                    CurrentContext.Default.Log.Info("Working");

                    Thread.Sleep(1000);
                }
            });
        }
    }
}