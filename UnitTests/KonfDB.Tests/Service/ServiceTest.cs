#region License and Product Information

// 
//     This file 'ServiceTest.cs' is part of KonfDB application - 
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

using System.Security.Cryptography.X509Certificates;
using KonfDB.Infrastructure.Enums;
using KonfDB.Infrastructure.Extensions;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Utilities;
using KonfDB.Infrastructure.WCF;
using KonfDB.Infrastructure.WCF.Bindings;
using KonfDB.Tests.FakeObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KonfDB.Tests.Service
{
    [TestClass]
    public class ServiceTest
    {
        private const string ServiceName = "DummyService";
        private string _certificateThumbprint;
        private CertificateConfiguration _certificateConfiguration;
        private const StoreName _storeName = StoreName.Root;

        [TestInitialize]
        [DeploymentItem("RootTrustedCA.cer")]
        [DeploymentItem("Server.cer")]
        public void Init()
        {
            UnitTestContext.CreateNew(new CommandArgs("-type:a -port:b -host:h -username:u -password:p"));
            AddCertificate();
        }

        [TestCleanup]
        [DeploymentItem("RootTrustedCA.cer")]
        [DeploymentItem("Server.cer")]
        public void Clean()
        {
            //TODO: Certificate clean up, etc
        }

        private void AddCertificate()
        {
            var store = new X509Store(_storeName, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            store.Add(new X509Certificate2("RootTrustedCA.cer"));


            var certificate = new X509Certificate2("Server.cer");
            store.Add(certificate);
            _certificateThumbprint = certificate.Thumbprint;

            _certificateConfiguration = new CertificateConfiguration
            {
                FindBy = X509FindType.FindByThumbprint,
                StoreLocation = StoreLocation.LocalMachine,
                StoreName = _storeName,
                Value = _certificateThumbprint
            };

            try
            {
                // Required for server to successfully pick up the certificate
                _certificateConfiguration.BindCertificateToPort("6661");
                _certificateConfiguration.BindCertificateToPort("6671");
                _certificateConfiguration.BindCertificateToPort("6681");
            }
            catch
            {
            }
            store.Close();
        }

        private ServiceCommandOutput<object> CheckServiceConnectivity(string port, ServiceType type,
            ServiceSecurityMode mode)
        {
            var service = new WcfService<ICommandService<object>, DummyNativeCommandService>("localhost", ServiceName);
            service.AddBinding(BindingFactory.Create(new BindingConfiguration {Port = port, ServiceType = type}));
            if (mode == ServiceSecurityMode.BasicSSL)
            {
                var serviceSecurity = new ServiceSecurity
                {
                    SecurityMode = ServiceSecurityMode.BasicSSL,
                    CertificateConfiguration = _certificateConfiguration
                };

                service.SetSecured(serviceSecurity);
            }

            service.Host();

            var client = WcfClient<ICommandService<object>>.Create(type, "localhost", port, ServiceName, mode);
            var output = client.Contract.ExecuteCommand("test", "token");

            service.Stop();
            return output;
        }

        private ServiceCommandOutput<string> CheckServiceConnectivityJson(string port, ServiceType type,
            ServiceSecurityMode mode)
        {
            var service = new WcfService<ICommandService<string>, DummyJsonCommandService>("localhost", ServiceName);
            service.AddBinding(BindingFactory.Create(new BindingConfiguration {Port = port, ServiceType = type}));
            if (mode == ServiceSecurityMode.BasicSSL)
            {
                var serviceSecurity = new ServiceSecurity
                {
                    SecurityMode = ServiceSecurityMode.BasicSSL,
                    CertificateConfiguration = _certificateConfiguration
                };

                service.SetSecured(serviceSecurity);
            }

            service.Host();

            var client = WcfClient<ICommandService<string>>.Create(type, "localhost", port, ServiceName, mode);
            var output = client.Contract.ExecuteCommand("test", "token");

            service.Stop();
            return output;
        }


        [TestMethod]
        public void TestBasicHttp()
        {
            const string port = "6080";
            const ServiceType type = ServiceType.HTTP;
            const ServiceSecurityMode mode = ServiceSecurityMode.None;

            var output = CheckServiceConnectivity(port, type, mode);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.DisplayMessage);
        }

        [TestMethod]
        [DeploymentItem("RootTrustedCA.cer")]
        [DeploymentItem("Server.cer")]
        public void TestBasicHttpSecured()
        {
            const string port = "6681";
            const ServiceType type = ServiceType.HTTP;
            const ServiceSecurityMode mode = ServiceSecurityMode.BasicSSL;

            var output = CheckServiceConnectivity(port, type, mode);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.DisplayMessage);
        }

        [TestMethod]
        public void TestBasicHttpPlus()
        {
            const string port = "6070";
            const ServiceType type = ServiceType.WSHTTP;
            const ServiceSecurityMode mode = ServiceSecurityMode.None;

            var output = CheckServiceConnectivity(port, type, mode);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.DisplayMessage);
        }

        [TestMethod]
        [DeploymentItem("RootTrustedCA.cer")]
        [DeploymentItem("Server.cer")]
        public void TestBasicHttpPlusSecured()
        {
            const string port = "6671";
            const ServiceType type = ServiceType.WSHTTP;
            const ServiceSecurityMode mode = ServiceSecurityMode.BasicSSL;

            var output = CheckServiceConnectivity(port, type, mode);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.DisplayMessage);
        }

        [TestMethod]
        public void TestTCP()
        {
            const string port = "6050";
            const ServiceType type = ServiceType.TCP;
            const ServiceSecurityMode mode = ServiceSecurityMode.None;

            var output = CheckServiceConnectivity(port, type, mode);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.DisplayMessage);
        }

        [TestMethod]
        public void TestREST()
        {
            const string port = "6060";
            const ServiceType type = ServiceType.REST;
            const ServiceSecurityMode mode = ServiceSecurityMode.None;

            var output = CheckServiceConnectivityJson(port, type, mode);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.DisplayMessage);
        }

        [TestMethod]
        [DeploymentItem("RootTrustedCA.cer")]
        [DeploymentItem("Server.cer")]
        public void TestRESTSecured()
        {
            const string port = "6661";
            const ServiceType type = ServiceType.REST;
            const ServiceSecurityMode mode = ServiceSecurityMode.BasicSSL;

            var output = CheckServiceConnectivityJson(port, type, mode);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.DisplayMessage);
        }
    }
}